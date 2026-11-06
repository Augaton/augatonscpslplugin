using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace SpectatorVote
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public const string Owner = "SpectatorVote";

        public static Plugin Instance { get; private set; }

        public VoteService Service { get; private set; }

        public override string Name => Owner;

        public override string Description => "Les spectateurs votent pour le prochain incident du site.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new VoteService(this);

            ServerEvents.RoundStarted += Service.Start;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += Service.Stop;
            PlayerEvents.Left += Service.OnLeft;

            PluginBus.Subscribe(Name, BusTopics.RoundMutatorStarted, Service.OnMutatorStarted);

            SelfCheck.Register(Name, "choix du vote disponibles", Service.CheckOptions);

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            ServerEvents.RoundStarted -= Service.Start;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= Service.Stop;
            PlayerEvents.Left -= Service.OnLeft;

            Service.Stop();
            PluginBus.UnsubscribeOwner(Name);
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.BlessingHeal = AtLeast("blessing_heal", Config.BlessingHeal, 0f);
            Config.BlessingBlindSeconds = AtLeast("blessing_blind_seconds", Config.BlessingBlindSeconds, 1f);
            Config.FirstVoteSeconds = AtLeast("first_vote_seconds", Config.FirstVoteSeconds, 0f);
            Config.VoteDurationSeconds = AtLeast("vote_duration_seconds", Config.VoteDurationSeconds, 10f);
            Config.VoteIntervalSeconds = AtLeast("vote_interval_seconds", Config.VoteIntervalSeconds, Config.VoteDurationSeconds + 30f);
            Config.VoteCommandCooldownSeconds = AtLeast("vote_command_cooldown_seconds", Config.VoteCommandCooldownSeconds, 1f);
            Config.OptionCount = (int)AtLeast("option_count", Config.OptionCount, 2f);

            if (Config.Options is not null)
                return;

            Logger.Warn("options est nul dans config.yml, aucun vote ne s'ouvrira.");
            Config.Options = new List<IncidentOption>();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Service.Stop();
    }
}
