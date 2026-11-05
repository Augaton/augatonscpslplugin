using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using System;

namespace RoundRecap
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        private readonly RecapCounters counters = new RecapCounters();

        public override string Name => "RoundRecap";

        public override string Description => "Affiche un bilan de fin de round.";

        public override string Author => "Zone-Shilari";

        public override void Enable()
        {
            counters.Subscribe(Name);

            ServerEvents.RoundStarted += counters.Reset;
            ServerEvents.RoundEnded += OnRoundEnded;

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            ServerEvents.RoundStarted -= counters.Reset;
            ServerEvents.RoundEnded -= OnRoundEnded;

            PluginBus.UnsubscribeOwner(Name);
            PluginDirectory.Unregister(this);
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            try
            {
                string recap = counters.Describe(Translation);

                if (!string.IsNullOrEmpty(recap))
                    Server.SendBroadcast(recap, Config.BroadcastDuration);
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundEnded: {e}");
            }
        }
    }
}
