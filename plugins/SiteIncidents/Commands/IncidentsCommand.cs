using System;
using AugatonLib.Commands;
using CommandSystem;

namespace SiteIncidents.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class IncidentsCommand : StaffParentCommand
    {
        internal const string Node = "siteincidents.manage";

        public IncidentsCommand() => LoadGeneratedCommands();

        public override string Command => "incidents";

        public override string[] Aliases => new[] { "inc" };

        public override string Description => "Gestion des incidents de site.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("SiteIncidents", typeof(Plugin), Permission));
            RegisterCommand(new TriggerCommand());
        }
    }

    public sealed class TriggerCommand : StaffCommand
    {
        public override string Command => "trigger";

        public override string[] Aliases => new[] { "t" };

        public override string Description => "Declenche un incident : Blackout, Alert, Lockdown, GasLeak, TeslaSurge ou CameraOutage.";

        public override string Permission => IncidentsCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1 || !Enum.TryParse(arguments.At(0), true, out IncidentType type) || !Enum.IsDefined(typeof(IncidentType), type))
            {
                response = "Usage : incidents trigger <Blackout|Alert|Lockdown|GasLeak|TeslaSurge|CameraOutage>";
                return false;
            }

            if (!Plugin.Instance.Service.Trigger(type))
            {
                response = "Incident impossible : zone perdue, aucune porte Tesla, ou SCP-079 absent.";
                return false;
            }

            Audit(sender, "SiteIncidents", $"a declenche l'incident {type}");

            response = $"Incident {type} declenche.";
            return true;
        }
    }
}
