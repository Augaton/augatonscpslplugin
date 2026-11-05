using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class TrollFlashEffect : ICoinEffect
    {
        private readonly float fuseTime;

        public TrollFlashEffect(float fuseTime)
        {
            this.fuseTime = fuseTime;
        }

        public string Name
        {
            get { return "TrollFlash"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .TrollFlashEffectMessage;
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
                ItemType.GrenadeFlash,
                player,
                Mathf.Max(0f, fuseTime));
        }
    }
}
