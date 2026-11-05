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
        public void PluginsNeverReferenceEachOther()
        {
            string pluginsRoot = Path.Combine(RepositoryRoot(), "plugins");
            string[] plugins = Directory.GetDirectories(pluginsRoot).Select(Path.GetFileName).ToArray();

            foreach (string plugin in plugins)
            {
                string folder = Path.Combine(pluginsRoot, plugin);

                foreach (string project in Directory.GetFiles(folder, "*.csproj"))
                    Assert.DoesNotContain("ProjectReference", File.ReadAllText(project));

                foreach (string source in Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories).Where(IsTrackedSource))
                {
                    string[] usings = File.ReadAllLines(source).Where(line => line.StartsWith("using ")).ToArray();

                    foreach (string other in plugins.Where(name => name != plugin))
                        Assert.DoesNotContain(usings, line => line.StartsWith($"using {other};") || line.StartsWith($"using {other}."));
                }
            }
        }

        private static bool IsTrackedSource(string path)
        {
            string separator = Path.DirectorySeparatorChar.ToString();

            return !path.Contains($"{separator}obj{separator}") && !path.Contains($"{separator}bin{separator}");
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
