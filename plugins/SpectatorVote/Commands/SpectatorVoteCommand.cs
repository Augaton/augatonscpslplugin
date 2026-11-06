using System;
using AugatonLib.Commands;
using CommandSystem;

namespace SpectatorVote.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class SpectatorVoteCommand : StaffParentCommand
    {
        internal const string Node = "spectatorvote.manage";

        public SpectatorVoteCommand() => LoadGeneratedCommands();

        public override string Command => "spectatorvote";

        public override string[] Aliases => new[] { "svote" };

        public override string Description => "Vote des spectateurs pour le prochain incident.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("SpectatorVote", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  vote : {(Plugin.Instance.Service.IsOpen ? "ouvert" : "ferme")}");
                return true;
            }));
            RegisterCommand(new OpenCommand());
        }
    }

    public sealed class OpenCommand : StaffCommand
    {
        public override string Command => "open";

        public override string Description => "Ouvre un vote tout de suite, meme avec peu de spectateurs.";

        public override string Permission => SpectatorVoteCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string problem = Plugin.Instance.Service.TryOpen(true);

            if (problem is not null)
            {
                response = problem;
                return false;
            }

            Audit(sender, "SpectatorVote", "a ouvert un vote");

            response = "Vote ouvert.";
            return true;
        }
    }
}
