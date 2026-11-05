using System;
using AugatonLib.Commands;
using CommandSystem;

namespace AmbientCassie.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class AmbientCommand : StaffParentCommand
    {
        internal const string Node = "ambientcassie.manage";

        public AmbientCommand() => LoadGeneratedCommands();

        public override string Command => "ambient";

        public override string[] Aliases => new[] { "amb" };

        public override string Description => "Annonces d'ambiance de C.A.S.S.I.E.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("AmbientCassie", typeof(Plugin), Permission));
            RegisterCommand(new PlayCommand());
        }
    }

    public sealed class PlayCommand : StaffCommand
    {
        public override string Command => "play";

        public override string[] Aliases => new[] { "p" };

        public override string Description => "Joue tout de suite une annonce d'ambiance.";

        public override string Permission => AmbientCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Service.Announce())
            {
                response = "Aucune annonce disponible pour ce moment du round.";
                return false;
            }

            Audit(sender, "AmbientCassie", "a joue une annonce d'ambiance");

            response = "Annonce jouee.";
            return true;
        }
    }
}
