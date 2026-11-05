// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System.Collections.Generic;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;

namespace AntiSLCrush.Patches
{
    // Un objet detruit reste parfois dans NetworkServer.spawned, et la methode d'origine plante alors
    // sur son gameObject en bloquant l'arrivee du joueur. Seuls les objets detruits sont retires : un
    // objet simplement desactive doit rester enregistre.
    [HarmonyPatch(typeof(NetworkServer), "SpawnObserversForConnection")]
    internal static class SpawnObserversForConnectionPatch
    {
        private static void Prefix()
        {
            List<uint> destroyed = null;

            foreach (KeyValuePair<uint, NetworkIdentity> entry in NetworkServer.spawned)
            {
                if (entry.Value != null)
                    continue;

                destroyed ??= ListPool<uint>.Shared.Rent();
                destroyed.Add(entry.Key);
            }

            if (destroyed is null)
                return;

            foreach (uint netId in destroyed)
                NetworkServer.spawned.Remove(netId);

            Plugin.Report($"[SpawnObservers] {destroyed.Count} objet(s) detruit(s) retire(s) de la liste reseau.");
            ListPool<uint>.Shared.Return(destroyed);
        }
    }
}
