using System;
using CommandSystem;
using AugatonLib.Commands;
using SCP1162.API;

namespace SCP1162.Commands
{
    public sealed class SpawnCommand : StaffCommand
    {
        public override string Command => "spawn";

        public override string[] Aliases => new[] { "place" };

        public override string Description => "Fait apparaitre SCP-1162.";

        public override string Permission => "scp1162.manage";

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Scp1162Service service = Plugin.Instance?.Service;

            if (service is null)
            {
                response = "Plugin non initialise.";
                return false;
            }

            if (service.IsSpawned)
            {
                response = Plugin.Instance.Translation.AlreadySpawned;
                return false;
            }

            int spawned = service.Spawn();

            if (spawned == 0)
            {
                response = Plugin.Instance.Translation.NotSpawned;
                return false;
            }

            Audit(sender, "SCP1162", $"a fait apparaitre SCP-1162 ({spawned} emplacement(s))");
            response = Plugin.Instance.Translation.Spawned.Replace("{count}", spawned.ToString());

            return true;
        }
    }
}
