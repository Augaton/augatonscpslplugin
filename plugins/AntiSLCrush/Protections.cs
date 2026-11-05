// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
