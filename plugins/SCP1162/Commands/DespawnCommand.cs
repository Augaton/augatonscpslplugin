using System;
using CommandSystem;
using AugatonLib.Commands;
using SCP1162.API;

namespace SCP1162.Commands
{
    public sealed class DespawnCommand : StaffCommand
    {
        public override string Command => "despawn";

        public override string[] Aliases => new[] { "remove" };

        public override string Description => "Retire SCP-1162 de la carte.";

        public override string Permission => "scp1162.manage";

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Scp1162Service service = Plugin.Instance?.Service;

            if (service is null)
            {
                response = "Plugin non initialise.";
                return false;
            }

            if (!service.IsSpawned)
            {
                response = Plugin.Instance.Translation.AlreadyDespawned;
                return false;
            }

            service.Despawn();
            Audit(sender, "SCP1162", "a retire SCP-1162");
            response = Plugin.Instance.Translation.Despawned;

            return true;
        }
    }
}
