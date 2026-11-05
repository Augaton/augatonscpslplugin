using System;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SCPReplacer.API;

namespace SCPReplacer.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class VolunteerCommand : ICommand
    {
        public string Command => "volunteer";

        public string[] Aliases => new[] { "v" };

        public string Description => "Se porter volontaire pour remplacer un SCP parti en debut de round.";

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

        private bool Run(ArraySegment<string> arguments, ICommandSender sender, out string response)
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
                response = plugin.Translation.CooldownMessage.Replace("%SECONDS%", remaining.ToString("0.0"));
                return false;
            }

            plugin.Cooldowns.Register(player.UserId);

            if (plugin.Service.Pending.Count == 0)
            {
                response = plugin.Translation.NoEligibleScpsError;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = plugin.Translation.AvailableScpsMessage + plugin.Service.DescribePending();
                return true;
            }

            if (arguments.Count != 1)
            {
                response = plugin.Translation.WrongUsageMessage;
                return false;
            }

            string rawArgument = arguments.At(0);

            if (!rawArgument.IsValidScpArgument())
            {
                response = plugin.Translation.WrongUsageMessage;
                return false;
            }

            if (plugin.Service.HasReplacementCutoffPassed())
            {
                response = plugin.Translation.TooLateMessage;
                return false;
            }

            if (player.IsSCP && player.Role != RoleTypeId.Scp0492)
            {
                response = plugin.Translation.ScpCannotVolunteerMessage;
                return false;
            }

            if (!plugin.Config.AllowAliveVolunteers && player.Role != RoleTypeId.Spectator)
            {
                response = plugin.Translation.SpectatorOnlyMessage;
                return false;
            }

            string requestedScp = rawArgument.ScpNumber();
            ScpToReplace entry = plugin.Service.Find(requestedScp);

            if (entry is null)
            {
                response = plugin.Translation.InvalidScpError + plugin.Service.DescribePending();
                return false;
            }

            if (!plugin.Service.TryVolunteer(player, entry, out string error))
            {
                response = error;
                return false;
            }

            player.SendBroadcast(
                plugin.Translation.BroadcastHeader +
                plugin.Translation.EnteredLotteryBroadcast.Replace("%NUMBER%", requestedScp),
                plugin.Config.ResultBroadcastDuration,
                Broadcast.BroadcastFlags.Normal,
                true);

            response = plugin.Translation.EnteredLotteryMessage.Replace("%NUMBER%", requestedScp);
            return true;
        }
    }
}
