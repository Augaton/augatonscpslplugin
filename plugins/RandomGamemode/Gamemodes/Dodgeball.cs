using System.Collections.Generic;
using System.Reflection;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PlayerRoles;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class Dodgeball : GamemodeBase
    {
        private const float BallFuse = 1f;

        // Le premier rebond relance la meche depuis ce champ prive, sans accesseur public :
        // reduire seulement le temps restant serait annule des ce rebond.
        private static readonly FieldInfo FuseField = typeof(InventorySystem.Items.ThrowableProjectiles.TimeGrenade)
            .GetField("_fuseTime", BindingFlags.Instance | BindingFlags.NonPublic);

        public override string Key => "dodgeball";

        protected override bool PlaysToLastStanding => true;

        public override string DisplayName => "Balle au prisonnier";

        public override bool HumansOnly => true;

        protected override RoleTypeId HumanRole => RoleTypeId.FacilityGuard;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            ConvertScpsToHumans();

            yield return Timing.WaitForSeconds(1f);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive)
                    continue;

                player.ClearInventory();

                for (int i = 0; i < 7; i++)
                    player.AddItem(ItemType.SCP018);

                MoveToSpawn(player, RoleTypeId.NtfCaptain);
            }

            yield return Timing.WaitForSeconds(2f);
            Announce(Settings.Description);
        }

        public override void OnThrownProjectile(PlayerThrewProjectileEventArgs ev)
        {
            if (ev?.Projectile is not Scp018Projectile ball)
                return;

            // Cet event part apres le spawn, et le client ne recoit l'echelle que dans le message de spawn.
            GameObject gameObject = ball.GameObject;

            NetworkServer.UnSpawn(gameObject);
            gameObject.transform.localScale *= 3f;
            NetworkServer.Spawn(gameObject);

            FuseField?.SetValue(ball.Base, BallFuse);
            ball.RemainingTime = BallFuse;

            ev.Player?.AddItem(ItemType.SCP018);
        }

        public override void OnDroppingItem(PlayerDroppingItemEventArgs ev)
        {
            if (ev is not null)
                ev.IsAllowed = false;
        }
    }
}
