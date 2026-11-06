using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace TickGuard
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public const string Owner = "TickGuard";

        public static Plugin Instance { get; private set; }

        public LoadWatch Watch { get; private set; }

        public override string Name => Owner;

        public override string Description => "Surveille les TPS et suspend les evenements automatiques si le serveur souffre.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Watch = new LoadWatch(this);

            ServerEvents.RoundStarted += Watch.Start;
            ServerEvents.RoundRestarted += Watch.Stop;

            PluginDirectory.Register(this);
        }

        public override void Disable()
        {
            ServerEvents.RoundStarted -= Watch.Start;
            ServerEvents.RoundRestarted -= Watch.Stop;

            Watch.Stop();
            PluginDirectory.Unregister(this);

            Watch = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.CheckSeconds = AtLeast("check_seconds", Config.CheckSeconds, 1f);
            Config.ChecksBeforeAlert = (int)AtLeast("checks_before_alert", Config.ChecksBeforeAlert, 1f);

            if (!(Config.RecoveryTps > Config.MinTps))
            {
                Logger.Warn("recovery_tps doit depasser min_tps : valeur alignee.");
                Config.RecoveryTps = Config.MinTps + 5f;
            }
        }
    }
}
