using System.Collections.Generic;
using AugatonLib.World;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class GeneratorRace : GamemodeBase
    {
        public override string Key => "generatorrace";

        public override string DisplayName => "Course aux generateurs";

        public override bool CanStart(int playerCount) => playerCount >= 5;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            foreach (Player player in ReadyPlayers())
            {
                if (player.IsHuman)
                    ItemGrant.Give(player, ItemType.KeycardMTFCaptain);
            }

            Announce(Settings.Description);

            float end = Time.time + Config.GeneratorRaceSeconds;

            while (Time.time < end)
            {
                yield return Timing.WaitForSeconds(1f);

                if (AllEngaged())
                {
                    EndRound(Text.GeneratorRaceWon);
                    yield break;
                }
            }

            EndRound(Text.GeneratorRaceLost);
        }

        private static bool AllEngaged()
        {
            if (Generator.List.Count == 0)
                return false;

            foreach (Generator generator in Generator.List)
            {
                if (!generator.Engaged)
                    return false;
            }

            return true;
        }
    }
}
