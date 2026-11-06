using AugatonLib.Runtime;
using LabApi.Events.Handlers;

namespace ScpScaling
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public HealthScaler Scaler { get; private set; }

        public override string Name => "ScpScaling";

        public override string Description => "Adapte les PV des SCP au nombre de joueurs.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Scaler = new HealthScaler(this);

            PlayerEvents.ChangedRole += Scaler.OnChangedRole;

            PluginDirectory.Register(this);
        }

        public override void Disable()
        {
            PlayerEvents.ChangedRole -= Scaler.OnChangedRole;

            PluginDirectory.Unregister(this);

            Scaler = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.ReferencePlayers = (int)AtLeast("reference_players", Config.ReferencePlayers, 1f);
            Config.PercentPerPlayer = AtLeast("percent_per_player", Config.PercentPerPlayer, 0f);
            Config.MinFactor = AtLeast("min_factor", Config.MinFactor, 0.1f);
            Config.MaxFactor = AtLeast("max_factor", Config.MaxFactor, Config.MinFactor);
        }
    }
}
