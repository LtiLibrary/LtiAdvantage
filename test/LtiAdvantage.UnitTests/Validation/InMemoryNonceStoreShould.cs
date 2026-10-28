using System;
using System.Linq;
using System.Threading.Tasks;
using LtiAdvantage.IdentityModel.Validation;
using Xunit;

namespace LtiAdvantage.UnitTests.Validation
{
    public class InMemoryNonceStoreShould
    {
        private static readonly DateTime Start = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        [Fact]
        public async Task AcceptANonceOnce()
        {
            var store = new InMemoryNonceStore(timeProvider: new ManualClock(Start));

            Assert.True(await store.TryConsumeAsync("n-1", Start.AddMinutes(5)));
            Assert.False(await store.TryConsumeAsync("n-1", Start.AddMinutes(5)));
            Assert.True(await store.TryConsumeAsync("n-2", Start.AddMinutes(5)));
        }

        /// <summary>The validator accepts a token until exp plus clock skew, so the nonce is kept that long.</summary>
        [Fact]
        public async Task KeepANonce_UntilExpiryPlusClockSkew()
        {
            var clock = new ManualClock(Start);
            var store = new InMemoryNonceStore(TimeSpan.FromMinutes(5), clock);
            await store.TryConsumeAsync("n-1", Start.AddMinutes(5));

            clock.Now = Start.AddMinutes(9);
            Assert.False(await store.TryConsumeAsync("n-1", Start.AddMinutes(5)));

            clock.Now = Start.AddMinutes(10).AddSeconds(1);
            Assert.True(await store.TryConsumeAsync("n-1", Start.AddMinutes(15)));
        }

        [Fact]
        public async Task DropExpiredNonces()
        {
            var clock = new ManualClock(Start);
            var store = new InMemoryNonceStore(TimeSpan.Zero, clock);
            for (var i = 0; i < 100; i++)
                await store.TryConsumeAsync("n-" + i, Start.AddMinutes(1));

            clock.Now = Start.AddMinutes(2);
            await store.TryConsumeAsync("later", Start.AddMinutes(3));

            Assert.Equal(1, store.Count);
        }

        [Fact]
        public async Task NotOverflow_ForAFarFutureExpiry()
        {
            var store = new InMemoryNonceStore();

            Assert.True(await store.TryConsumeAsync("n-1", DateTime.MaxValue));
            Assert.False(await store.TryConsumeAsync("n-1", DateTime.MaxValue));
        }

        [Fact]
        public async Task LetOnlyOneOfManyConcurrentCallsWin()
        {
            var store = new InMemoryNonceStore();
            var expires = DateTime.UtcNow.AddMinutes(5);

            var results = await Task.WhenAll(Enumerable.Range(0, 64)
                .Select(_ => Task.Run(() => store.TryConsumeAsync("same", expires))));

            Assert.Equal(1, results.Count(r => r));
        }

        private sealed class ManualClock : TimeProvider
        {
            public ManualClock(DateTime now) => Now = now;

            public DateTime Now { get; set; }

            public override DateTimeOffset GetUtcNow() => new(Now, TimeSpan.Zero);
        }
    }
}
