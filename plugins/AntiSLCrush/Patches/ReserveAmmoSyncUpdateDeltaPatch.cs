// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System;
using HarmonyLib;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;

namespace AntiSLCrush.Patches
{
    // La methode tourne a chaque frame : une arme dont le proprietaire est detruit la ferait planter en
    // boucle. Le code du jeu reste intact, et le nettoyage n'a lieu que si l'erreur se produit.
    [HarmonyPatch(typeof(ReserveAmmoSync), "UpdateDelta")]
    internal static class ReserveAmmoSyncUpdateDeltaPatch
    {
        private static Exception Finalizer(Exception __exception)
        {
            if (__exception is null)
                return null;

            int removed = AutosyncItem.Instances.RemoveWhere(IsOrphan);

            Plugin.Report($"[ReserveAmmoSync] {removed} arme(s) sans proprietaire retiree(s) : {__exception.Message}");
            return null;
        }

        private static bool IsOrphan(AutosyncItem item)
        {
            return item == null || (item is Firearm firearm && firearm.HasOwner && firearm.Owner == null);
        }
    }
}
