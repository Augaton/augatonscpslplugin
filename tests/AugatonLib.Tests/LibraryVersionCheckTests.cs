using System;
using AugatonLib.Runtime;
using Xunit;

namespace AugatonLib.Tests
{
    public class LibraryVersionCheckTests
    {
        [Theory]
        [InlineData("2.1.0", "2.0.0", true)]
        [InlineData("2.1.0", "2.1.0", false)]
        [InlineData("2.1.0", "3.0.0", false)]
        public void ComparesRequiredAndLoadedVersions(string required, string loaded, bool expected)
        {
            Assert.Equal(expected, LibraryVersionCheck.IsOutdated(new Version(required), new Version(loaded)));
        }

        [Fact]
        public void MissingVersionsAreNotReported()
        {
            Assert.False(LibraryVersionCheck.IsOutdated(null, new Version(1, 0)));
            Assert.False(LibraryVersionCheck.IsOutdated(new Version(1, 0), null));
        }

        [Fact]
        public void ReadsTheVersionAPluginWasBuiltAgainst()
        {
            Assert.Equal(typeof(LibraryVersionCheck).Assembly.GetName().Version, LibraryVersionCheck.RequiredBy(typeof(LibraryVersionCheckTests).Assembly));
        }

        [Fact]
        public void AssemblyWithoutTheLibraryRequiresNothing()
        {
            Assert.Null(LibraryVersionCheck.RequiredBy(typeof(object).Assembly));
        }
    }
}
