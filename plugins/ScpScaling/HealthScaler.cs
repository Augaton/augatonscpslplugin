using System;
using AugatonLib.Arcade;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

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

        // Le jeu pose les PV du role avant ChangedRole : ils peuvent etre lus et remplaces ici. Replacer rend
        // ensuite au remplacant les PV du joueur parti, donc un SCP repris ne repart pas a neuf.
        public void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                Player player = ev?.Player;

                if (player is null || !player.IsSCP || player.MaxHealth <= 0f)
                    return;

                if (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses)
                    return;

                if (Config.ExcludedRoles is not null && Config.ExcludedRoles.Contains(player.Role))
                    return;

                player.MaxHealth *= CurrentFactor;
                player.Health = player.MaxHealth;
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangedRole: {e}");
            }
        }
    }
}
