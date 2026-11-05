using AugatonLib.Arbitration;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class SizeChangeEffect : ICoinEffect
    {
        private string selectedSize;

        public SizeChangeEffect()
        {

            selectedSize =
                Plugin.Instance.Translation
                    .ChangedSizeName;
        }

        public string Name
        {
            get { return "SizeChange"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .SizeChangeEffectMessage
                    .Replace(
                        "{size}",
                        selectedSize);
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
            bool makeSmall =
                API.Rng.Next(0, 2) == 0;

            float factor = makeSmall ? 0.7f : 1.1f;

            ScaleArbiter.Request(
                player,
                Interop.Owner,
                new Vector3(factor, factor, factor));

            selectedSize = makeSmall
                ? Plugin.Instance.Translation.SmallSizeName
                : Plugin.Instance.Translation.LargeSizeName;
        }
    }
}
