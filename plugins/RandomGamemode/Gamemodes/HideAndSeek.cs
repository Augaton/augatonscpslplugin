using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class HideAndSeek : GamemodeBase
    {
        private string seekerUserId;

        public override string Key => "hideandseek";

        public override string DisplayName => "Cache-cache";

        public override bool CanStart(int playerCount) => playerCount >= 4;

        public override bool PausesWorldEvents => true;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            List<Player> hiders = new List<Player>(32);

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && !player.IsDestroyed)
                    hiders.Add(player);
            }

            if (hiders.Count < 2)
            {
                Abort();
                yield break;
            }

            Player seeker = hiders[UnityEngine.Random.Range(0, hiders.Count)];
            seekerUserId = seeker.UserId;
            hiders.Remove(seeker);

            // Pendant la cachette, seuls des Classe-D vivent : le jeu croirait le round termine.
            KeepRoundOpen = true;

            seeker.SetRole(RoleTypeId.Spectator);

            int changed = 0;

            foreach (Player hider in hiders)
            {
                if (hider.IsDestroyed)
                    continue;

                hider.SetRole(RoleTypeId.ClassD);
                hider.ClearInventory();
                hider.AddItem(ItemType.Flashlight);

                if (++changed % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            Announce(Settings.Description);

            yield return Timing.WaitForSeconds(Config.HideAndSeekHideSeconds);

            KeepRoundOpen = false;

            Player hunter = Player.Get(seekerUserId);

            if (hunter is null || hunter.IsDestroyed)
                yield break;

            hunter.SetRole(Config.HideAndSeekSeekerRole);

            // Les joueurs se cachent autour des cellules : depuis son point d'apparition en zone lourde,
            // le chasseur perdrait une bonne partie de sa traque a les rejoindre.
            MoveToSpawn(hunter, RoleTypeId.ClassD);
            Announce(Text.HideAndSeekReleased);

            yield return Timing.WaitForSeconds(Config.HideAndSeekHuntSeconds);

            Announce(Text.HideAndSeekSurvived);
            Plugin.Instance.Registry.ForceRoundEnd();
        }

        public override void Stop() => seekerUserId = null;
    }
}
