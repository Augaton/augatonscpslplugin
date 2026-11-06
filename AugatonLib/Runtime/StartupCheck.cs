using System;
using System.Collections.Generic;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using MEC;

namespace AugatonLib.Runtime
{
    internal static class StartupCheck
    {
        // Les machines et les anomalies se posent quelques secondes apres le debut du round.
        private const float DelaySeconds = 20f;

        private static bool hooked;
        private static bool scheduled;

        internal static void Hook()
        {
            if (hooked)
                return;

            hooked = true;
            ServerEvents.RoundStarted += OnRoundStarted;
        }

        private static void OnRoundStarted()
        {
            if (scheduled)
                return;

            scheduled = true;
            Timing.CallDelayed(DelaySeconds, Report);
        }

        private static void Report()
        {
            try
            {
                List<string> failures = SelfCheck.Failures();

                foreach (string failure in failures)
                    Logger.Warn($"[collection check] {failure}");

                Logger.Info($"[collection check] {failures.Count} probleme(s). Detail : <plugin> collection check.");
            }
            catch (Exception e)
            {
                Logger.Error($"StartupCheck: {e}");
            }
        }
    }
}
