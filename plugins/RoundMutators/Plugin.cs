using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;

namespace RoundMutators
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public const string Owner = "RoundMutators";

        private const int MaxSpeedPercent = 100;

        public static Plugin Instance { get; private set; }

        public MutatorService Service { get; private set; }

        public override string Name => Owner;

        public override string Description => "Ajoute parfois une regle legere a un round normal.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new MutatorService(this);

            ServerEvents.RoundStarted += Service.ScheduleDraw;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += Service.Stop;
            PlayerEvents.Death += Service.OnDeath;
            PlayerEvents.Hurting += Service.OnHurting;
            PlayerEvents.DroppingAmmo += Service.OnDroppingAmmo;

            PluginBus.Subscribe(Owner, BusTopics.GamemodeStarted, Service.OnGamemodeStarted);
            PluginBus.Subscribe(Owner, BusTopics.ChaosLevelChanged, Service.OnChaosLevelChanged);
            PluginBus.Subscribe(Owner, BusTopics.MutatorRequested, Service.OnMutatorRequested);

            PluginDirectory.Register(this, Capability.Scale, Capability.Light, Capability.Bus);
        }

        public override void Disable()
        {
            ServerEvents.RoundStarted -= Service.ScheduleDraw;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= Service.Stop;
            PlayerEvents.Death -= Service.OnDeath;
            PlayerEvents.Hurting -= Service.OnHurting;
            PlayerEvents.DroppingAmmo -= Service.OnDroppingAmmo;

            Service.Stop();
            PluginBus.UnsubscribeOwner(Owner);
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.ChancePercent = AtLeast("chance_percent", Config.ChancePercent, 0f);
            Config.RoundsBetween = (int)AtLeast("rounds_between", Config.RoundsBetween, 0f);
            Config.StartDelaySeconds = AtLeast("start_delay_seconds", Config.StartDelaySeconds, 3f);
            Config.GravityFactor = AtLeast("gravity_factor", Config.GravityFactor, 0.2f);
            Config.ChaosLevelForMutator = (int)AtLeast("chaos_level_for_mutator", Config.ChaosLevelForMutator, 0f);
            Config.VampireHeal = AtLeast("vampire_heal", Config.VampireHeal, 0f);
            Config.GlassCannonFactor = AtLeast("glass_cannon_factor", Config.GlassCannonFactor, 1f);
            Config.EndlessAmmoCap = System.Math.Min((int)AtLeast("endless_ammo_cap", Config.EndlessAmmoCap, 1f), 1000);
            Config.RegenerationPerSecond = AtLeast("regeneration_per_second", Config.RegenerationPerSecond, 0f);
            Config.ScarcePercent = System.Math.Min(100f, AtLeast("scarce_percent", Config.ScarcePercent, 0f));
            Config.SwiftScpPercent = System.Math.Min((int)AtLeast("swift_scp_percent", Config.SwiftScpPercent, 0f), 100);
            Config.FragileHealthPercent = AtLeast("fragile_health_percent", Config.FragileHealthPercent, 10f);
            Config.SizeFactor = AtLeast("size_factor", Config.SizeFactor, 0.5f);
            Config.SpeedPercent = System.Math.Min((int)AtLeast("speed_percent", Config.SpeedPercent, 0f), MaxSpeedPercent);
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Service.Stop();
    }
}
