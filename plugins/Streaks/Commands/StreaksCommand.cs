using System;
using AugatonLib.Commands;
using AugatonLib.Text;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace Streaks.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class StreaksCommand : StaffParentCommand
    {
        internal const string Node = "streaks.manage";

        public StreaksCommand() => LoadGeneratedCommands();

        public override string Command => "streaks";

        public override string[] Aliases => new[] { "streak" };

        public override string Description => "Series de kills et primes.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("Streaks", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  paliers : {Plugin.Instance.Ladder.Steps.Count}");
                return true;
            }));
            RegisterCommand(new AddKillCommand());
        }
    }

    public sealed class AddKillCommand : StaffCommand
    {
        public override string Command => "add";

        public override string Description => "Compte un kill pour un joueur, pour tester les paliers seul.";

        public override string Permission => StreaksCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Usage : streaks add <joueur>";
                return false;
            }

            if (!TryFindPlayer(arguments.At(0), out Player player, out response))
                return false;

            string name = SafeText.Nickname(player);

            Plugin.Instance.Service.RegisterKill(player);
            Audit(sender, "Streaks", $"a ajoute un kill a {name}");

            response = $"{name} : {Plugin.Instance.Ladder.KillsOf(player.UserId)} kill(s) d'affilee.";
            return true;
        }
    }
}
