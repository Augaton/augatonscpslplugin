using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class LightbulbEffect : ICoinEffect
    {
        public string Name
        {
            get { return "SCP2176"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .LightbulbEffectMessage;
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
            API.PickupDrop.Give(player, ItemType.SCP2176);
        }
    }
}
