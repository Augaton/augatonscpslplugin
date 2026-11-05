using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class DoubleCoinEffect : ICoinEffect
    {
        private readonly int maxInventorySlots;

        private bool droppedOnGround;

        public DoubleCoinEffect(int maxInventorySlots)
        {
            this.maxInventorySlots = maxInventorySlots;
        }

        public string Name
        {
            get { return "DoubleCoin"; }
        }

        public string Message
        {
            get
            {
                if (droppedOnGround)
                {
                    return Plugin.Instance.Translation
                        .DoubleCoinDroppedMessage;
                }

                return Plugin.Instance.Translation
                    .DoubleCoinEffectMessage;
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
            droppedOnGround = false;

            for (int i = 0; i < 2; i++)
            {
                if (player.Inventory.UserInventory.Items.Count < maxInventorySlots)
                {
                    player.AddItem(ItemType.Coin);
                    continue;
                }

                API.PickupDrop.AtFeet(player, ItemType.Coin);

                droppedOnGround = true;
            }
        }
    }
}
