using System;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace Contracts.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class ContractCommand : ICommand
    {
        public string Command => "contrat";

        public string[] Aliases => new[] { "contract" };

        public string Description => "Affiche votre contrat secret.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                return Run(sender, out response);
            }
            catch (Exception e)
            {
                Logger.Error($"{Command}: {e}");
                response = "Erreur interne.";
                return false;
            }
        }

        private static bool Run(ICommandSender sender, out string response)
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

            if (plugin.Cooldowns.IsOnCooldown(player, plugin.Config.CommandCooldownSeconds, out _))
            {
                response = plugin.Translation.TooFast;
                return false;
            }

            plugin.Cooldowns.Register(player);

            response = plugin.Service.Describe(player);
            return true;
        }
    }
}
