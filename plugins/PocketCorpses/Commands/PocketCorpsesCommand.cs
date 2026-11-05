using System;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace PocketCorpses.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class PocketCorpsesCommand : StaffParentCommand
    {
        internal const string Node = "pocketcorpses.manage";

        public PocketCorpsesCommand() => LoadGeneratedCommands();

        public override string Command => "pocketcorpses";

        public override string[] Aliases => new[] { "corpses" };

        public override string Description => "Gestion des corps de la dimension de poche.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("PocketCorpses", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  corps en attente : {Plugin.Instance.Returns.PendingCount}");
                return true;
            }));
            RegisterCommand(new TestCommand());
        }
    }

    public sealed class TestCommand : StaffCommand
    {
        public override string Command => "test";

        public override string[] Aliases => new[] { "t" };

        public override string Description => "Fait tomber un corps au-dessus de vous, avec alerte et gouffre.";

        public override string Permission => PocketCorpsesCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Player.Get(sender) is not Player player)
            {
                response = "Commande reservee a un joueur en jeu.";
                return false;
            }

            if (!Plugin.Instance.Returns.RunTest(player))
            {
                response = "Test impossible : il faut un round en cours, un role vivant et un plafond au-dessus de vous.";
                return false;
            }

            Audit(sender, "PocketCorpses", "a lance un test de chute de corps");

            response = "Corps en chute au-dessus de vous.";
            return true;
        }
    }
}
