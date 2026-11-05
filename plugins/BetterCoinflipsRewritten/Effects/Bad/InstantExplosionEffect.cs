using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class InstantExplosionEffect : ICoinEffect
    {
        private readonly float fuseTime;

        public InstantExplosionEffect(float fuseTime)
        {
            this.fuseTime = fuseTime;
        }

        public string Name
        {
            get { return "InstantExplosion"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .InstantExplosionEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive;
        }

        public void Execute(Player player)
        {
            Vector3 position =
                player.Position + (Vector3.up * 0.2f);

            TimedGrenadeProjectile.SpawnActive(
                position,
                ItemType.GrenadeHE,
                player,
                Mathf.Max(0f, fuseTime));
        }
    }
}
