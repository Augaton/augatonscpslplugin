using System;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace Replacer.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class NoReplaceCommand : ICommand
    {
        public string Command => "noreplace";

        public string[] Aliases => new[] { "nr" };

        public string Description => "Refuser ou accepter d'etre choisi comme remplacant pendant ce round.";

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
                response = plugin.Translation.PlayerOnlyMessage;
                return false;
            }

            if (plugin.Cooldowns.IsOnCooldown(player.UserId, plugin.Config.CommandCooldownSeconds, out double remaining))
            {
                response = plugin.Translation.CooldownMessage.Replace("{seconds}", remaining.ToString("0.0"));
                return false;
            }

            plugin.Cooldowns.Register(player.UserId);

            response = plugin.Queue.ToggleOptOut(player)
                ? plugin.Translation.OptedOutMessage
                : plugin.Translation.OptedInMessage;

            return true;
        }
    }
}
