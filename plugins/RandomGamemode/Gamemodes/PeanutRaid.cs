using System.Collections.Generic;
using AugatonLib.Arbitration;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class PeanutRaid : GamemodeBase
    {
        public override string Key => "peanutraid";

        public override string DisplayName => "Invasion de cacahuetes";

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            List<Player> shrink = new List<Player>(8);

            int changed = 0;

            foreach (Player player in ReadyPlayers())
            {
                if (player is null || player.IsDestroyed)
                    continue;

                if (player.IsSCP)
                {
                    player.SetRole(RoleTypeId.ClassD);
                    shrink.Add(player);
                }
                else
                {
                    player.SetRole(RoleTypeId.Scp173);
                }

                if (++changed % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            yield return Timing.WaitForSeconds(1f);

            foreach (Player player in shrink)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive)
                    continue;

                ScaleArbiter.Request(player, Interop.Owner, Vector3.one * 0.4f);
            }

            yield return Timing.WaitForSeconds(2f);
            Announce(Settings.Description);
        }
    }
}
