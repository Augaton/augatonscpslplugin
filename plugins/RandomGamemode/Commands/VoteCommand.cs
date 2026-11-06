using System;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using RandomGamemode.API;

namespace RandomGamemode.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class VoteCommand : ICommand
    {
        public string Command => "votemode";

        public string[] Aliases => new[] { "vm" };

        public string Description => "Voter pour le mode de jeu du prochain round.";

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

            Translation text = plugin.Translation;
            GamemodeVote vote = plugin.Registry.Vote;

            if (Player.Get(sender) is not Player player)
            {
                response = text.VotePlayerOnly;
                return false;
            }

            if (!vote.IsOpen)
            {
                response = text.VoteClosed;
                return false;
            }

            if (arguments.Count == 0)
            {
                if (plugin.VoteCooldowns.IsOnCooldown(player.UserId, plugin.Config.VoteCommandCooldownSeconds, out _))
                {
                    response = text.VoteTooFast;
                    return false;
                }

                plugin.VoteCooldowns.Register(player.UserId);

                response = vote.Describe();
                return true;
            }

            if (plugin.VoteCooldowns.IsOnCooldown(player.UserId, plugin.Config.VoteCommandCooldownSeconds, out _))
            {
                response = text.VoteTooFast;
                return false;
            }

            plugin.VoteCooldowns.Register(player.UserId);

            if (!int.TryParse(arguments.At(0), out int choice) || !vote.Cast(player.UserId, choice - 1))
            {
                response = text.VoteInvalid;
                return false;
            }

            response = text.VoteRegistered.Replace("%MODE%", vote.Options[choice - 1].DisplayName);
            return true;
        }
    }
}
