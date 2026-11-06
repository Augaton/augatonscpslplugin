using AugatonLib.Arcade;
using Xunit;

namespace AugatonLib.Tests
{
    public class JackpotTests
    {
        private static Jackpot WithStakes(int stakes)
        {
            Jackpot jackpot = new Jackpot();

            for (int i = 0; i < stakes; i++)
                jackpot.Add();

            return jackpot;
        }

        [Fact]
        public void TheChanceGrowsWithEveryStake()
        {
            Assert.Equal(1f, WithStakes(0).ChancePercent(1f, 0.5f));
            Assert.Equal(3f, WithStakes(4).ChancePercent(1f, 0.5f));
        }

        [Fact]
        public void TheChanceNeverExceedsACertainty()
        {
            Assert.Equal(100f, WithStakes(500).ChancePercent(1f, 1f));
        }

        [Fact]
        public void PrizesGrowWithStakesUpToTheLimit()
        {
            Assert.Equal(1, WithStakes(2).Claim(3, 6));
            Assert.Equal(3, WithStakes(6).Claim(3, 6));
            Assert.Equal(6, WithStakes(99).Claim(3, 6));
        }

        [Fact]
        public void ClaimingEmptiesThePot()
        {
            Jackpot jackpot = WithStakes(5);

            jackpot.Claim(3, 6);

            Assert.Equal(0, jackpot.Stakes);
        }

        [Fact]
        public void ADisabledStepStillPaysOnePrize()
        {
            Assert.Equal(1, WithStakes(50).Claim(0, 6));
            Assert.Equal(1, WithStakes(50).Claim(3, 0));
        }
    }
}
