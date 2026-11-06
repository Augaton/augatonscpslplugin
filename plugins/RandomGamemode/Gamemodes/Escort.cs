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

            Chance.Shuffle(players);

            Player vip = players[0];
            vipUserId = vip.UserId;

            vip.SetRole(RoleTypeId.Scientist);
            vip.AddItem(ItemType.KeycardO5);

            for (int i = 1; i < players.Count; i++)
            {
                if (i % 2 == 1)
                {
                    players[i].SetRole(RoleTypeId.NtfPrivate);
                    players[i].Position = vip.Position;
                }
                else
                {
                    players[i].SetRole(RoleTypeId.ChaosConscript);
                }
            }

            KeepRoundOpen = true;
            Announce(Text.EscortStart.Replace("%PLAYER%", SafeText.Nickname(vip)));

            yield return Timing.WaitForSeconds(Config.EscortRoundSeconds);

            EndRound(Text.EscortFailed);
        }

        public override void Stop() => vipUserId = null;

        public override void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev?.Player is not null && KeepRoundOpen && ev.Player.UserId == vipUserId)
                EndRound(Text.EscortFailed);
        }

        public override void OnPlayerEscaped(PlayerEscapedEventArgs ev)
        {
            if (ev?.Player is not null && KeepRoundOpen && ev.Player.UserId == vipUserId)
                EndRound(Text.EscortSucceeded);
        }
    }
}
