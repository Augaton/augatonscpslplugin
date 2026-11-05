using System;
using AugatonLib.Chaos;
using Xunit;

namespace AugatonLib.Tests
{
    public class EscalationTests : IDisposable
    {
        public EscalationTests() => ChaosState.Reset();

        public void Dispose() => ChaosState.Reset();

        [Fact]
        public void ChaosBonusAddsToTheTimeLevel()
        {
            ChaosState.SetBonusLevel(2);

            Assert.Equal(3, Escalation.Level(120d, 120f, 10));
        }

        [Fact]
        public void ChaosBonusNeverExceedsTheMaximum()
        {
            ChaosState.SetBonusLevel(50);

            Assert.Equal(10, Escalation.Level(120d, 120f, 10));
        }

        [Fact]
        public void DisabledEscalationIgnoresTheChaosBonus()
        {
            ChaosState.SetBonusLevel(5);

            Assert.Equal(0, Escalation.Level(600d, 0f, 10));
        }

        [Fact]
        public void BonusLevelCannotBeNegative()
        {
            ChaosState.SetBonusLevel(-3);

            Assert.Equal(0, ChaosState.BonusLevel);
        }

        [Theory]
        [InlineData(0d, 0)]
        [InlineData(119d, 0)]
        [InlineData(120d, 1)]
        [InlineData(359d, 2)]
        public void LevelGrowsWithEachFullStep(double elapsed, int expected)
        {
            Assert.Equal(expected, Escalation.Level(elapsed, 120f, 10));
        }

        [Fact]
        public void LevelStopsAtTheMaximum()
        {
            Assert.Equal(3, Escalation.Level(100000d, 120f, 3));
        }

        [Theory]
        [InlineData(-5d, 120f, 10)]
        [InlineData(600d, 0f, 10)]
        [InlineData(600d, -1f, 10)]
        [InlineData(600d, 120f, 0)]
        public void InvalidSettingsStayAtLevelZero(double elapsed, float step, int max)
        {
            Assert.Equal(0, Escalation.Level(elapsed, step, max));
        }

        [Fact]
        public void GrowAddsPerLevel()
        {
            Assert.Equal(18, Escalation.Grow(12, 3, 2));
        }

        [Fact]
        public void GrowIgnoresNegativeSteps()
        {
            Assert.Equal(12, Escalation.Grow(12, -3, 2));
        }

        [Fact]
        public void ShrinkRemovesPerLevel()
        {
            Assert.Equal(54f, Escalation.Shrink(60f, 3f, 2, 30f));
        }

        [Fact]
        public void ShrinkNeverGoesBelowTheFloor()
        {
            Assert.Equal(30f, Escalation.Shrink(60f, 3f, 50, 30f));
        }
    }
}
