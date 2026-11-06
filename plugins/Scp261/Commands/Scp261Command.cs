using AugatonLib.Commands;
using CommandSystem;

namespace Scp261.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Scp261Command : StaffParentCommand
    {
        public Scp261Command() => LoadGeneratedCommands();

        public override string Command => "scp261";

        public override string[] Aliases => new[] { "261" };

        public override string Description => "Etat de la machine distributrice.";

        public override string Permission => "scp261.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("Scp261", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  machines : {Plugin.Instance.Machines.Describe()}");
                return true;
            }));
        }
    }
}
