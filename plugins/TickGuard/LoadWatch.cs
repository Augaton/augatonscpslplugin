using System;
using System.Collections.Generic;
using AugatonLib.Runtime;
using LabApi.Features.Wrappers;
using MEC;
using Logger = LabApi.Features.Console.Logger;

namespace TickGuard
{
    public sealed class LoadWatch
    {
        private readonly Plugin plugin;
        private readonly LoadMonitor monitor = new LoadMonitor();

        private CoroutineHandle loop;
        private bool running;

        public LoadWatch(Plugin plugin) => this.plugin = plugin;

        public LoadMonitor Monitor => monitor;

        public void Start()
        {
            Stop();

            running = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public void Stop()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;
            monitor.Reset();
            WorldEvents.Resume(Plugin.Owner);
        }

        private IEnumerator<float> Loop()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(plugin.Config.CheckSeconds);

                try
                {
                    Check();
                }
                catch (Exception e)
                {
                    Logger.Error($"LoadWatch: {e}");
                }
            }
        }

        private void Check()
        {
            Config config = plugin.Config;
            float tps = (float)Server.Tps;

            if (!monitor.Report(tps, config.MinTps, config.RecoveryTps, config.ChecksBeforeAlert))
                return;

            if (!monitor.IsDegraded)
            {
                WorldEvents.Resume(Plugin.Owner);
                Logger.Info($"TPS revenus a {tps:0}.");
                return;
            }

            Logger.Warn($"Serveur en souffrance : {tps:0} TPS.");

            if (config.PauseWorldEvents)
                WorldEvents.Pause(Plugin.Owner);
        }
    }
}
