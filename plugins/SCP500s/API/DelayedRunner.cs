using System;
using System.Collections.Generic;
using LabApi.Features.Console;
using MEC;

namespace SCP500s.API
{
    public sealed class DelayedRunner
    {
        private readonly List<CoroutineHandle> handles = new List<CoroutineHandle>(16);

        public void Run(float delay, Action action)
        {
            if (action is null || Plugin.Instance is null)
                return;

            Prune();

            int roundId = Plugin.Instance.RoundId;

            handles.Add(Timing.CallDelayed(delay, () =>
            {
                if (Plugin.Instance is null || Plugin.Instance.RoundId != roundId)
                    return;

                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Logger.Error($"DelayedRunner: {e}");
                }
            }));
        }

        public void Clear()
        {
            foreach (CoroutineHandle handle in handles)
                Timing.KillCoroutines(handle);

            handles.Clear();
        }

        private void Prune()
        {
            for (int i = handles.Count - 1; i >= 0; i--)
            {
                if (!handles[i].IsRunning)
                    handles.RemoveAt(i);
            }
        }
    }
}
