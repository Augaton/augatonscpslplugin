using System;
using AugatonLib.Commands;
using AugatonLib.Text;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace ArcadeShop.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ArcadeShopCommand : StaffParentCommand
    {
        internal const string Node = "arcadeshop.manage";

        public ArcadeShopCommand() => LoadGeneratedCommands();

        public override string Command => "arcadeshop";

        public override string[] Aliases => new[] { "ashop" };

        public override string Description => "Boutique de round.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("ArcadeShop", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  objets en vente : {Plugin.Instance.Config.Items.Count}");
                return true;
            }));
            RegisterCommand(new GivePointsCommand());
        }
    }

    public sealed class GivePointsCommand : StaffCommand
    {
        private const int MaxPoints = 10000;

        public override string Command => "give";

        public override string Description => "Donne des points a un joueur.";

        public override string Permission => ArcadeShopCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2 || !int.TryParse(arguments.At(1), out int points) || points < 1 || points > MaxPoints)
            {
                response = $"Usage : arcadeshop give <joueur> <points, de 1 a {MaxPoints}>";
                return false;
            }

            if (!TryFindPlayer(arguments.At(0), out Player player, out response))
                return false;

            int gained = Plugin.Instance.Wallet.Earn(player.UserId, points);
            string name = SafeText.Nickname(player);

            Audit(sender, "ArcadeShop", $"a donne {gained} points a {name}");

            response = $"{name} a maintenant {Plugin.Instance.Wallet.BalanceOf(player.UserId)} points.";
            return true;
        }
    }
}
