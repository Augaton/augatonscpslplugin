using AugatonLib.Voting;
using Xunit;

namespace AugatonLib.Tests
{
    public class BallotBoxTests
    {
        private static BallotBox OpenBox(int options = 3)
        {
            BallotBox box = new BallotBox();
            box.Open(options);
            return box;
        }

        [Fact]
        public void ClosedBoxRefusesVotes()
        {
            BallotBox box = new BallotBox();

            Assert.False(box.IsOpen);
            Assert.False(box.Cast("a", 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void RefusesOptionsOutOfRange(int option)
        {
            Assert.False(OpenBox().Cast("a", option));
        }

        [Fact]
        public void RefusesAnonymousVoters()
        {
            Assert.False(OpenBox().Cast(string.Empty, 0));
        }

        [Fact]
        public void AVoterCountsOnceAndMayChangeTheirVote()
        {
            BallotBox box = OpenBox();

            box.Cast("a", 0);
            box.Cast("a", 1);

            Assert.Equal(0, box.CountFor(0));
            Assert.Equal(1, box.CountFor(1));
        }

        [Fact]
        public void LeaderIsTheMostVotedOption()
        {
            BallotBox box = OpenBox();

            box.Cast("a", 2);
            box.Cast("b", 2);
            box.Cast("c", 0);

            Assert.Equal(new[] { 2 }, box.Leaders());
        }

        [Fact]
        public void TieKeepsEveryLeader()
        {
            BallotBox box = OpenBox();

            box.Cast("a", 0);
            box.Cast("b", 2);

            Assert.Equal(new[] { 0, 2 }, box.Leaders());
        }

        [Fact]
        public void NoVoteGivesNoLeader()
        {
            Assert.Empty(OpenBox().Leaders());
        }

        [Fact]
        public void IneligibleOptionsCannotLead()
        {
            BallotBox box = OpenBox();

            box.Cast("a", 0);
            box.Cast("b", 0);
            box.Cast("c", 1);

            Assert.Equal(new[] { 1 }, box.Leaders(option => option != 0));
        }

        [Fact]
        public void ReopeningForgetsPreviousBallots()
        {
            BallotBox box = OpenBox();
            box.Cast("a", 0);

            box.Open(3);

            Assert.Equal(0, box.CountFor(0));
        }
    }
}
