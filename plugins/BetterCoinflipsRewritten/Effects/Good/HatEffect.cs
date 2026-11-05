using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class HatEffect : ICoinEffect
    {
        public string Name
        {
            get { return "SCP268"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .HatEffectMessage;
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
            API.PickupDrop.Give(player, ItemType.SCP268);
        }
    }
}
