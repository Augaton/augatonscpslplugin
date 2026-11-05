using CommandSystem;
using AugatonLib.Commands;

namespace RealisticSizes.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class SizesCommand : StaffParentCommand
    {
        public SizesCommand() => LoadGeneratedCommands();

        public override string Command => "sizes";

        public override string[] Aliases => new[] { "rsize" };

        public override string Description => "Etat et remise a zero des tailles de joueurs.";

        public override string Permission => "realisticsizes.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "RealisticSizes",
                typeof(Plugin), Permission, builder =>
            {
                Config config = Plugin.Instance.Config;
                builder.AppendLine($"  mode : {config.ActiveMode}");
                builder.AppendLine($"  SCP affectes : {(config.AffectScps ? "oui" : "non")}");
                builder.AppendLine($"  proportionne : {(config.AllowUnproportionalValues ? "non" : "oui")}");
                return true;
            }));

            RegisterCommand(new ResetSizeCommand());
        }
    }
}
