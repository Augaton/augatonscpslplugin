using AugatonLib.Caching;
using Xunit;

namespace AugatonLib.Tests
{
    public class RateWindowTests
    {
        [Fact]
        public void AllowsUpToTheLimitThenRefuses()
        {
            RateWindow window = new RateWindow();

            Assert.True(window.TryTake(0f, 2));
            Assert.True(window.TryTake(0.1f, 2));
            Assert.False(window.TryTake(0.2f, 2));
        }

        [Fact]
        public void ANewWindowStartsFromZero()
        {
            RateWindow window = new RateWindow();

            window.TryTake(0f, 1);

            Assert.False(window.TryTake(0.9f, 1));
            Assert.True(window.TryTake(1f, 1));
        }

        [Fact]
        public void AZeroLimitAlwaysRefuses()
        {
            Assert.False(new RateWindow().TryTake(0f, 0));
        }
    }
}
