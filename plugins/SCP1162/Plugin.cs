using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using SCP1162.API;
using SCP1162.Handlers;

namespace SCP1162
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public override string Name => "SCP1162";

        public override string Description => "Ajoute SCP-1162, qui echange un objet lache a proximite contre un autre.";

        public override string Author => "Zone-Shilari (base: MrAfitol)";

        public static Plugin Instance { get; private set; }

        public Scp1162Service Service { get; private set; }

        private ItemHandlers itemHandlers;
        private ServerHandlers serverHandlers;

        public override void Enable()
        {
            Instance = this;


            Service = new Scp1162Service(this);
            itemHandlers = new ItemHandlers(Service);
            serverHandlers = new ServerHandlers(this);

            PlayerEvents.DroppingItem += itemHandlers.OnDroppingItem;
            PlayerEvents.Left += itemHandlers.OnLeft;

            ServerEvents.WaitingForPlayers += serverHandlers.OnWaitingForPlayers;
            ServerEvents.RoundEnded += serverHandlers.OnRoundEnded;
            ServerEvents.RoundRestarted += serverHandlers.OnRoundRestarted;

            PluginDirectory.Register(this, Capability.Hints, Capability.Bus);
        }

        public override void Disable()
        {
            if (itemHandlers is not null)
            {
                PlayerEvents.DroppingItem -= itemHandlers.OnDroppingItem;
                PlayerEvents.Left -= itemHandlers.OnLeft;
            }

            if (serverHandlers is not null)
            {
                ServerEvents.WaitingForPlayers -= serverHandlers.OnWaitingForPlayers;
                ServerEvents.RoundEnded -= serverHandlers.OnRoundEnded;
                ServerEvents.RoundRestarted -= serverHandlers.OnRoundRestarted;
            }

            Service?.Reset();
            Hints.Clear();

            PluginDirectory.Unregister(this);

            serverHandlers = null;
            itemHandlers = null;
            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig() => Config.Validate();
    }
}
