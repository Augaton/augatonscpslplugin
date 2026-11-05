using CommandSystem;
using AugatonLib.Commands;

namespace SCPReplacer.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ScpReplacerCommand : StaffParentCommand
    {
        public ScpReplacerCommand() => LoadGeneratedCommands();

        public override string Command => "scpreplacer";

        public override string[] Aliases => new[] { "scprep" };

        public override string Description => "Etat des remplacements de SCP en attente.";

        public override string Permission => "scpreplacer.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "SCPReplacer",
                typeof(Plugin), Permission, builder =>
            {
                string pending = Plugin.Instance.Service.DescribePending();
                builder.AppendLine($"  SCP en attente : {(string.IsNullOrEmpty(pending) ? "aucun" : pending)}");
                builder.AppendLine($"  fenetre de depart : {Plugin.Instance.Config.QuitCutoff:0}s, fenetre de volontariat : {Plugin.Instance.Config.ReplaceCutoff:0}s");
                builder.AppendLine($"  loterie : {Plugin.Instance.Config.LotteryPeriodSeconds:0}s");
                return true;
            }));
        }
    }
}
