using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Caching;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace Contracts
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public static Plugin Instance { get; private set; }

        public ContractService Service { get; private set; }

        public CooldownTracker Cooldowns { get; private set; }

        public override string Name => "Contracts";

        public override string Description => "Chaque humain recoit un objectif secret, recompense s'il le remplit.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new ContractService(this);
            Cooldowns = new CooldownTracker();

            ServerEvents.RoundStarted += Service.Start;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += OnRoundRestarted;
            PlayerEvents.Death += Service.OnDeath;
            PlayerEvents.Escaped += Service.OnEscaped;
            PlayerEvents.ActivatedGenerator += Service.OnActivatedGenerator;
            PlayerEvents.UsedItem += Service.OnUsedItem;
            PluginBus.Subscribe(Name, BusTopics.PlayerReplaced, Service.OnPlayerReplaced);

            SelfCheck.Register(Name, "recompenses valides", Service.CheckRewards);

            PluginDirectory.Register(this, Capability.Hints, Capability.Bus);
        }

        public override void Disable()
        {
            ServerEvents.RoundStarted -= Service.Start;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
            PlayerEvents.Death -= Service.OnDeath;
            PlayerEvents.Escaped -= Service.OnEscaped;
            PlayerEvents.ActivatedGenerator -= Service.OnActivatedGenerator;
            PlayerEvents.UsedItem -= Service.OnUsedItem;
            PluginBus.UnsubscribeOwner(Name);

            OnRoundRestarted();
            PluginDirectory.Unregister(this);

            Service = null;
            Cooldowns = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.CommandCooldownSeconds = AtLeast("command_cooldown_seconds", Config.CommandCooldownSeconds, 1f);
            Config.FirstOfferSeconds = AtLeast("first_offer_seconds", Config.FirstOfferSeconds, 5f);

            if (Config.Offers is not null)
                return;

            Logger.Warn("offers est nul dans la config, aucun contrat.");
            Config.Offers = new List<ContractOffer>();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Service.Stop();

        private void OnRoundRestarted()
        {
            Service.Stop();
            Cooldowns.Clear();
            Hints.Clear();
        }
    }
}
