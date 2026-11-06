using AugatonLib.Arcade;
using Xunit;

namespace AugatonLib.Tests
{
    public class ObjectiveBookTests
    {
        private enum Kind
        {
            Kill,
            Escape,
        }

        [Fact]
        public void AnObjectiveCompletesAtItsTarget()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 2);

            Assert.False(book.Advance("a", Kind.Kill));
            Assert.True(book.Advance("a", Kind.Kill));
        }

        [Fact]
        public void ACompletedObjectivePaysOnlyOnce()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 1);

            Assert.True(book.Advance("a", Kind.Kill));
            Assert.False(book.Advance("a", Kind.Kill));
        }

        [Fact]
        public void ACompletedObjectiveBlocksASecondOneUntilCleared()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 1);
            book.Advance("a", Kind.Kill);

            Assert.False(book.Assign("a", Kind.Escape, 1));

            book.Clear();

            Assert.True(book.Assign("a", Kind.Escape, 1));
        }

        [Fact]
        public void AnotherKindDoesNotCount()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 1);

            Assert.False(book.Advance("a", Kind.Escape));
            Assert.True(book.TryGet("a", out Kind kind, out int progress, out int target));
            Assert.Equal(Kind.Kill, kind);
            Assert.Equal(0, progress);
            Assert.Equal(1, target);
        }

        [Fact]
        public void UnknownPlayersAndInvalidValuesAreIgnored()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            Assert.False(book.Assign(null, Kind.Kill, 1));
            Assert.False(book.Assign("a", Kind.Kill, 0));
            Assert.False(book.Advance("b", Kind.Kill));
            Assert.False(book.Advance(null, Kind.Kill));
            Assert.False(book.TryGet("b", out _, out _, out _));
        }

        [Fact]
        public void ALargeStepStillCompletes()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 3);

            Assert.True(book.Advance("a", Kind.Kill, 5));
        }

        [Fact]
        public void AnObjectiveFollowsItsReplacementWithItsProgress()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 2);
            book.Advance("a", Kind.Kill);

            Assert.True(book.Transfer("a", "b"));
            Assert.True(book.Advance("b", Kind.Kill));
        }

        [Fact]
        public void TheFormerHolderCannotPickUpASecondObjective()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 2);
            book.Transfer("a", "b");

            Assert.False(book.Assign("a", Kind.Escape, 1));
            Assert.False(book.Advance("a", Kind.Kill));
        }

        [Fact]
        public void ATransferNeverOverwritesAnExistingObjective()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 2);
            book.Assign("b", Kind.Escape, 1);

            Assert.False(book.Transfer("a", "b"));
            Assert.True(book.TryGet("b", out Kind kind, out _, out _));
            Assert.Equal(Kind.Escape, kind);
            Assert.True(book.Advance("a", Kind.Kill, 2));
        }

        [Fact]
        public void AFinishedObjectiveIsNotTransferred()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 1);
            book.Advance("a", Kind.Kill);

            Assert.False(book.Transfer("a", "b"));
            Assert.False(book.Has("b"));
            Assert.True(book.Has("a"));
        }

        [Fact]
        public void OnlyAnUnfinishedObjectiveCanBeDiscarded()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 1);
            book.Assign("b", Kind.Kill, 1);
            book.Advance("b", Kind.Kill);

            Assert.True(book.Discard("a"));
            Assert.False(book.Discard("b"));
            Assert.False(book.Discard("c"));
        }
    }
}
