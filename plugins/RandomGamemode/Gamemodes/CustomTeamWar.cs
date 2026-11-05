using System.Collections.Generic;
using AugatonLib.Integrations;
using LabApi.Features.Console;
using MEC;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class CustomTeamWar : GamemodeBase
    {
        public override string Key => "customteamwar";

        public override string DisplayName => "Guerre des factions";

        public override bool CanStart(int playerCount) => Bridge.TeamsAvailable && Bridge.Teams.Count >= 2;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(5f);

            UncomplicatedEntry first = Bridge.RandomTeam();
            UncomplicatedEntry second = Bridge.RandomTeam();

            int guard = 0;

            while (second is not null && first is not null && second.Id == first.Id && guard < 12)
            {
                second = Bridge.RandomTeam();
                guard++;
            }

            if (first is not null && Bridge.TrySpawnTeam(first.Id))
                Logger.Info($"[RandomGamemode] Equipe personnalisee \"{first.Name}\" invoquee.");

            yield return Timing.WaitForSeconds(Config.CustomTeamWarDelay);

            if (second is not null && second.Id != first?.Id && Bridge.TrySpawnTeam(second.Id))
                Logger.Info($"[RandomGamemode] Equipe personnalisee \"{second.Name}\" invoquee.");

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }
    }
}
