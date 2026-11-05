using AugatonLib.Commands;
using CommandSystem;

namespace SCP500s.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Scp500Command : StaffParentCommand
    {
        public const string PermissionNode = "scp500s.give";

        public Scp500Command() => LoadGeneratedCommands();

        public override string Command => "scp500";

        public override string[] Aliases => new[] { "pills" };

        public override string Description => "Gestion des pilules SCP-500.";

        public override string Permission => PermissionNode;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(PermissionNode));

            RegisterCommand(new GiveCommand());
            RegisterCommand(new ListCommand());
        }
    }
}
