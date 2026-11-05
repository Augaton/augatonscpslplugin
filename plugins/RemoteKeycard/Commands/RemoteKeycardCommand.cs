using CommandSystem;
using AugatonLib.Commands;

namespace RemoteKeycard.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class RemoteKeycardCommand : StaffParentCommand
    {
        public RemoteKeycardCommand() => LoadGeneratedCommands();

        public override string Command => "remotekeycard";

        public override string[] Aliases => new[] { "rk" };

        public override string Description => "Etat de l'ouverture a distance par carte.";

        public override string Permission => "remotekeycard.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "RemoteKeycard",
                typeof(Plugin), Permission, builder =>
            {
                Config.Config config = Plugin.Instance.Config;
                builder.AppendLine($"  portes {Yes(config.AffectDoors)}, generateurs {Yes(config.AffectGenerators)}, ogive {Yes(config.AffectWarheadPanel)}, casiers {Yes(config.AffectScpLockers)}");
                builder.AppendLine($"  amnesie bloquante : {Yes(config.AmnesiaMatters)}");
                builder.AppendLine($"  carte en main requise : {Yes(config.RequireHeldKeycard)}");
                builder.AppendLine($"  roles ignores : {config.IgnoredRoles.Count}");
                return true;
            }));
        }

        private static string Yes(bool value) => value ? "oui" : "non";
    }
}
