using System.Collections.Generic;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class HideAndSeek : GamemodeBase
    {
        private string seekerUserId;
        private bool hiding;

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
                yield break;

            Player seeker = hiders[UnityEngine.Random.Range(0, hiders.Count)];
            seekerUserId = seeker.UserId;
            hiders.Remove(seeker);
            hiding = true;

            seeker.SetRole(RoleTypeId.Spectator);

            foreach (Player hider in hiders)
            {
                hider.SetRole(RoleTypeId.ClassD);
                hider.ClearInventory();
                hider.AddItem(ItemType.Flashlight);
            }

            Announce(Settings.Description);

            yield return Timing.WaitForSeconds(Config.HideAndSeekHideSeconds);

            hiding = false;

            Player hunter = Player.Get(seekerUserId);

            if (hunter is null || hunter.IsDestroyed)
                yield break;

            hunter.SetRole(Config.HideAndSeekSeekerRole);
            Announce(Text.HideAndSeekReleased);

            yield return Timing.WaitForSeconds(Config.HideAndSeekHuntSeconds);

            Announce(Text.HideAndSeekSurvived);
            Plugin.Instance.Registry.ForceRoundEnd();
        }

        public override void Stop()
        {
            hiding = false;
            seekerUserId = null;
        }

        // Pendant la cachette, seuls des Classe-D vivent : le jeu croirait le round termine.
        public override void OnEndingRound(RoundEndingConditionsCheckEventArgs ev)
        {
            if (ev is not null && hiding)
                ev.CanEnd = false;
        }
    }
}
