using AugatonLib.Commands;
using CommandSystem;

namespace Contracts.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ContractsCommand : StaffParentCommand
    {
        public ContractsCommand() => LoadGeneratedCommands();

        public override string Command => "contracts";

        public override string[] Aliases => new[] { "ctr" };

        public override string Description => "Etat des contrats secrets.";

        public override string Permission => "contracts.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("Contracts", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  contrats en cours ce round : {Plugin.Instance.Service.ActiveCount}");
                return true;
            }));
        }
    }
}
