using System.Collections.Generic;
using AugatonLib.Random;
using AugatonLib.Text;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class HotPotato : GamemodeBase
    {
        // Sans ce delai, un chargeur vide d'un coup ferait passer la bombe et revenir dans la meme seconde.
        private const float PassImmunitySeconds = 1f;

        private string carrierUserId;
        private float passAllowedAt;

        public override string Key => "hotpotato";

        public override string DisplayName => "Patate chaude";

        public override bool CanStart(int playerCount) => playerCount >= 4;

        public override bool HumansOnly => true;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            foreach (Player player in ReadyPlayers())
            {
                player.SetRole(RoleTypeId.ClassD);
                player.ClearInventory();
                player.AddItem(ItemType.GunCOM15);
                player.SetAmmo(ItemType.Ammo9x19, 200);
            }

            KeepRoundOpen = true;
            Announce(Settings.Description);

            yield return Timing.WaitForSeconds(Config.HotPotatoPauseSeconds);

            while (true)
            {
                List<Player> alive = AlivePlayers();

                if (alive.Count <= 1)
                {
                    if (alive.Count == 1)
                        Announce(Text.HotPotatoWinner.Replace("%PLAYER%", SafeText.Nickname(alive[0])));

                    Plugin.Instance.Registry.ForceRoundEnd();
                    yield break;
                }

                GiveBomb(Chance.Pick(alive));

                yield return Timing.WaitForSeconds(Config.HotPotatoFuseSeconds);

                // Les autres ont pu quitter le serveur : le dernier ne doit pas sauter sur sa victoire.
                if (CountAlive() > 1)
                    Explode();

                yield return Timing.WaitForSeconds(Config.HotPotatoPauseSeconds);
            }
        }

        public override void Stop() => carrierUserId = null;

        // Seule la bombe tue : une balle ne sert qu'a la passer.
        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (ev?.Player is null || ev.Attacker is null || !KeepRoundOpen)
                return;

            ev.IsAllowed = false;

            if (ev.Attacker.UserId != carrierUserId || ev.Player == ev.Attacker || Time.time < passAllowedAt)
                return;

            ev.Attacker.DisableEffect<MovementBoost>();
            GiveBomb(ev.Player);
        }

        private static List<Player> AlivePlayers()
        {
            List<Player> alive = ReadyPlayers();

            alive.RemoveAll(player => !player.IsAlive);
            return alive;
        }

        private void GiveBomb(Player player)
        {
            carrierUserId = player.UserId;
            passAllowedAt = Time.time + PassImmunitySeconds;

            player.EnableEffect<MovementBoost>(Config.HotPotatoSpeedBoost);
            Announce(Text.HotPotatoCarrier.Replace("%PLAYER%", SafeText.Nickname(player)), 4);
        }

        private void Explode()
        {
            Player carrier = Player.Get(carrierUserId);
            carrierUserId = null;

            if (carrier is null || carrier.IsDestroyed || !carrier.IsAlive)
                return;

            TimedGrenadeProjectile.SpawnActive(carrier.Position, ItemType.GrenadeHE, carrier, 0f);
            carrier.Kill();
        }
    }
}
