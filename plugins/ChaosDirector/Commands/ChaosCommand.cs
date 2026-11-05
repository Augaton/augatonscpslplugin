using AugatonLib.Chaos;
using AugatonLib.Commands;
using CommandSystem;

namespace ChaosDirector.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ChaosCommand : StaffParentCommand
    {
        public ChaosCommand() => LoadGeneratedCommands();

        public override string Command => "chaos";

        public override string[] Aliases => new[] { "chaosdirector" };

        public override string Description => "Etat du niveau de chaos.";

        public override string Permission => "chaosdirector.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "ChaosDirector",
                typeof(Plugin),
                Permission,
                builder =>
                {
                    builder.AppendLine($"  niveau : {ChaosState.BonusLevel}/{Plugin.Instance.Config.MaxBonusLevel}");
                    builder.AppendLine($"  points : {Plugin.Instance.Meter.Points}");
                    return true;
                }));
        }
    }
}
