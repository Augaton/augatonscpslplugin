using System;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace ArcadeShop.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class ShopCommand : ICommand
    {
        public string Command => "shop";

        public string[] Aliases => new[] { "boutique" };

        public string Description => "Affiche la boutique ou achete un objet.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                return Run(arguments, sender, out response);
            }
            catch (Exception e)
            {
                Logger.Error($"{Command}: {e}");
                response = "Erreur interne.";
                return false;
            }
        }

        private static bool Run(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null)
            {
                response = "Plugin indisponible.";
                return false;
            }

            if (Player.Get(sender) is not Player player)
            {
                response = plugin.Translation.PlayerOnly;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = plugin.Service.Describe(player);
                return true;
            }

            return plugin.Service.TryBuy(player, arguments.At(0), out response);
        }
    }
}
