using AugatonLib.Bus;
using AugatonLib.Arcade;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;

namespace ScpLevels
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public static Plugin Instance { get; private set; }

        public LevelService Service { get; private set; }

        public KillLadder<ScpLevel> Ladder { get; } = new KillLadder<ScpLevel>(level => level.Kills);

        public override string Name => "ScpLevels";

        public override string Description => "Les SCP montent de niveau a chaque serie de kills.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new LevelService(this);

            PlayerEvents.Death += Service.OnDeath;
            PlayerEvents.ChangedRole += Service.OnChangedRole;
            PlayerEvents.Left += Service.OnLeft;
            PlayerDeparture.Departing += Service.OnDeparting;
            PluginBus.Subscribe(Name, BusTopics.PlayerReplaced, Service.OnPlayerReplaced);
            ServerEvents.RoundStarted += Service.Start;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += Service.Stop;

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            PlayerEvents.Death -= Service.OnDeath;
            PlayerEvents.ChangedRole -= Service.OnChangedRole;
            PlayerEvents.Left -= Service.OnLeft;
            PlayerDeparture.Departing -= Service.OnDeparting;
            PluginBus.UnsubscribeOwner(Name);
            ServerEvents.RoundStarted -= Service.Start;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= Service.Stop;

            Service.Stop();
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig() => Ladder.SetSteps(Config.Levels);

        private void OnRoundEnded(RoundEndedEventArgs ev) => Service.Stop();
    }
}
