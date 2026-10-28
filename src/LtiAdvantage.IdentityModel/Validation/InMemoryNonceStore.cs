using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LtiAdvantage.IdentityModel.Validation
{
    /// <summary>
    /// An <see cref="INonceStore"/> that keeps used nonces in memory until they expire.
    /// </summary>
    /// <remarks>
    /// Only suitable for a tool that runs as a single instance: another instance does not see the
    /// nonces this one recorded, so a token replayed against it would be accepted. For several
    /// instances, implement <see cref="INonceStore"/> over a shared store such as a distributed
    /// cache. Register one instance for the whole app (a singleton), not one per request.
    /// </remarks>
    public class InMemoryNonceStore : INonceStore
    {
        private static readonly TimeSpan PruneInterval = TimeSpan.FromMinutes(1);

        private readonly ConcurrentDictionary<string, DateTimeOffset> _nonces =
            new ConcurrentDictionary<string, DateTimeOffset>(StringComparer.Ordinal);
        private readonly TimeProvider _timeProvider;
        private readonly TimeSpan _clockSkew;
        private long _nextPruneTicks;

        /// <summary>
        /// Creates the store.
        /// </summary>
        /// <param name="clockSkew">
        /// How long past the token's expiry a nonce is kept. Use the validator's
        /// <see cref="LtiTokenValidationOptions.ClockSkew"/> (the default, 5 minutes, matches it),
        /// because the validator still accepts the token for that long.
        /// </param>
        /// <param name="timeProvider">The clock. Defaults to the system clock.</param>
        public InMemoryNonceStore(TimeSpan? clockSkew = null, TimeProvider timeProvider = null)
        {
            _clockSkew = clockSkew ?? TimeSpan.FromMinutes(5);
            if (_clockSkew < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(clockSkew));
            _timeProvider = timeProvider ?? TimeProvider.System;
        }

        /// <summary>The number of nonces currently held, including expired ones not yet pruned.</summary>
        public int Count => _nonces.Count;

        /// <inheritdoc />
        public Task<bool> TryConsumeAsync(string nonce, DateTime expiresAt, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(nonce)) throw new ArgumentException("A nonce is required.", nameof(nonce));
            cancellationToken.ThrowIfCancellationRequested();

            var now = _timeProvider.GetUtcNow();
            PruneIfDue(now);

            var keepUntil = ToUtc(expiresAt) + _clockSkew;
            while (true)
            {
                if (_nonces.TryAdd(nonce, keepUntil))
                {
                    return Task.FromResult(true);
                }

                if (!_nonces.TryGetValue(nonce, out var existing))
                {
                    continue; // Pruned between the two calls; try again.
                }

                if (existing > now)
                {
                    return Task.FromResult(false);
                }

                // An expired entry not yet pruned: replace it atomically, so only one caller wins.
                if (_nonces.TryUpdate(nonce, keepUntil, existing))
                {
                    return Task.FromResult(true);
                }
            }
        }

        private void PruneIfDue(DateTimeOffset now)
        {
            var next = Interlocked.Read(ref _nextPruneTicks);
            if (now.UtcTicks < next
                || Interlocked.CompareExchange(ref _nextPruneTicks, (now + PruneInterval).UtcTicks, next) != next)
            {
                return;
            }

            foreach (var entry in _nonces)
            {
                if (entry.Value <= now)
                {
                    // Removes the entry only if it still holds the expired value.
                    _nonces.TryRemove(entry);
                }
            }
        }

        private static DateTimeOffset ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => new DateTimeOffset(value, TimeSpan.Zero),
            DateTimeKind.Local => new DateTimeOffset(value.ToUniversalTime(), TimeSpan.Zero),
            // JwtSecurityToken.ValidTo is UTC; treat an unspecified kind the same way.
            _ => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc), TimeSpan.Zero)
        };
    }
}
