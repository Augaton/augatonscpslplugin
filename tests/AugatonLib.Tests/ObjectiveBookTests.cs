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
        public void ForgettingAPlayerFreesTheirSlot()
        {
            ObjectiveBook<Kind> book = new ObjectiveBook<Kind>();

            book.Assign("a", Kind.Kill, 1);
            book.Forget("a");

            Assert.False(book.Has("a"));
            Assert.True(book.Assign("a", Kind.Escape, 1));
        }
    }
}
