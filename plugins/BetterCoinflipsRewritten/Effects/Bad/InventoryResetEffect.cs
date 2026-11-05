using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class InventoryResetEffect : ICoinEffect
    {
        public string Name
        {
            get { return "InventoryReset"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .InventoryResetEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   !player.IsWithoutItems;
        }

        public void Execute(Player player)
        {
            player.ClearInventory();
        }
    }
}
