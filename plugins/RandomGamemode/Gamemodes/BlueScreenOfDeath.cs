using System.Collections.Generic;
using AugatonLib.Arbitration;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class BlueScreenOfDeath : GamemodeBase
    {
        private const int Scp079Level = 3;

        public override string Key => "bluescreen";

        public override string DisplayName => "Ecran bleu de la mort";

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            int changed = 0;

            foreach (Player player in ReadyPlayers())
            {
                if (player is null || player.IsDestroyed)
                    continue;

                if (player.IsSCP)
                {
                    player.SetRole(RoleTypeId.Scp079);
                    SetLevel(player, Scp079Level);
                }
                else
                {
                    player.SetRole(RoleTypeId.Scientist);
                    player.ClearInventory();
                }

                if (++changed % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            LightArbiter.Tint(Interop.Owner, Color.blue);

            yield return Timing.WaitForSeconds(3f);
            Announce(Settings.Description);
        }

        public override void Stop() => LightArbiter.ReleaseTint(Interop.Owner);

        public override void OnStoppingGenerator(PlayerDeactivatingGeneratorEventArgs ev)
        {
            if (ev is not null)
                ev.IsAllowed = false;
        }

        private static void SetLevel(Player player, int level)
        {
            if (!(player.RoleBase is Scp079Role scp079)
                || !scp079.SubroutineModule.TryGetSubroutine(out Scp079TierManager tier))
            {
                return;
            }

            int[] thresholds = tier.AbsoluteThresholds;

            tier.TotalExp = thresholds[Mathf.Clamp(level - 2, 0, thresholds.Length - 1)];
        }
    }
}
