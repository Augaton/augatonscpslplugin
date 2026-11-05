using System;
using System.Collections.Generic;
using AugatonLib.Text;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace SCPReplacer.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class HumanCommand : ICommand
    {
        public string Command => "human";

        public string[] Aliases => new[] { "no" };

        public string Description => "Renoncer a son role de SCP pour un role humain aleatoire, en debut de round.";

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

            if (!plugin.Config.AllowHumanCommand)
            {
                response = plugin.Translation.HumanCommandDisabled;
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

            if (!player.IsSCP)
            {
                response = plugin.Translation.HumanCommandNotScp;
                return false;
            }

            if (plugin.Config.IgnoredRoles.Contains(player.Role))
            {
                response = plugin.Translation.HumanCommandIgnoredRole;
                return false;
            }

            if (Round.Duration.TotalSeconds > plugin.Config.QuitCutoff)
            {
                response = plugin.Translation.HumanCommandTooLate;
                return false;
            }

            float requiredHealth = plugin.Config.RequiredHealthPercentage / 100f * player.MaxHealth;

            if (player.Health < requiredHealth)
            {
                response = plugin.Translation.HumanCommandTooLowHealth;
                return false;
            }

            plugin.Service.TryRegisterDeparture(player, out _);

            RoleTypeId newRole = PickHumanRole(plugin.Config.HumanCommandRoles);

            player.SetRole(newRole, RoleChangeReason.LateJoin, RoleSpawnFlags.All);

            if (newRole == RoleTypeId.ClassD)
            {
                foreach (ItemType item in plugin.Config.ClassDBonusItems)
                {
                    if (item == ItemType.None)
                        continue;

                    player.AddItem(item);
                }
            }

            string fullName = RoleNames.Full(newRole);
            string roleName = $"<color={ColorOf(newRole).ToHex()}>{fullName}</color>";

            player.SendBroadcast(
                plugin.Translation.BroadcastHeader + plugin.Translation.HumanCommandSuccess.Replace("%ROLE%", roleName),
                plugin.Config.ResultBroadcastDuration);

            Logger.Info($"{player.Nickname} ({player.UserId}) a renonce a son role de SCP pour devenir {newRole}.");

            response = plugin.Translation.HumanCommandSuccess.Replace("%ROLE%", fullName);
            return true;
        }

        private static UnityEngine.Color ColorOf(RoleTypeId role)
        {
            return role.TryGetRoleTemplate(out PlayerRoleBase template) ? template.RoleColor : UnityEngine.Color.white;
        }

        private static RoleTypeId PickHumanRole(Dictionary<RoleTypeId, int> weights)
        {
            int total = 0;

            foreach (KeyValuePair<RoleTypeId, int> entry in weights)
            {
                if (entry.Value > 0)
                    total += entry.Value;
            }

            if (total <= 0)
                return RoleTypeId.ClassD;

            int roll = UnityEngine.Random.Range(0, total);

            foreach (KeyValuePair<RoleTypeId, int> entry in weights)
            {
                if (entry.Value <= 0)
                    continue;

                roll -= entry.Value;

                if (roll < 0)
                    return entry.Key;
            }

            return RoleTypeId.ClassD;
        }
    }
}
