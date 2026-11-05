using InventorySystem.Items;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten
{
    public sealed class CoinConsumeService
    {
        private readonly Plugin plugin;

        public CoinConsumeService(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public bool IsEnabled
        {
            get { return plugin.Config.ConsumeCoinOnFlip; }
        }

        public void Schedule(Player player, Item coin)
        {
            if (!IsEnabled || player is null || coin is null)
                return;

            ushort serial = coin.Serial;

            API.Scheduler.Delay(
                plugin.Config.CoinConsumeDelay,
                delegate { Consume(player, serial); });
        }

        private void Consume(Player player, ushort serial)
        {
            if (player is not null &&
                !player.IsDestroyed &&
                player.Inventory.UserInventory.Items.TryGetValue(serial, out ItemBase coin))
            {
                player.RemoveItem(coin);
                return;
            }

            Pickup dropped = Pickup.Get(serial);

            if (dropped is null ||
                !dropped.IsSpawned ||
                dropped.Type != ItemType.Coin)
            {
                return;
            }

            dropped.Destroy();
        }
    }
}
