using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class AnnoyingMimicry : GamemodeBase
    {
        public override string Key => "mimicry";

        public override string DisplayName => "Mimetisme agacant";

        public override IEnumerator<float> Run()
        {
            GateLock.Engage();

            yield return Timing.WaitForSeconds(3f);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed)
                    continue;

                if (player.IsSCP)
                {
                    player.SetRole(RoleTypeId.Scp939);
                }
                else
                {
                    player.SetRole(RoleTypeId.ClassD);
                    player.AddItem(ItemType.Jailbird);
                }
            }

            yield return Timing.WaitForSeconds(2f);
            Announce(Settings.Description);
        }

        public override void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev?.Player is null)
                return;

            string userId = ev.Player.UserId;

            Delay(3f, () =>
            {
                Player target = Player.Get(userId);

                if (target is null || target.IsDestroyed)
                    return;

                target.SetRole(RoleTypeId.Scp939);
            });
        }

        public override void OnInteractingDoor(PlayerInteractingDoorEventArgs ev) => GateLock.Apply(ev);

        public override void Stop() => GateLock.Disengage();
    }
}
