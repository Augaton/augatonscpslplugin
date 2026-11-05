using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class SkyLaunchEffect : ICoinEffect
    {
        private readonly float height;

        public SkyLaunchEffect(float height)
        {
            this.height = height;
        }

        public string Name
        {
            get { return "SkyLaunch"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .SkyLaunchEffectMessage
                    .Replace(
                        "{height}",
                        height.ToString("0.#"));
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   height > 0f;
        }

        public void Execute(Player player)
        {
            player.Position += Vector3.up * height;
        }
    }
}
