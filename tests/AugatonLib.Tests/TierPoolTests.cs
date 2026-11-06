using System.Collections.Generic;
using AugatonLib.Random;
using Xunit;

namespace AugatonLib.Tests
{
    public class TierPoolTests
    {
        private static readonly int[] Entries = { 0, 0, 2, 2, 2 };

        [Fact]
        public void KeepsOnlyTheRequestedTier()
        {
            List<int> pool = TierPool.Closest(Entries, entry => entry, 2);

            Assert.Equal(3, pool.Count);
            Assert.All(pool, entry => Assert.Equal(2, entry));
        }

        [Fact]
        public void FallsBackToTheClosestLowerTier()
        {
            Assert.Equal(new[] { 0, 0 }, TierPool.Closest(Entries, entry => entry, 1));
            Assert.Equal(3, TierPool.Closest(Entries, entry => entry, 9).Count);
        }

        [Fact]
        public void NothingBelowTheLowestTier()
        {
            Assert.Empty(TierPool.Closest(new[] { 3 }, entry => entry, 1));
        }
    }
}
