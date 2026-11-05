// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using Mirror;

namespace AntiSLCrush.Patches
{
    // Le serveur gardait _userInput a vrai apres avoir rengaine en visant, et le renvoyait aux clients.
    // Le remettre a faux avant l'original donne le meme resultat que l'ancien transpiler, sans dependre
    // des instructions exactes de la methode.
    [HarmonyPatch(typeof(LinearAdsModule), "OnHolstered")]
    internal static class LinearAdsOnHolsteredPatch
    {
        private static readonly AccessTools.FieldRef<LinearAdsModule, bool> UserInput =
            AccessTools.FieldRefAccess<LinearAdsModule, bool>("_userInput");

        private static void Prefix(LinearAdsModule __instance)
        {
            if (NetworkServer.active)
                UserInput(__instance) = false;
        }
    }
}
