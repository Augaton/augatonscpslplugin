using AugatonLib.Text;
using Xunit;

namespace AugatonLib.Tests
{
    public class SafeTextTests
    {
        [Theory]
        [InlineData("<color=red>Bob</color>", "color=redBob/color")]
        [InlineData("Bob​", "Bob")]
        [InlineData("Bo\u0007b", "Bob")]
        [InlineData("  Bob  ", "Bob")]
        [InlineData("Bo\\b", "Bob")]
        public void RemovesDangerousCharacters(string input, string expected)
        {
            Assert.Equal(expected, SafeText.Sanitize(input));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("<>")]
        [InlineData("​​")]
        public void FallsBackWhenNothingRemains(string input)
        {
            Assert.Equal("repli", SafeText.Sanitize(input, fallback: "repli"));
        }

        [Fact]
        public void KeepsEmoji()
        {
            Assert.Equal("Bob \U0001F600", SafeText.Sanitize("Bob \U0001F600"));
        }

        [Fact]
        public void TruncatesToMaxLength()
        {
            Assert.Equal("abcd", SafeText.Sanitize("abcdef", 4));
        }

        [Fact]
        public void NeverCutsASurrogatePairInHalf()
        {
            string result = SafeText.Sanitize("abc\U0001F600", 4);

            Assert.Equal("abc", result);
        }

        [Fact]
        public void RejectsLoneSurrogates()
        {
            Assert.Equal("ab", SafeText.Sanitize("a\uD83Db"));
        }

        [Fact]
        public void NonPositiveMaxLengthGivesFallback()
        {
            Assert.Equal("repli", SafeText.Sanitize("Bob", 0, "repli"));
        }
    }
}
