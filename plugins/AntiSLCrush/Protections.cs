// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using AntiSLCrush.Patches;
using HarmonyLib;
using LabApi.Features.Console;

namespace AntiSLCrush
{
    // Chaque protection est appliquee a part : si une mise a jour du jeu en casse une, les autres restent
    // actives, alors qu'un PatchAll s'arreterait a la premiere erreur.
    internal static class Protections
    {
        private static readonly Harmony Patcher = new Harmony(Plugin.PluginName);
        private static readonly List<string> Failed = new List<string>(2);
        private static readonly Dictionary<string, int> Triggers = new Dictionary<string, int>(StringComparer.Ordinal);

        public static IReadOnlyList<string> Failures => Failed;

        public static void Apply(Config config)
        {
            Failed.Clear();

            Patch(config.ProtectTransportData, typeof(OnTransportDataPatch));
            Patch(config.ProtectPlayerJoin, typeof(SpawnObserversForConnectionPatch));
            Patch(config.ProtectReserveAmmo, typeof(ReserveAmmoSyncUpdateDeltaPatch));
            Patch(config.ProtectScp330Bag, typeof(Scp330BagOnAddedPatch));
            Patch(config.ProtectAdsHolster, typeof(LinearAdsOnHolsteredPatch));
        }

        public static void Remove() => Patcher.UnpatchAll(Plugin.PluginName);

        // Le premier declenchement remonte en clair : un serveur doit savoir qu'un bug du jeu a ete evite.
        public static void Record(string protection, int amount, string detail)
        {
            Triggers.TryGetValue(protection, out int count);
            Triggers[protection] = count + amount;

            if (count == 0)
                Logger.Warn($"{protection} : {detail}");
            else
                Plugin.Report($"{protection} : {detail}");
        }

        public static string Describe()
        {
            StringBuilder builder = new StringBuilder(128);

            builder.AppendLine(Triggers.Count == 0 ? "  aucun declenchement" : "  declenchements :");

            foreach (KeyValuePair<string, int> entry in Triggers)
                builder.AppendLine($"    {entry.Key} : {entry.Value}");

            if (Failed.Count > 0)
                builder.AppendLine($"  protections en echec : {string.Join(", ", Failed)}");

            return builder.ToString();
        }

        private static void Patch(bool enabled, Type patch)
        {
            if (!enabled)
                return;

            try
            {
                RuntimeHelpers.RunClassConstructor(patch.TypeHandle);
                Patcher.CreateClassProcessor(patch).Patch();
            }
            catch (Exception e)
            {
                Failed.Add(patch.Name);
                Logger.Error($"Protection {patch.Name} desactivee, le jeu a peut-etre change : {e.Message}");
            }
        }
    }
}
