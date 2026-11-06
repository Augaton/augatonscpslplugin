using BetterCoinflipsRewritten.API;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class TrollFlashEffect : ICoinEffect
    {
        private readonly float fuseTime;
        private readonly float blindSeconds;

        public TrollFlashEffect(float fuseTime, float blindSeconds)
        {
            this.fuseTime = fuseTime;
            this.blindSeconds = blindSeconds;
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

            float fuse = Mathf.Max(0f, fuseTime);

            // Un lanceur est immunise contre son propre flash : la grenade reste visuelle, l'effet part a cote.
            TimedGrenadeProjectile.SpawnActive(position, ItemType.GrenadeFlash, null, fuse);

            Scheduler.Delay(fuse, delegate
            {
                if (!player.IsDestroyed && player.IsAlive)
                    player.EnableEffect<Flashed>(1, blindSeconds);
            });
        }
    }
}
