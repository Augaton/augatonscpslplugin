using AugatonLib.Commands;
using CommandSystem;

namespace ScpScaling.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ScpScalingCommand : StaffParentCommand
    {
        public ScpScalingCommand() => LoadGeneratedCommands();

        public override string Command => "scpscaling";

        public override string[] Aliases => new[] { "scaling" };

        public override string Description => "PV des SCP selon le nombre de joueurs.";

        public override string Permission => "scpscaling.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("ScpScaling", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  facteur actuel : {Plugin.Instance.Scaler.CurrentFactor:0.00}");
                return true;
            }));
        }
    }
}
