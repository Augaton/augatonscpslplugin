using System;
using AugatonLib.Arcade;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;

namespace ScpScaling
{
    public sealed class HealthScaler
    {
        private readonly Plugin plugin;

        public HealthScaler(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        public float CurrentFactor => ScpHealthScale.Factor(
            Player.Count,
            Config.ReferencePlayers,
            Config.PercentPerPlayer,
            Config.MinFactor,
            Config.MaxFactor);

        // Le jeu pose les PV du role avant ChangedRole : ils peuvent etre lus et remplaces ici. Au debut du
        // round, les roles passent avant le depart d'un mode : une frame d'attente le laisse se declarer.
        // Partout ailleurs, l'effet est immediat, pour rester avant le transfert de PV de Replacer.
        public void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                Player player = ev?.Player;

                if (player is null || !player.IsSCP)
                    return;

                if (ev.ChangeReason != RoleChangeReason.RoundStart)
                {
                    Scale(player);
                    return;
                }

                RoleTypeId role = player.Role;

                Timing.CallDelayed(0f, () => ScaleIfStillIn(player, role));
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangedRole: {e}");
            }
        }

        private void ScaleIfStillIn(Player player, RoleTypeId role)
        {
            try
            {
                if (!player.IsDestroyed && player.Role == role)
                    Scale(player);
            }
            catch (Exception e)
            {
                Logger.Error($"ScaleIfStillIn: {e}");
            }
        }

        private void Scale(Player player)
        {
            if (player.MaxHealth <= 0f || (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses))
                return;

            if (Config.ExcludedRoles is not null && Config.ExcludedRoles.Contains(player.Role))
                return;

            player.MaxHealth *= CurrentFactor;
            player.Health = player.MaxHealth;
        }
    }
}
