using AugatonLib.World;
using System;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class TantrumTrapEffect : ICoinEffect
    {
        public string Name
        {
            get { return "TantrumTrap"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .TantrumTrapEffectMessage;
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
            if (!Tantrums.TrySpawn(player.Position))
            {
                throw new InvalidOperationException(
                    "The tantrum prefab was not found.");
            }
        }
    }
}
