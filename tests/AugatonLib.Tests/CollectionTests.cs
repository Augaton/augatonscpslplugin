using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AugatonLib.Runtime;
using Xunit;

namespace AugatonLib.Tests
{
    public class CollectionTests
    {
        private static string RepositoryRoot([CallerFilePath] string path = null)
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..", ".."));
        }

        [Fact]
        public void EveryPluginFolderIsDeclaredInTheCatalog()
        {
            string[] folders = Directory.GetDirectories(Path.Combine(RepositoryRoot(), "plugins"))
                .Select(Path.GetFileName)
                .OrderBy(name => name)
                .ToArray();

            string[] known = KnownPlugins.All
                .Select(plugin => plugin.AssemblyName)
                .OrderBy(name => name)
                .ToArray();

            Assert.Equal(known, folders);
        }

        [Fact]
        public void FindIgnoresCase()
        {
            Assert.Equal("Replacer", KnownPlugins.Find("REPLACER").AssemblyName);
            Assert.Null(KnownPlugins.Find("inconnu"));
            Assert.Null(KnownPlugins.Find(null));
        }

        [Fact]
        public void RecordDescribesItsCapabilities()
        {
            PluginRecord record = new PluginRecord("Nom", "Assembly", null, new[] { Capability.Scale, Capability.Bus });

            Assert.True(record.Declares(Capability.Scale));
            Assert.False(record.Declares(Capability.Light));
            Assert.Equal("taille bus", record.DescribeCapabilities());
        }

        [Fact]
        public void RecordAcceptsNoCapability()
        {
            Assert.Equal(string.Empty, new PluginRecord("Nom", "Assembly", null, null).DescribeCapabilities());
        }
    }
}
