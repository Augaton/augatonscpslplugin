using System.Collections.Generic;
using System.Linq;
using AugatonLib.Arcade;
using Xunit;

namespace AugatonLib.Tests
{
    public class KillLadderTests
    {
        private sealed class Step
        {
            public Step(int kills) => Kills = kills;

            public int Kills { get; }
        }

        private static KillLadder<Step> Ladder(params int[] thresholds)
        {
            KillLadder<Step> ladder = new KillLadder<Step>(step => step.Kills);

            ladder.SetSteps(thresholds.Select(value => new Step(value)));
            return ladder;
        }

        private static Step AddKills(KillLadder<Step> ladder, string player, int count)
        {
            Step last = null;

            for (int i = 0; i < count; i++)
                last = ladder.Add(player, out _);

            return last;
        }

        [Fact]
        public void AStepIsReachedOnItsExactKill()
        {
            KillLadder<Step> ladder = Ladder(2, 4);

            Assert.Null(ladder.Add("a", out int first));
            Assert.Equal(1, first);

            Step reached = ladder.Add("a", out int second);

            Assert.Equal(2, reached.Kills);
            Assert.Equal(2, second);
            Assert.Null(ladder.Add("a", out _));
        }

        [Fact]
        public void StepsAreSortedWhateverTheConfigOrder()
        {
            KillLadder<Step> ladder = Ladder(5, 1, 3);

            Assert.Equal(new[] { 1, 3, 5 }, ladder.Steps.Select(step => step.Kills));
        }

        [Fact]
        public void InvalidOrRepeatedThresholdsAreIgnored()
        {
            KillLadder<Step> ladder = Ladder(0, -3, 2, 2);

            Assert.Single(ladder.Steps);
        }

        [Fact]
        public void LevelFollowsTheKills()
        {
            KillLadder<Step> ladder = Ladder(2, 4);

            Assert.Equal(0, ladder.LevelOf("a"));
            Assert.Null(ladder.CurrentStep("a"));

            AddKills(ladder, "a", 3);

            Assert.Equal(1, ladder.LevelOf("a"));
            Assert.Equal(2, ladder.CurrentStep("a").Kills);

            AddKills(ladder, "a", 5);

            Assert.Equal(2, ladder.LevelOf("a"));
        }

        [Fact]
        public void ForgetReturnsTheEndedStreak()
        {
            KillLadder<Step> ladder = Ladder(2);

            AddKills(ladder, "a", 3);

            Assert.Equal(3, ladder.Forget("a"));
            Assert.Equal(0, ladder.KillsOf("a"));
            Assert.Equal(0, ladder.Forget("a"));
        }

        [Fact]
        public void PlayersAreCountedSeparately()
        {
            KillLadder<Step> ladder = Ladder(2);

            AddKills(ladder, "a", 2);

            Assert.Equal(0, ladder.KillsOf("b"));
        }

        [Fact]
        public void AnEmptyIdentityCountsNothing()
        {
            KillLadder<Step> ladder = Ladder(1);

            Assert.Null(ladder.Add(null, out int count));
            Assert.Equal(0, count);
            Assert.Equal(0, ladder.KillsOf(string.Empty));
            Assert.Equal(0, ladder.Forget(null));
        }

        [Fact]
        public void NullStepsAreIgnored()
        {
            KillLadder<Step> ladder = new KillLadder<Step>(step => step.Kills);

            ladder.SetSteps(null);
            Assert.Empty(ladder.Steps);

            ladder.SetSteps(new List<Step> { null, new Step(1) });
            Assert.Single(ladder.Steps);
        }

        [Fact]
        public void ClearKeepsTheSteps()
        {
            KillLadder<Step> ladder = Ladder(1);

            AddKills(ladder, "a", 1);
            ladder.Clear();

            Assert.Equal(0, ladder.KillsOf("a"));
            Assert.NotNull(ladder.Add("a", out _));
        }
    }
}
