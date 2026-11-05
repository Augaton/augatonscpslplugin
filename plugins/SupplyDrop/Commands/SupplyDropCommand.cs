using AugatonLib.Commands;
using CommandSystem;

namespace SupplyDrop.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class SupplyDropCommand : StaffParentCommand
    {
        internal const string Node = "supplydrop.call";

        public SupplyDropCommand() => LoadGeneratedCommands();

        public override string Command => "supplydrop";

        public override string[] Aliases => new[] { "sd" };

        public override string Description => "Gestion des largages de ravitaillement.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new CallCommand());
            RegisterCommand(new ListCommand());
        }
    }
}
