using System.Collections.Generic;
using AugatonLib.Runtime;
using LabApi.Features.Console;
using PlayerRoles;
using RemoteKeycard.Handlers;

namespace RemoteKeycard
{
    public sealed class Plugin : AugatonPlugin<Config.Config>
    {
        public override string Name => "RemoteKeycard";

        public override string Description => "Ouvre portes, generateurs, casiers SCP et panneau d'ogive avec une carte gardee dans l'inventaire.";

        public override string Author => "Zone-Shilari (base: Beryl)";

        public static Plugin Instance { get; private set; }

        private KeycardHandlers keycardHandlers;

        public override void Enable()
        {
            Instance = this;

            keycardHandlers = new KeycardHandlers(this);
            keycardHandlers.Start();

            PluginDirectory.Register(this, Capability.DoorLock);
        }

        public override void Disable()
        {
            keycardHandlers?.Stop();

            PluginDirectory.Unregister(this);

            keycardHandlers = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.IgnoredRoles is null)
            {
                Logger.Warn("IgnoredRoles est absent, remis a une liste vide.");
                Config.IgnoredRoles = new List<RoleTypeId>();
            }

            if (Config.Extras is null)
            {
                Logger.Warn("Extras est absent, reglages par defaut appliques.");
                Config.Extras = new Config.ExtraSettings();
            }
        }
    }
}
