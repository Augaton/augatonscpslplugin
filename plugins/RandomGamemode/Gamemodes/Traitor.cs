using System.Collections.Generic;
using AugatonLib.Random;
using AugatonLib.Text;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class Traitor : GamemodeBase
    {
        private string traitorUserId;

        public override string Key => "traitor";

        public override string DisplayName => "Un traitre parmi nous";

        public override bool CanStart(int playerCount) => playerCount >= 5;

        public override bool HumansOnly => true;

        protected override RoleTypeId HumanRole => RoleTypeId.Scientist;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            List<Player> players = ReadyPlayers();

            if (players.Count < 3)
            {
                Abort();
                yield break;
            }

            Player traitor = Chance.Pick(players);
            traitorUserId = traitor.UserId;

            KeepRoundOpen = true;

            int changed = 0;

            foreach (Player player in players)
            {
                if (player.IsDestroyed)
                    continue;

                player.SetRole(RoleTypeId.Scientist);
                player.ClearInventory();
                player.AddItem(ItemType.Flashlight);
                player.AddItem(ItemType.GunCOM15);
                player.SetAmmo(ItemType.Ammo9x19, Config.TraitorAmmo);

                if (++changed % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            Announce(Settings.Description);
            traitor.SendBroadcast(Text.TraitorSecret, 10, Broadcast.BroadcastFlags.Normal, true);

            yield return Timing.WaitForSeconds(Config.TraitorRoundSeconds);

            EndRound(Text.TraitorEscapedJustice.Replace("%PLAYER%", TraitorName()));
        }

        public override void Stop() => traitorUserId = null;

        public override void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev?.Player is null || !KeepRoundOpen)
                return;

            if (ev.Player.UserId == traitorUserId)
            {
                EndRound(Text.TraitorCaught.Replace("%PLAYER%", SafeText.Nickname(ev.Player)));
                return;
            }

            Player killer = ev.Attacker;

            // Tirer au hasard doit couter quelque chose : sinon, abattre tout le monde resout l'enquete.
            if (killer is not null && killer != ev.Player && killer.UserId != traitorUserId)
            {
                killer.ClearInventory();
                killer.SendBroadcast(Text.TraitorWrongTarget, 5);
            }

            if (CountAlive() <= 1)
                EndWithTraitorVictory();
        }

        public override void OnPlayerLeft(string userId)
        {
            if (!KeepRoundOpen)
                return;

            if (userId == traitorUserId)
                EndRound(Text.TraitorLeft);
            else if (CountAlive() <= 1)
                EndWithTraitorVictory();
        }

        private void EndWithTraitorVictory()
        {
            Plugin.Instance.Registry.DeclareWinner(Player.Get(traitorUserId));
            EndRound(Text.TraitorWon.Replace("%PLAYER%", TraitorName()));
        }

        private string TraitorName()
        {
            Player traitor = Player.Get(traitorUserId);

            return traitor is null ? "?" : SafeText.Nickname(traitor);
        }
    }
}
