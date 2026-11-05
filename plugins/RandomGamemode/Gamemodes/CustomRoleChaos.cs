using System.Collections.Generic;
using AugatonLib.Integrations;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class CustomRoleChaos : GamemodeBase
    {
        public override string Key => "customrolechaos";

        public override string DisplayName => "Anarchie des roles";

        public override bool CanStart(int playerCount) => Bridge.RolesAvailable && Bridge.Roles.Count > 0;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(4f);

            int applied = 0;
            List<string> userIds = new List<string>(32);

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && !player.IsDestroyed && player.IsAlive)
                    userIds.Add(player.UserId);
            }

            foreach (string userId in userIds)
            {
                Player player = Player.Get(userId);

                if (player is null || player.IsDestroyed || !player.IsAlive)
                {
                    yield return Timing.WaitForSeconds(0.1f);
                    continue;
                }

                UncomplicatedEntry entry = Bridge.RandomRole();

                if (entry is not null && Bridge.TryApplyRole(player, entry.Id))
                    applied++;

                yield return Timing.WaitForSeconds(0.1f);
            }

            Logger.Info($"[RandomGamemode] Anarchie des roles : {applied} roles personnalises appliques.");

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }
    }
}
