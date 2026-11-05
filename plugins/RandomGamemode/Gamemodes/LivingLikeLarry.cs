using System.Collections.Generic;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class LivingLikeLarry : GamemodeBase
    {
        public override string Key => "larry";

        public override string DisplayName => "La vie selon Larry";

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            Door bottom = Door.Get(DoorName.Lcz173Bottom);
            Door gate = Door.Get(DoorName.Lcz173Gate);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed)
                    continue;

                if (player.IsSCP)
                {
                    player.SetRole(RoleTypeId.ClassD);
                    player.AddItem(ItemType.KeycardO5);

                    if (bottom is not null)
                        player.Position = bottom.Position + (Vector3.up * 2f);
                }
                else
                {
                    player.SetRole(RoleTypeId.Scp106);

                    if (gate is not null)
                        player.Position = gate.Position + (Vector3.up * 2f);
                }
            }

            yield return Timing.WaitForSeconds(2f);
            Announce(Settings.Description);
        }
    }
}
