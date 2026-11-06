using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class Evacuation : GamemodeBase
    {
        public override string Key => "evacuation";

        public override string DisplayName => "Evacuation generale";

        public override bool CanStart(int playerCount) => playerCount >= 4;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            Announce(Settings.Description);

            yield return Timing.WaitForSeconds(Config.EvacuationDelaySeconds);

            if (Warhead.IsDetonated)
                yield break;

            if (!Warhead.IsDetonationInProgress)
                Warhead.Start();

            // Apres le depart et non avant : le jeu refuse de lancer une ogive verrouillee.
            Warhead.IsLocked = true;
        }

        public override void Stop() => Warhead.IsLocked = false;
    }
}
