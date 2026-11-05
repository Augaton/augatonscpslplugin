// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System.Collections.Generic;
using HarmonyLib;
using Mirror;

namespace AntiSLCrush.Patches
{
    [HarmonyPatch(typeof(NetworkServer), "OnTransportData")]
    internal static class OnTransportDataPatch
    {
        private static readonly HashSet<int> Kicked = new HashSet<int>();

        private static bool Prefix(int connectionId)
        {
            if (connectionId == 0 || NetworkServer.connections.ContainsKey(connectionId))
                return true;

            if (Kicked.Add(connectionId))
            {
                Plugin.Report($"[OnTransportData] connectionId {connectionId} inconnu, deconnexion.");
                Transport.active?.ServerDisconnect(connectionId);
            }

            return false;
        }

        internal static void Clear() => Kicked.Clear();
    }
}
