using System;
using System.Collections.Generic;
using System.Reflection;
using LabApi.Features.Console;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// LabAPI ne compare pas les versions des dependances : un plugin recent avec une AugatonLib
    /// ancienne ne se plaindrait qu'au premier appel manquant, par une MissingMethodException.
    /// </summary>
    public static class LibraryVersionCheck
    {
        private const string LibraryName = "AugatonLib";

        private static readonly List<string> Outdated = new List<string>(2);

        public static IReadOnlyList<string> OutdatedPlugins => Outdated;

        public static Version RequiredBy(Assembly assembly)
        {
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (reference.Name == LibraryName)
                    return reference.Version;
            }

            return null;
        }

        public static bool IsOutdated(Version required, Version loaded)
        {
            return required is not null && loaded is not null && loaded < required;
        }

        internal static void Verify(string pluginName, Assembly assembly)
        {
            Version required = RequiredBy(assembly);
            Version loaded = PluginDirectory.LibraryVersion;

            if (!IsOutdated(required, loaded))
                return;

            Outdated.Add(pluginName);
            Logger.Error($"{pluginName} requiert AugatonLib {required}, mais la version {loaded} est chargee. Mettez a jour AugatonLib.dll dans LabAPI/dependencies/.");
        }
    }
}
