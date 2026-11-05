using System;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace SiteIncidents
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public const string Owner = "SiteIncidents";

        public override string Name => Owner;

        public override string Description => "Provoque des incidents aleatoires dans le complexe.";

        public override string Author => "Zone-Shilari";

        public static Plugin Instance { get; private set; }

        public IncidentService Service { get; private set; }

        public override void Enable()
        {
            Instance = this;
            Service = new IncidentService(this);

            CharacterClassManager.OnRoundStarted += OnRoundStarted;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += Service.Stop;

            PluginDirectory.Register(this, Capability.Light, Capability.Bus);
        }

        public override void Disable()
        {
            CharacterClassManager.OnRoundStarted -= OnRoundStarted;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= Service.Stop;

            Service.Stop();
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        // Evenement du jeu, sans la protection de LabAPI : une exception priverait les abonnes suivants.
        private void OnRoundStarted()
        {
            try
            {
                Service.Start();
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Service.Stop();
    }
}
