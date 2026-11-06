using AugatonLib.World;
using Xunit;

namespace AugatonLib.Tests
{
    public class SpentItemsTests
    {
        [Fact]
        public void AMarkedItemIsSpentUntilReleased()
        {
            SpentItems.Clear();
            SpentItems.Mark(12);

            Assert.True(SpentItems.IsSpent(12));
            Assert.False(SpentItems.IsSpent(13));

            SpentItems.Release(12);

            Assert.False(SpentItems.IsSpent(12));
        }

        [Fact]
        public void ClearForgetsEverything()
        {
            SpentItems.Mark(40);
            SpentItems.Clear();

            Assert.False(SpentItems.IsSpent(40));
        }
    }
}
