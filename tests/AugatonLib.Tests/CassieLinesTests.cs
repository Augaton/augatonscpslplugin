using AugatonLib.Text;
using Xunit;

namespace AugatonLib.Tests
{
    public class CassieLinesTests
    {
        [Theory]
        [InlineData("attention", true)]
        [InlineData("danger", true)]
        [InlineData(".", false)]
        [InlineData(",", false)]
        [InlineData(".g4", false)]
        [InlineData("079", false)]
        [InlineData("pitch_0.8", false)]
        [InlineData("jam_02", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void OnlyRealWordsAreChecked(string word, bool expected)
        {
            Assert.Equal(expected, CassieLines.IsSpoken(word));
        }
    }
}
