using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Random;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class KingOfTheHill : GamemodeBase
    {
        private const RoleTypeId GuardRole = RoleTypeId.NtfSergeant;
        private const RoleTypeId RaiderRole = RoleTypeId.ChaosRifleman;
        private const int ProgressEverySeconds = 15;

        private readonly HashSet<string> guards = new HashSet<string>();

        private Room hill;
        private int guardScore;
        private int raiderScore;

        public override string Key => "kingofthehill";

        public override string DisplayName => "Roi de la salle";

        public override bool CanStart(int playerCount) => playerCount >= 4;

        public override bool HumansOnly => true;

        protected override RoleTypeId HumanRole => GuardRole;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            hill = PickHill();

            if (hill is null)
            {
                Abort();
                yield break;
            }

            if (!ColorUtility.TryParseHtmlString(Config.KingOfTheHillColor, out Color color))
                color = Color.yellow;

            LightArbiter.TintRoom(Interop.Owner, hill, color);

            KeepRoundOpen = true;

            List<Player> players = ReadyPlayers();
            Chance.Shuffle(players);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].IsDestroyed)
                    continue;

                if (i % 2 == 0)
                    guards.Add(players[i].UserId);

                Deploy(players[i]);

                if ((i + 1) % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            Announce(Text.KingOfTheHillRoom.Replace("%ROOM%", hill.Name.ToString()));

            int elapsed = 0;

            while (guardScore < Config.KingOfTheHillTarget && raiderScore < Config.KingOfTheHillTarget)
            {
                yield return Timing.WaitForSeconds(1f);

                Score();

                if (++elapsed % ProgressEverySeconds == 0)
                    Announce(Text.KingOfTheHillProgress.Replace("%GUARDS%", guardScore.ToString()).Replace("%RAIDERS%", raiderScore.ToString()), 4);
            }

            EndRound(guardScore >= Config.KingOfTheHillTarget ? Text.KingOfTheHillGuardsWon : Text.KingOfTheHillRaidersWon);
        }

        public override void Stop()
        {
            if (hill is not null)
                LightArbiter.ReleaseRoomTint(Interop.Owner, hill);

            hill = null;
            guardScore = 0;
            raiderScore = 0;
            guards.Clear();
        }

        // Ce mode fait reapparaitre sans fin : chaque mort laisserait sinon une dotation complete et un corps au sol,
        // autant d'objets reseau que rien ne retire.
        public override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (KeepRoundOpen)
                ev?.Player?.ClearInventory();
        }

        public override void OnSpawningRagdoll(PlayerSpawningRagdollEventArgs ev)
        {
            if (KeepRoundOpen && ev is not null)
                ev.IsAllowed = false;
        }

        public override void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            string userId = ev?.Player?.UserId;

            if (string.IsNullOrEmpty(userId) || !KeepRoundOpen)
                return;

            Delay(Config.KingOfTheHillRespawnSeconds, () =>
            {
                Player player = Player.Get(userId);

                if (player is not null && !player.IsDestroyed && !player.IsAlive)
                    Deploy(player);
            });
        }

        private static Room PickHill()
        {
            List<Room> rooms = new List<Room>(16);

            foreach (Room room in Room.Get(FacilityZone.Entrance))
            {
                if (room.Name != RoomName.Unnamed)
                    rooms.Add(room);
            }

            return Chance.Pick(rooms);
        }

        // Les deux camps partent de la zone d'entree : depuis leurs points de renfort en surface, le trajet
        // jusqu'a la salle prendrait une bonne part de la partie.
        private void Deploy(Player player)
        {
            player.SetRole(guards.Contains(player.UserId) ? GuardRole : RaiderRole);
            MoveToSpawn(player, RoleTypeId.FacilityGuard);
        }

        private void Score()
        {
            int guardsInside = 0;
            int raidersInside = 0;

            foreach (Player player in Player.ReadyList)
            {
                if (!player.IsAlive || player.Room != hill)
                    continue;

                if (guards.Contains(player.UserId))
                    guardsInside++;
                else
                    raidersInside++;
            }

            if (guardsInside > raidersInside)
                guardScore++;
            else if (raidersInside > guardsInside)
                raiderScore++;
        }
    }
}
