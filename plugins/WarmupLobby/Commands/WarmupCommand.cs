using AugatonLib.Commands;
using CommandSystem;

namespace WarmupLobby.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class WarmupCommand : StaffParentCommand
    {
        public WarmupCommand() => LoadGeneratedCommands();

        public override string Command => "warmup";

        public override string[] Aliases => new[] { "lobby" };

        public override string Description => "Echauffement pendant l'attente de joueurs.";

        public override string Permission => "warmuplobby.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("WarmupLobby", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  echauffement : {(Plugin.Instance.Service.IsRunning ? "en cours" : "arrete")}");
                return true;
            }));
        }
    }
}
