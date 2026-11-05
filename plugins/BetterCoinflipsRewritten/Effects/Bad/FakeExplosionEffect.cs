using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using UnityEngine;
using Utils;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class FakeExplosionEffect : ICoinEffect
    {
        private readonly float stunDuration;

        public FakeExplosionEffect(float stunDuration)
        {
            this.stunDuration = stunDuration;
        }

        public string Name
        {
            get { return "FakeExplosion"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .FakeExplosionEffectMessage;
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

            ExplosionUtils.ServerSpawnEffect(position, ItemType.GrenadeHE);

            if (stunDuration <= 0f)
                return;

            player.EnableEffect<Deafened>(1, stunDuration);
            player.EnableEffect<Concussed>(1, stunDuration);
        }
    }
}
