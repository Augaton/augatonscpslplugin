using AugatonLib.Commands;
using AugatonLib.Runtime;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace TickGuard.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class TickGuardCommand : StaffParentCommand
    {
        public TickGuardCommand() => LoadGeneratedCommands();

        public override string Command => "tickguard";

        public override string[] Aliases => new[] { "tps" };

        public override string Description => "Etat de la surveillance des TPS.";

        public override string Permission => "tickguard.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("TickGuard", typeof(Plugin), Permission, builder =>
            {
                LoadMonitor monitor = Plugin.Instance.Watch.Monitor;

                builder.AppendLine($"  TPS actuels : {Server.Tps:0}");
                builder.AppendLine(monitor.Lowest == float.MaxValue
                    ? "  TPS les plus bas du round : aucune mesure"
                    : $"  TPS les plus bas du round : {monitor.Lowest:0}");
                builder.AppendLine($"  souffrance en cours : {(monitor.IsDegraded ? "oui" : "non")}, {monitor.Episodes} ce round");
                return true;
            }));
        }
    }
}
