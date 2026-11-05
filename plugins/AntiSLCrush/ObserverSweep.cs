// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System;
using System.Collections.Generic;
using LabApi.Features.Console;
using MEC;
using Mirror;

namespace AntiSLCrush
{
    // Le jeu ignore deja les entrees nulles de connection.observing, mais les signale a chaque tick et
    // pour chaque joueur. Un nettoyage periodique evite ce spam sans patcher une methode aussi frequente.
    internal static class ObserverSweep
    {
        private const float SweepSeconds = 5f;

        private static readonly Predicate<NetworkIdentity> IsDestroyed = identity => identity == null;

        private static CoroutineHandle loop;
        private static bool running;

        public static void Start()
        {
            Stop();

            running = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public static void Stop()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;
        }

        private static IEnumerator<float> Loop()
        {
            while (running)
            {
                yield return Timing.WaitForSeconds(SweepSeconds);

                try
                {
                    Sweep();
                }
                catch (Exception e)
                {
                    Logger.Error($"ObserverSweep: {e}");
                }
            }
        }

        private static void Sweep()
        {
            int removed = 0;

            foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
            {
                if (connection is not null)
                    removed += connection.observing.RemoveWhere(IsDestroyed);
            }

            if (removed > 0)
                Plugin.Report($"[ObserverSweep] {removed} entree(s) nulle(s) retiree(s).");
        }
    }
}
