// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using HarmonyLib;
using InventorySystem.Items.Usables.Scp330;
using UnityEngine;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.OnAdded))]
    internal static class Scp330BagOnAddedPatch
    {
        private static float lastCalledTime = -1f;

        private static bool Prefix(Scp330Bag __instance)
        {
            if (lastCalledTime == Time.time)
            {
                Plugin.Report($"[Scp330Bag.OnAdded] Appel trop rapide pour {__instance.Owner?.nicknameSync?.MyNick ?? "?"}, ignore.");
                return false;
            }

            lastCalledTime = Time.time;
            return true;
        }
    }
}
