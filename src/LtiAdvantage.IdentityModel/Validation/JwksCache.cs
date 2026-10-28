using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IdentityModel.Validation
{
    /// <summary>
    /// Fetches and caches the signing keys published at a peer's JWKS URL.
    /// </summary>
    /// <remarks>
    /// Platforms rotate signing keys, so a cache that only expires on a timer will reject valid
    /// tokens for as long as the stale entry lives. This cache also refreshes when it is asked
    /// for a key id it does not hold, rate limited by
    /// <see cref="LtiTokenValidationOptions.MinimumRefreshInterval"/> so that a stream of tokens
    /// with bogus key ids cannot be used to hammer the platform.
    /// <para>
    /// A failed refresh does not throw away keys that were fetched before: the cache keeps serving
    /// them, so a JWKS outage at the platform does not block every launch. Failed attempts count
    /// toward the rate limit too, so an outage cannot turn into a fetch per incoming token.
    /// Stale keys are only served up to a maximum age (7 days by default); after that they are
    /// dropped, so a revoked key cannot stay trusted through a long outage.
    /// </para>
    /// <para>
    /// An empty key set is not trusted to stay empty: the peer may not have published its key yet.
    /// While the cached set is empty, every lookup refetches, still at most once per
    /// <see cref="LtiTokenValidationOptions.MinimumRefreshInterval"/>, and a failed refetch is
    /// reported instead of answering with the empty set.
    /// </para>
    /// </remarks>
    public class JwksCache
    {
        private readonly HttpClient _client;
        private readonly TimeSpan _cacheDuration;
        private readonly TimeSpan _minimumRefreshInterval;
        private readonly Dictionary<string, Entry> _entries = new();

        // When the last fetch of a URL that has no cached keys failed, so retries are rate limited.
        private readonly Dictionary<string, DateTimeOffset> _failedFirstFetches = new();
        private readonly object _gate = new();
        private readonly Dictionary<string, Task<IReadOnlyCollection<SecurityKey>>> _inFlight = new();

        /// <summary>
        /// Constructs a new <see cref="JwksCache"/>.
        /// </summary>
        /// <param name="client">The HTTP client used to fetch JWKS documents.</param>
        /// <param name="cacheDuration">How long a fetched key set is served without refetching.</param>
        /// <param name="minimumRefreshInterval">
        /// The shortest interval between two fetches of the same URL, which bounds how often an
        /// unknown key id can trigger a refresh.
        /// </param>
        /// <param name="maximumStaleness">
        /// How long after their last successful fetch keys may still be served while refreshes
        /// fail. Defaults to 7 days, and is never shorter than <paramref name="cacheDuration"/>.
        /// After that the keys are dropped, so a key the platform has revoked cannot stay trusted
        /// indefinitely just because its JWKS cannot be reached.
        /// </param>
        public JwksCache(HttpClient client, TimeSpan cacheDuration, TimeSpan minimumRefreshInterval,
            TimeSpan? maximumStaleness = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cacheDuration = cacheDuration;
            _minimumRefreshInterval = minimumRefreshInterval;
            // Never shorter than the cache duration: keys still within their cache lifetime are
            // fresh, so a smaller value would only throw away good keys.
            var staleness = maximumStaleness ?? TimeSpan.FromDays(7);
            _maximumStaleness = staleness < cacheDuration ? cacheDuration : staleness;
        }

        private readonly TimeSpan _maximumStaleness;

        private bool IsTooStale(Entry entry, DateTimeOffset now) => now - entry.FetchedAt > _maximumStaleness;

        /// <summary>
        /// Returns the signing keys published at <paramref name="jwksUri"/>.
        /// </summary>
        /// <param name="jwksUri">The peer's JWKS URL.</param>
        /// <param name="requiredKeyId">
        /// The key id the caller needs. When supplied and absent from the cached set, the cache
        /// refetches (subject to the minimum refresh interval) so that a freshly rotated key is
        /// picked up without waiting for the cache to expire. Pass null to accept whatever is
        /// cached, unless the cached set is empty, which is refetched either way.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public async Task<IReadOnlyCollection<SecurityKey>> GetSigningKeysAsync(string jwksUri,
            string requiredKeyId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jwksUri))
                throw new ArgumentException("URL is required", nameof(jwksUri));

            Task<IReadOnlyCollection<SecurityKey>> fetch;

            // The lock only guards the cache's bookkeeping; the HTTP fetch runs outside it, so a
            // slow or hanging JWKS endpoint never blocks callers whose key is already cached.
            lock (_gate)
            {
                var now = DateTimeOffset.UtcNow;
                _entries.TryGetValue(jwksUri, out var entry);
                if (entry != null && IsTooStale(entry, now))
                {
                    // Too old to trust any longer: behave as if nothing was ever fetched.
                    _entries.Remove(jwksUri);
                    entry = null;
                }

                // A key id we do not hold usually means the peer rotated. (Only meaningful when the
                // peer names its keys: if none has a kid, every token would look like a rotation
                // and the cache would degrade into polling.)
                //
                // An empty set counts as missing too: the peer may not have published its key yet
                // (for example a tool registered before its JWKS was up). Without this, an unknown
                // kid could never refetch, because an empty set has no key ids to compare against.
                var keyMissing = entry != null
                    && (entry.Keys.Count == 0
                        || (requiredKeyId != null && entry.HasKeyIds && !entry.HasKeyId(requiredKeyId)));
                var expired = entry != null && now - entry.FetchedAt >= _cacheDuration;

                if (entry != null && !keyMissing)
                {
                    // The cached set can answer this caller. If it has expired, refresh in the
                    // background (once, rate limited) instead of making the caller wait for it.
                    if (expired && !_inFlight.ContainsKey(jwksUri)
                        && now - entry.LastAttemptAt >= _minimumRefreshInterval)
                    {
                        entry.LastAttemptAt = now;
                        var refresh = FetchAndStoreAsync(jwksUri, now);
                        _inFlight[jwksUri] = refresh;
                        // Nobody awaits a background refresh; observe a failure so it is not
                        // reported as an unobserved task exception. Later callers see its outcome.
                        _ = refresh.ContinueWith(t => _ = t.Exception, CancellationToken.None,
                            TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
                    }

                    return entry.Keys;
                }

                // No usable keys: join a fetch that is already running rather than starting another.
                if (!_inFlight.TryGetValue(jwksUri, out fetch))
                {
                    // Never fetch more often than the minimum interval, counting failed attempts.
                    if (entry != null && now - entry.LastAttemptAt < _minimumRefreshInterval)
                    {
                        return entry.Keys;
                    }

                    if (entry == null
                        && _failedFirstFetches.TryGetValue(jwksUri, out var failedAt)
                        && now - failedAt < _minimumRefreshInterval)
                    {
                        throw new HttpRequestException(
                            $"The JWKS at {jwksUri} could not be fetched recently; not retrying until " +
                            $"{failedAt + _minimumRefreshInterval:O}.");
                    }

                    if (entry != null) entry.LastAttemptAt = now;
                    fetch = FetchAndStoreAsync(jwksUri, now);
                    _inFlight[jwksUri] = fetch;
                }
            }

            return await fetch.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches the key set and stores it. If the fetch fails and keys were fetched before,
        /// those are returned instead; otherwise the failure is recorded and rethrown.
        /// </summary>
        /// <remarks>
        /// Shared by every caller waiting for the same URL, so it does not take any one caller's
        /// cancellation token: one caller giving up must not cancel the fetch for the others.
        /// </remarks>
        private async Task<IReadOnlyCollection<SecurityKey>> FetchAndStoreAsync(string jwksUri, DateTimeOffset startedAt)
        {
            // Never complete synchronously: the caller registers this task as in flight while it
            // holds the lock, and the bookkeeping below must only run after that.
            await Task.Yield();

            try
            {
                var keys = await FetchAsync(jwksUri, CancellationToken.None).ConfigureAwait(false);
                lock (_gate)
                {
                    _entries[jwksUri] = new Entry(keys, DateTimeOffset.UtcNow);
                    _failedFirstFetches.Remove(jwksUri);
                }
                return keys;
            }
            catch (Exception)
            {
                lock (_gate)
                {
                    // Judge staleness now, not when the fetch started: a fetch can hang for the whole
                    // HttpClient timeout.
                    if (_entries.TryGetValue(jwksUri, out var entry) && !IsTooStale(entry, DateTimeOffset.UtcNow))
                    {
                        // The next attempt waits the minimum interval either way.
                        entry.LastAttemptAt = startedAt;

                        // Keep serving the keys we have. An empty set is not worth serving: answering
                        // with it would hide the outage behind "no matching key".
                        if (entry.Keys.Count > 0)
                        {
                            return entry.Keys;
                        }
                    }
                    else
                    {
                        _failedFirstFetches[jwksUri] = startedAt;
                    }
                }
                throw;
            }
            finally
            {
                lock (_gate)
                {
                    _inFlight.Remove(jwksUri);
                }
            }
        }

        private async Task<IReadOnlyCollection<SecurityKey>> FetchAsync(string jwksUri,
            CancellationToken cancellationToken)
        {
            using var response = await _client.GetAsync(jwksUri, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Could not fetch JWKS from {jwksUri}: {(int)response.StatusCode} {response.ReasonPhrase}.");
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonWebKeySet.Create(body).GetSigningKeys().ToArray();
        }

        private sealed class Entry
        {
            private readonly HashSet<string> _keyIds;

            public Entry(IReadOnlyCollection<SecurityKey> keys, DateTimeOffset fetchedAt)
            {
                Keys = keys;
                FetchedAt = fetchedAt;
                LastAttemptAt = fetchedAt;
                _keyIds = new HashSet<string>(keys
                    .Select(k => k.KeyId)
                    .Where(id => !string.IsNullOrEmpty(id)), StringComparer.Ordinal);
            }

            public IReadOnlyCollection<SecurityKey> Keys { get; }

            public DateTimeOffset FetchedAt { get; }

            /// <summary>When a fetch was last attempted, successful or not.</summary>
            public DateTimeOffset LastAttemptAt { get; set; }

            public bool HasKeyId(string keyId) => _keyIds.Contains(keyId);

            /// <summary>True when at least one cached key has a kid.</summary>
            public bool HasKeyIds => _keyIds.Count > 0;
        }
    }
}
