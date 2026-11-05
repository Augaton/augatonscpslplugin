using System.Collections.Generic;
using AugatonLib.Arbitration;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class Infection : GamemodeBase
    {
        public override string Key => "infection";

        public override string DisplayName => "Infection";

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed)
                    continue;

                if (player.IsSCP)
                    player.SetRole(RoleTypeId.Scp049);
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

                target.SetRole(RoleTypeId.Scp0492);
                ScaleArbiter.Request(target, Interop.Owner, Vector3.one * Config.InfectionZombieScale);
                BoostHealth(target, Config.InfectionZombieHealth);
            });
        }
    }
}
