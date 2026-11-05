using System;
using System.Collections.Generic;
using LabApi.Features.Console;
using MEC;

namespace BetterCoinflipsRewritten.API
{
    public static class Scheduler
    {
        private static readonly List<CoroutineHandle> Handles = new List<CoroutineHandle>(16);

        public static void Delay(float seconds, Action action)
        {
            if (action is null)
                return;

            if (seconds <= 0f)
            {
                Invoke(action);
                return;
            }

            Prune();

            Handles.Add(
                Timing.CallDelayed(
                    seconds,
                    delegate { Invoke(action); }));
        }

        public static void Clear()
        {
            foreach (CoroutineHandle handle in Handles)
                Timing.KillCoroutines(handle);

            Handles.Clear();
        }

        private static void Invoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Logger.Error(
                    "[BetterCoinflipsRewritten] Scheduled action failed: " +
                    exception);
            }
        }

        private static void Prune()
        {
            for (int i = Handles.Count - 1; i >= 0; i--)
            {
                if (Handles[i].IsRunning)
                    continue;

                Handles.RemoveAt(i);
            }
        }
    }
}
