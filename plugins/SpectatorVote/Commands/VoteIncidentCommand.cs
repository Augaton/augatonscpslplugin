using System;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace SpectatorVote.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class VoteIncidentCommand : ICommand
    {
        public string Command => "voteincident";

        public string[] Aliases => new[] { "vi" };

        public string Description => "Voter pour le prochain incident, reserve aux spectateurs.";

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
            VoteService service = plugin.Service;

            if (Player.Get(sender) is not Player player)
            {
                response = text.PlayerOnly;
                return false;
            }

            if (!service.IsOpen)
            {
                response = text.VoteClosed;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = service.DescribeOptions();
                return true;
            }

            if (player.Role != RoleTypeId.Spectator)
            {
                response = text.SpectatorsOnly;
                return false;
            }

            if (service.Cooldowns.IsOnCooldown(player.UserId, plugin.Config.VoteCommandCooldownSeconds, out _))
            {
                response = text.VoteTooFast;
                return false;
            }

            service.Cooldowns.Register(player.UserId);

            if (!int.TryParse(arguments.At(0), out int choice) || !service.Cast(player, choice, out string label))
            {
                response = text.VoteInvalid;
                return false;
            }

            response = text.VoteRegistered.Replace("%LABEL%", label);
            return true;
        }
    }
}
