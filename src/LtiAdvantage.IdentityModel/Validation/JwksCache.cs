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
    /// </remarks>
    public class JwksCache
    {
        private readonly HttpClient _client;
        private readonly TimeSpan _cacheDuration;
        private readonly TimeSpan _minimumRefreshInterval;
        private readonly Dictionary<string, Entry> _entries = new();
        private readonly SemaphoreSlim _lock = new(1, 1);

        /// <summary>
        /// Constructs a new <see cref="JwksCache"/>.
        /// </summary>
        /// <param name="client">The HTTP client used to fetch JWKS documents.</param>
        /// <param name="cacheDuration">How long a fetched key set is served without refetching.</param>
        /// <param name="minimumRefreshInterval">
        /// The shortest interval between two fetches of the same URL, which bounds how often an
        /// unknown key id can trigger a refresh.
        /// </param>
        public JwksCache(HttpClient client, TimeSpan cacheDuration, TimeSpan minimumRefreshInterval)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cacheDuration = cacheDuration;
            _minimumRefreshInterval = minimumRefreshInterval;
        }

        /// <summary>
        /// Returns the signing keys published at <paramref name="jwksUri"/>.
        /// </summary>
        /// <param name="jwksUri">The peer's JWKS URL.</param>
        /// <param name="requiredKeyId">
        /// The key id the caller needs. When supplied and absent from the cached set, the cache
        /// refetches (subject to the minimum refresh interval) so that a freshly rotated key is
        /// picked up without waiting for the cache to expire. Pass null to accept whatever is
        /// cached.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public async Task<IReadOnlyCollection<SecurityKey>> GetSigningKeysAsync(string jwksUri,
            string requiredKeyId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jwksUri))
                throw new ArgumentException("URL is required", nameof(jwksUri));

            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var now = DateTimeOffset.UtcNow;
                _entries.TryGetValue(jwksUri, out var entry);

                if (entry != null && !ShouldRefresh(entry, requiredKeyId, now))
                {
                    return entry.Keys;
                }

                var keys = await FetchAsync(jwksUri, cancellationToken).ConfigureAwait(false);
                _entries[jwksUri] = new Entry(keys, now);
                return keys;
            }
            finally
            {
                _lock.Release();
            }
        }

        private bool ShouldRefresh(Entry entry, string requiredKeyId, DateTimeOffset now)
        {
            if (now - entry.FetchedAt >= _cacheDuration)
            {
                return true;
            }

            // A key id we do not hold usually means the peer rotated. Refetch, but not more
            // often than the minimum interval.
            return requiredKeyId != null
                   && !entry.HasKeyId(requiredKeyId)
                   && now - entry.FetchedAt >= _minimumRefreshInterval;
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

#if NET5_0_OR_GREATER
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
            return JsonWebKeySet.Create(body).GetSigningKeys().ToArray();
        }

        private sealed class Entry
        {
            private readonly HashSet<string> _keyIds;

            public Entry(IReadOnlyCollection<SecurityKey> keys, DateTimeOffset fetchedAt)
            {
                Keys = keys;
                FetchedAt = fetchedAt;
                _keyIds = new HashSet<string>(keys
                    .Select(k => k.KeyId)
                    .Where(id => !string.IsNullOrEmpty(id)), StringComparer.Ordinal);
            }

            public IReadOnlyCollection<SecurityKey> Keys { get; }

            public DateTimeOffset FetchedAt { get; }

            public bool HasKeyId(string keyId) => _keyIds.Contains(keyId);
        }
    }
}
