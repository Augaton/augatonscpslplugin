using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;

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

            CharacterClassManager.OnRoundStarted += Service.Start;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += Service.Stop;

            PluginDirectory.Register(this, Capability.Light, Capability.Bus);
        }

        public override void Disable()
        {
            CharacterClassManager.OnRoundStarted -= Service.Start;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= Service.Stop;

            Service.Stop();
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Service.Stop();
    }
}
