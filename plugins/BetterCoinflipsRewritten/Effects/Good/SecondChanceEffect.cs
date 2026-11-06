using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class SecondChanceEffect : ICoinEffect
    {
        public string Name
        {
            get { return "SecondChance"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.SecondChanceEffectMessage; }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   !SecondChance.IsArmed(player);
        }

        public void Execute(Player player)
        {
            SecondChance.Arm(player);
        }
    }
}
