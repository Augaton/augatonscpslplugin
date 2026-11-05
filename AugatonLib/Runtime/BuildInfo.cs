using System;
using System.Reflection;
using LabApi.Features;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// LabApiProperties.CompiledVersion est un champ evalue a l'execution : il vaut toujours la version
    /// du serveur et ne permet donc pas de detecter un ecart. La version de compilation vient du build.
    /// </summary>
    internal static class BuildInfo
    {
        public static readonly Version LabApiVersion = ReadLabApiVersion();

        private static Version ReadLabApiVersion()
        {
            foreach (AssemblyMetadataAttribute metadata in typeof(BuildInfo).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
            {
                if (metadata.Key == "LabApiVersion")
                    return new Version(metadata.Value);
            }

            return LabApiProperties.CurrentVersion;
        }
    }
}
