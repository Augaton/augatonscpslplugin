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
        private static ReferenceHub lastOwner;

        // Seul un second appel pour le meme joueur est suspect : deux joueurs differents peuvent
        // ramasser un sac dans la meme frame.
        private static bool Prefix(Scp330Bag __instance)
        {
            ReferenceHub owner = __instance.Owner;

            if (lastCalledTime == Time.time && lastOwner == owner)
            {
                Plugin.Report($"[Scp330Bag.OnAdded] Appel trop rapide pour {owner?.nicknameSync?.MyNick ?? "?"}, ignore.");
                return false;
            }

            lastCalledTime = Time.time;
            lastOwner = owner;
            return true;
        }
    }
}
