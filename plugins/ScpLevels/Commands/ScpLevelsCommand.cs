using System;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace ScpLevels.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ScpLevelsCommand : StaffParentCommand
    {
        internal const string Node = "scplevels.manage";

        public ScpLevelsCommand() => LoadGeneratedCommands();

        public override string Command => "scplevels";

        public override string[] Aliases => new[] { "levels" };

        public override string Description => "Niveaux des SCP.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("ScpLevels", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  niveaux configures : {Plugin.Instance.Ladder.Steps.Count}");
                builder.AppendLine($"  SCP ayant pris un niveau : {Plugin.Instance.Service.LeveledCount}");
                return true;
            }));
            RegisterCommand(new AddKillCommand());
        }
    }

    public sealed class AddKillCommand : StaffCommand
    {
        public override string Command => "add";

        public override string Description => "Compte un kill pour un SCP, pour tester les niveaux seul.";

        public override string Permission => ScpLevelsCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Usage : scplevels add <joueur>";
                return false;
            }

            if (!TryFindPlayer(arguments.At(0), out Player player, out response))
                return false;

            if (!player.IsSCP)
            {
                response = $"{player.Nickname} n'est pas un SCP.";
                return false;
            }

            Plugin.Instance.Service.AddKill(player);
            Audit(sender, "ScpLevels", $"a ajoute un kill a {player.Nickname}");

            response = $"{player.Nickname} : {Plugin.Instance.Ladder.KillsOf(player.UserId)} kill(s), niveau {Plugin.Instance.Ladder.LevelOf(player.UserId)}.";
            return true;
        }
    }
}
