using System;
using AugatonLib.Arbitration;
using LabApi.Features.Enums;
using Xunit;

namespace AugatonLib.Tests
{
    public class DoorLockRegistryTests : IDisposable
    {
        public DoorLockRegistryTests() => DoorLockRegistry.Clear();

        public void Dispose() => DoorLockRegistry.Clear();

        [Fact]
        public void LocksOnlyTheListedDoors()
        {
            DoorLockRegistry.Lock("mode", DoorName.SurfaceGate);

            Assert.True(DoorLockRegistry.IsLocked(DoorName.SurfaceGate));
            Assert.False(DoorLockRegistry.IsLocked(DoorName.SurfaceNuke));
        }

        [Fact]
        public void LockAllCoversEveryDoor()
        {
            DoorLockRegistry.LockAll("mode");

            Assert.True(DoorLockRegistry.IsLocked(DoorName.SurfaceGate, out string owner));
            Assert.Equal("mode", owner);
        }

        [Fact]
        public void RelockingReplacesThePreviousClaim()
        {
            DoorLockRegistry.Lock("mode", DoorName.SurfaceGate);
            DoorLockRegistry.Lock("mode", DoorName.SurfaceNuke);

            Assert.Equal(1, DoorLockRegistry.Count);
            Assert.False(DoorLockRegistry.IsLocked(DoorName.SurfaceGate));
        }

        [Fact]
        public void UnlockReleasesOnlyThatOwner()
        {
            DoorLockRegistry.Lock("a", DoorName.SurfaceGate);
            DoorLockRegistry.Lock("b", DoorName.SurfaceGate);

            DoorLockRegistry.Unlock("a");

            Assert.True(DoorLockRegistry.IsLocked(DoorName.SurfaceGate, out string owner));
            Assert.Equal("b", owner);
        }

        [Fact]
        public void EmptyOwnerIsIgnored()
        {
            DoorLockRegistry.LockAll(string.Empty);

            Assert.Equal(0, DoorLockRegistry.Count);
        }

        [Fact]
        public void DescribeNamesEachClaim()
        {
            Assert.Equal("aucun verrou", DoorLockRegistry.Describe());

            DoorLockRegistry.LockAll("a");
            DoorLockRegistry.Lock("b", DoorName.SurfaceGate);

            Assert.Equal("a (toutes), b (1)", DoorLockRegistry.Describe());
        }
    }
}
