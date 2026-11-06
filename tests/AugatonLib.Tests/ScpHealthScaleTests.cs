using AugatonLib.Arcade;
using Xunit;

namespace AugatonLib.Tests
{
    public class ScpHealthScaleTests
    {
        [Fact]
        public void TheReferenceKeepsNormalHealth()
        {
            Assert.Equal(1f, ScpHealthScale.Factor(16, 16, 2.5f, 0.6f, 1.5f));
        }

        [Fact]
        public void FewerPlayersLowerTheHealth()
        {
            Assert.Equal(0.8f, ScpHealthScale.Factor(8, 16, 2.5f, 0.6f, 1.5f), 3);
        }

        [Fact]
        public void MorePlayersRaiseTheHealth()
        {
            Assert.Equal(1.2f, ScpHealthScale.Factor(24, 16, 2.5f, 0.6f, 1.5f), 3);
        }

        [Fact]
        public void TheFactorStaysWithinItsBounds()
        {
            Assert.Equal(0.6f, ScpHealthScale.Factor(1, 16, 5f, 0.6f, 1.5f));
            Assert.Equal(1.5f, ScpHealthScale.Factor(40, 16, 5f, 0.6f, 1.5f));
        }

        [Fact]
        public void ZeroPercentChangesNothing()
        {
            Assert.Equal(1f, ScpHealthScale.Factor(2, 16, 0f, 0.6f, 1.5f));
        }
    }
}
