using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Hints;
using AugatonLib.World;
using Hazards;
using LabApi.Features.Wrappers;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;

namespace AugatonLib.Runtime
{
    public static class PluginDirectory
    {
        private static readonly Dictionary<string, PluginRecord> Entries =
            new Dictionary<string, PluginRecord>(StringComparer.OrdinalIgnoreCase);

        private static bool hooked;

        public static int Count => Entries.Count;

        public static IEnumerable<PluginRecord> All => Entries.Values;

        public static Version LibraryVersion => typeof(PluginDirectory).Assembly.GetName().Version;

        public static void Register(Plugin plugin, params Capability[] capabilities)
        {
            if (plugin is null)
                return;

            string assemblyName = AssemblyNameOf(plugin);

            Entries[assemblyName] = new PluginRecord(plugin.Name, assemblyName, plugin.Version, capabilities);

            Hook();
        }

        public static void Unregister(Plugin plugin)
        {
            SelfCheck.UnregisterOwner(plugin?.Name);

            if (plugin is not null)
                Entries.Remove(AssemblyNameOf(plugin));
        }

        public static PluginRecord Find(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
                return null;

            return Entries.TryGetValue(assemblyName, out PluginRecord record) ? record : null;
        }

        public static bool IsRegistered(string assemblyName) => Find(assemblyName) is not null;

        public static bool Declares(string assemblyName, Capability capability)
        {
            PluginRecord record = Find(assemblyName);

            return record is not null && record.Declares(capability);
        }

        public static Version LoadedVersion(string assemblyName)
        {
            PluginRecord record = Find(assemblyName);

            return record is not null ? record.Version : IntegrationProbe.VersionOf(assemblyName);
        }

        public static bool IsLoaded(string assemblyName)
        {
            return IsRegistered(assemblyName) || IntegrationProbe.IsLoaded(assemblyName);
        }

        public static void PrintBanner() => Banner.Print();

        private static void RegisterChecks()
        {
            const string owner = "AugatonLib";

            SelfCheck.Register(owner, "HintServiceMeow charge", () => HintChannel.ServiceAvailable ? null : "introuvable");
            SelfCheck.Register(owner, "prefab de tantrum", () => NetworkPrefabs.Find<TantrumEnvironmentalHazard>() is null ? "introuvable" : null);
            SelfCheck.Register(owner, "mots de C.A.S.S.I.E", () => Announcer.LineDatabase == null ? "non charges" : null);
        }

        private static void Hook()
        {
            if (hooked)
                return;

            hooked = true;
            RoundReset.Hook();
            PlayerDeparture.Hook();
            TeamWatcher.Hook();
            ScaleArbiter.Hook();
            StartupCheck.Hook();
            GamemodeState.Hook();
            RegisterChecks();
            ServerEvents.PluginsEnabled += Banner.Print;
        }

        private static string AssemblyNameOf(Plugin plugin) => plugin.GetType().Assembly.GetName().Name;
    }
}
