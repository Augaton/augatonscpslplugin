using System;
using AugatonLib.Runtime;
using Xunit;

namespace AugatonLib.Tests
{
    public class WorldEventsTests : IDisposable
    {
        public WorldEventsTests() => WorldEvents.Clear();

        public void Dispose() => WorldEvents.Clear();

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
    }
}
