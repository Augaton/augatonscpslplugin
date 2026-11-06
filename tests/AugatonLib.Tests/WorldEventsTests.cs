using System;
using AugatonLib.Runtime;
using Xunit;

namespace AugatonLib.Tests
{
    public class WorldEventsTests : IDisposable
    {
        public WorldEventsTests() => Reset();

        public void Dispose() => Reset();

        private static void Reset()
        {
            WorldEvents.Clear();
            GamemodeState.End();
        }

        [Fact]
        public void NothingIsPausedByDefault()
        {
            Assert.False(WorldEvents.IsPaused);
        }

        [Fact]
        public void StaysPausedUntilEveryOwnerResumes()
        {
            WorldEvents.Pause("a");
            WorldEvents.Pause("b");

            WorldEvents.Resume("a");
            Assert.True(WorldEvents.IsPaused);

            WorldEvents.Resume("b");
            Assert.False(WorldEvents.IsPaused);
        }

        [Fact]
        public void PausingTwiceNeedsOneResume()
        {
            WorldEvents.Pause("a");
            WorldEvents.Pause("a");

            WorldEvents.Resume("a");

            Assert.False(WorldEvents.IsPaused);
        }

        [Fact]
        public void EmptyOwnerIsIgnored()
        {
            WorldEvents.Pause(string.Empty);

            Assert.False(WorldEvents.IsPaused);
        }

        [Fact]
        public void NothingBlocksBonusesByDefault()
        {
            Assert.False(GamemodeState.BlocksBonuses);
        }

        [Fact]
        public void AModeWithoutPauseStillBlocksBonuses()
        {
            GamemodeState.Begin();

            Assert.True(GamemodeState.BlocksBonuses);
            Assert.False(WorldEvents.IsPaused);

            GamemodeState.End();

            Assert.False(GamemodeState.BlocksBonuses);
        }

        [Fact]
        public void APauseWithoutModeBlocksBonuses()
        {
            WorldEvents.Pause("a");

            Assert.True(GamemodeState.BlocksBonuses);
            Assert.False(GamemodeState.IsActive);
        }
    }
}
