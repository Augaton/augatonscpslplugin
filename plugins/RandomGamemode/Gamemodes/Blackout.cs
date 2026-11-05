using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class Blackout : GamemodeBase
    {
        public override string Key => "blackout";

        public override string DisplayName => "Panne generale";

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive || player.IsSCP)
                    continue;

                player.AddItem(ItemType.Flashlight);
                player.AddItem(ItemType.Lantern);
            }

            KillLights(Config.BlackoutDuration);

            yield return Timing.WaitForSeconds(2f);
            Announce(Settings.Description);

            while (Round.IsRoundInProgress)
            {
                yield return Timing.WaitForSeconds(Mathf.Max(2f, Config.BlackoutDuration - 1f));
                KillLights(Config.BlackoutDuration);
            }
        }

        public override void Stop() => RestoreLights();
    }
}
