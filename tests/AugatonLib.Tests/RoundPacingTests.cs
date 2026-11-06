using AugatonLib.Arcade;
using Xunit;

namespace AugatonLib.Tests
{
    public class RoundPacingTests
    {
        [Fact]
        public void TheFirstRoundIsAllowed()
        {
            Assert.True(new RoundPacing().TryAdvance(3));
        }

        [Fact]
        public void TheRequiredRoundsPassBeforeTheNextUse()
        {
            RoundPacing pacing = new RoundPacing();

            pacing.MarkUsed();

            Assert.False(pacing.TryAdvance(2));
            Assert.False(pacing.TryAdvance(2));
            Assert.True(pacing.TryAdvance(2));
        }

        [Fact]
        public void ZeroAllowsEveryRound()
        {
            RoundPacing pacing = new RoundPacing();

            pacing.MarkUsed();

            Assert.True(pacing.TryAdvance(0));
        }

        [Fact]
        public void AnAllowedRoundLeftUnusedStaysAllowed()
        {
            RoundPacing pacing = new RoundPacing();

            Assert.True(pacing.TryAdvance(1));
            Assert.True(pacing.TryAdvance(1));
        }
    }
}
