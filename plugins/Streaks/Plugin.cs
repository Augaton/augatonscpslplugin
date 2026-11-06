using AugatonLib.Bus;
using AugatonLib.Arcade;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;

namespace Streaks
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public const string Owner = "Streaks";

        public static Plugin Instance { get; private set; }

        public StreakService Service { get; private set; }

        public KillLadder<StreakReward> Ladder { get; } = new KillLadder<StreakReward>(reward => reward.Kills);

        public override string Name => Owner;

        public override string Description => "Recompense les series de kills et met une prime sur les meilleurs.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new StreakService(this);

            PlayerEvents.Death += Service.OnDeath;
            PlayerEvents.Left += Service.OnLeft;
            PlayerDeparture.Departing += Service.OnDeparting;
            PluginBus.Subscribe(Name, BusTopics.PlayerReplaced, Service.OnPlayerReplaced);
            ServerEvents.RoundRestarted += Service.Reset;

            SelfCheck.Register(Name, "objets des recompenses", Service.CheckItems);

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            PlayerEvents.Death -= Service.OnDeath;
            PlayerEvents.Left -= Service.OnLeft;
            PlayerDeparture.Departing -= Service.OnDeparting;
            PluginBus.UnsubscribeOwner(Name);
            ServerEvents.RoundRestarted -= Service.Reset;

            Service.Reset();
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig() => Ladder.SetSteps(Config.Rewards);
    }
}
