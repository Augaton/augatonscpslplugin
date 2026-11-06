using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;

namespace AugatonLib.World
{
    public static class PocketKills
    {
        // Une mort par decomposition dans la dimension de poche n'a pas de tueur : c'est pourtant la facon de tuer
        // de SCP-106, dont le second coup teleporte sans achever.
        public static Player ScpBehind(PlayerDeathEventArgs ev)
        {
            if (ev?.DamageHandler is not UniversalDamageHandler handler || handler.TranslationId != DeathTranslations.PocketDecay.Id)
                return null;

            foreach (Player player in Player.ReadyList)
            {
                if (player.Role == RoleTypeId.Scp106)
                    return player;
            }

            return null;
        }
    }
}
