using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;

namespace ChaosDirector
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public override string Name => "ChaosDirector";

        public override string Description => "Fait monter un niveau de chaos commun a toute la collection.";

        public override string Author => "Zone-Shilari";

        public static Plugin Instance { get; private set; }

        public ChaosMeter Meter { get; private set; }

        public override void Enable()
        {
            Instance = this;
            Meter = new ChaosMeter(this);

            Meter.Subscribe(Name);

            PlayerEvents.Death += Meter.OnDeath;
            ServerEvents.RoundStarted += Meter.Reset;
            ServerEvents.RoundRestarted += Meter.Reset;

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            PlayerEvents.Death -= Meter.OnDeath;
            ServerEvents.RoundStarted -= Meter.Reset;
            ServerEvents.RoundRestarted -= Meter.Reset;

            Meter.Reset();

            PluginBus.UnsubscribeOwner(Name);
            PluginDirectory.Unregister(this);

            Meter = null;
            Instance = null;
        }
    }
}
