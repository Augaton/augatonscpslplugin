using System;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

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

            PluginBus.Subscribe(Owner, BusTopics.IncidentRequested, OnIncidentRequested);

            SelfCheck.Register(Name, "mots de C.A.S.S.I.E", Service.CheckCassieWords);

            PluginDirectory.Register(this, Capability.Light, Capability.Bus);
        }

        public override void Disable()
        {
            CharacterClassManager.OnRoundStarted -= OnRoundStarted;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= Service.Stop;

            Service.Stop();
            PluginBus.UnsubscribeOwner(Owner);
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.TeslaSurgeInterval = AtLeast("tesla_surge_interval", Config.TeslaSurgeInterval, 2f);
            Config.MinEventGapSeconds = AtLeast("min_event_gap_seconds", Config.MinEventGapSeconds, 0f);
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

        // Le nom vient de la config d'un autre plugin : il peut etre faux.
        private void OnIncidentRequested(BusMessage message)
        {
            if (!Enum.TryParse(message.Text, true, out IncidentType type) || !Enum.IsDefined(typeof(IncidentType), type))
            {
                Logger.Warn($"Incident demande inconnu : {message.Text}.");
                return;
            }

            if (Round.IsRoundInProgress && !(Config.PauseDuringGamemodes && WorldEvents.IsPaused))
                Service.Trigger(type);
        }
    }
}
