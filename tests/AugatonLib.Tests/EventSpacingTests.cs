using AugatonLib.Runtime;
using Xunit;

namespace AugatonLib.Tests
{
    public class EventSpacingTests
    {
        public EventSpacingTests() => EventSpacing.Clear();

        [Fact]
        public void TheFirstEventAlwaysPasses()
        {
            Assert.True(EventSpacing.TryClaim(10d, 60d));
        }

        [Fact]
        public void ASecondEventWaitsForTheGap()
        {
            EventSpacing.TryClaim(10d, 60d);

            Assert.False(EventSpacing.TryClaim(30d, 60d));
            Assert.True(EventSpacing.TryClaim(70d, 60d));
        }

        [Fact]
        public void ARefusedEventDoesNotPushTheGapBack()
        {
            EventSpacing.TryClaim(10d, 60d);
            EventSpacing.TryClaim(30d, 60d);

            Assert.True(EventSpacing.TryClaim(70d, 60d));
        }

        [Fact]
        public void ClearReopensImmediately()
        {
            EventSpacing.TryClaim(10d, 60d);
            EventSpacing.Clear();

            Assert.True(EventSpacing.TryClaim(11d, 60d));
        }
    }
}
