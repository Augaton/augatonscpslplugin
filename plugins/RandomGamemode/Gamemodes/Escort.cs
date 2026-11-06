using System.Collections.Generic;
using AugatonLib.Random;
using AugatonLib.Text;
using AugatonLib.World;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class Escort : GamemodeBase
    {
        private string vipUserId;

        public override string Key => "escort";

        public override string DisplayName => "Escorte";

        public override bool CanStart(int playerCount) => playerCount >= 5;

        public override bool HumansOnly => true;

        protected override RoleTypeId HumanRole => RoleTypeId.NtfPrivate;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            List<Player> players = ReadyPlayers();

            if (players.Count < 3)
                yield break;

            KeepRoundOpen = true;
            Chance.Shuffle(players);

            Player vip = players[0];
            vipUserId = vip.UserId;

            vip.SetRole(RoleTypeId.Scientist);
            ItemGrant.Give(vip, ItemType.KeycardO5);

            List<Player> escorts = new List<Player>(players.Count);

            for (int i = 1; i < players.Count; i++)
            {
                if (players[i].IsDestroyed)
                    continue;

                bool isEscort = i % 2 == 1;

                players[i].SetRole(isEscort ? RoleTypeId.NtfPrivate : RoleTypeId.ChaosConscript);

                if (isEscort)
                    escorts.Add(players[i]);

                if (i % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            Announce(Text.EscortStart.Replace("%PLAYER%", SafeText.Nickname(vip)));

            // Le jeu place le scientifique apres SetRole : sa position n'est juste qu'un instant plus tard.
            yield return Timing.WaitForSeconds(1f);

            foreach (Player escort in escorts)
            {
                if (!escort.IsDestroyed && !vip.IsDestroyed)
                    escort.Position = vip.Position;
            }

            yield return Timing.WaitForSeconds(Config.EscortRoundSeconds);

            EndRound(Text.EscortFailed);
        }

        public override void Stop() => vipUserId = null;

        public override void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev?.Player is not null && KeepRoundOpen && ev.Player.UserId == vipUserId)
                EndRound(Text.EscortFailed);
        }

        public override void OnPlayerLeft(string userId)
        {
            if (KeepRoundOpen && userId == vipUserId)
                EndRound(Text.EscortFailed);
        }

        public override void OnPlayerEscaped(PlayerEscapedEventArgs ev)
        {
            if (ev?.Player is not null && KeepRoundOpen && ev.Player.UserId == vipUserId)
                EndRound(Text.EscortSucceeded);
        }
    }
}
