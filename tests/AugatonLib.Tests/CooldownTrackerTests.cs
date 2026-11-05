using AugatonLib.Caching;
using Xunit;

namespace AugatonLib.Tests
{
    public class CooldownTrackerTests
    {
        private const string UserId = "123@steam";

        [Fact]
        public void UnknownPlayerIsNotOnCooldown()
        {
            Assert.False(new CooldownTracker().IsOnCooldown(UserId, 10f, out double remaining));
            Assert.Equal(0d, remaining);
        }

        [Fact]
        public void RegisteredPlayerIsOnCooldownWithRemainingTime()
        {
            CooldownTracker tracker = new CooldownTracker();
            tracker.Register(UserId);

            Assert.True(tracker.IsOnCooldown(UserId, 60f, out double remaining));
            Assert.InRange(remaining, 59d, 60d);
        }

        [Fact]
        public void ZeroDurationDisablesTheCooldown()
        {
            CooldownTracker tracker = new CooldownTracker();
            tracker.Register(UserId);

            Assert.False(tracker.IsOnCooldown(UserId, 0f, out _));
        }

        [Fact]
        public void RemoveForgetsThePlayer()
        {
            CooldownTracker tracker = new CooldownTracker();
            tracker.Register(UserId);

            tracker.Remove(UserId);

            Assert.False(tracker.IsOnCooldown(UserId, 60f, out _));
        }

        [Fact]
        public void EmptyUserIdIsIgnored()
        {
            CooldownTracker tracker = new CooldownTracker();
            tracker.Register(string.Empty);

            Assert.False(tracker.IsOnCooldown(string.Empty, 60f, out _));
        }
    }
}
