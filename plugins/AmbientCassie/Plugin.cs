using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace AmbientCassie
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public static Plugin Instance { get; private set; }

        public AnnouncementService Service { get; private set; }

        public override string Name => "AmbientCassie";

        public override string Description => "Annonces d'ambiance de C.A.S.S.I.E pendant le round.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new AnnouncementService(this);

            ServerEvents.RoundStarted += Service.Start;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += Service.Stop;

            PluginBus.Subscribe(Name, BusTopics.RoundMutatorStarted, _ => Service.React(AmbientPhase.Mutator));
            PluginBus.Subscribe(Name, BusTopics.ScpLevelReached, _ => Service.React(AmbientPhase.ScpLevel));

            SelfCheck.Register(Name, "mots de C.A.S.S.I.E", Service.CheckCassieWords);

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            ServerEvents.RoundStarted -= Service.Start;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= Service.Stop;

            Service.Stop();
            PluginBus.UnsubscribeOwner(Name);
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.ChaosLevelForLate = (int)AtLeast("chaos_level_for_late", Config.ChaosLevelForLate, 0f);
            Config.ReactionCooldownSeconds = AtLeast("reaction_cooldown_seconds", Config.ReactionCooldownSeconds, 30f);
            Config.FirstAnnouncementSeconds = AtLeast("first_announcement_seconds", Config.FirstAnnouncementSeconds, 0f);
            Config.MinIntervalSeconds = AtLeast("min_interval_seconds", Config.MinIntervalSeconds, 60f);
            Config.MaxIntervalSeconds = AtLeast("max_interval_seconds", Config.MaxIntervalSeconds, Config.MinIntervalSeconds);

            if (Translation.Lines is not null)
                return;

            Logger.Warn("lines est nul dans translation.yml, aucune annonce.");
            Translation.Lines = new List<AmbientLine>();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Service.Stop();
    }
}
