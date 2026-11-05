using System;
using AugatonLib.Arbitration;
using AugatonLib.Chaos;
using LabApi.Features.Console;
using RoundRestarting;

namespace AugatonLib.Runtime
{
    internal static class RoundReset
    {
        private static bool hooked;

        internal static void Hook()
        {
            if (hooked)
                return;

            hooked = true;
            RoundRestart.OnRestartTriggered += OnRestartTriggered;
        }

        private static void OnRestartTriggered()
        {
            try
            {
                LightArbiter.Forget();
                ScaleArbiter.ForgetAll();
                DoorLockRegistry.Clear();
                GenocideArbiter.Clear();
                WorldEvents.Clear();
                ChaosState.Reset();
            }
            catch (Exception e)
            {
                Logger.Error($"RoundReset: {e}");
            }
        }
    }
}
