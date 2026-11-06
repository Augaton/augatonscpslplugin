using AugatonLib.World;
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

        public bool IsSpent(Item coin)
        {
            return coin is not null && SpentItems.IsSpent(coin.Serial);
        }

        public void Schedule(Item coin)
        {
            if (!IsEnabled || coin is null)
                return;

            ushort serial = coin.Serial;

            SpentItems.Mark(serial);

            API.Scheduler.Delay(
                plugin.Config.CoinConsumeDelay,
                delegate { Consume(serial); });
        }

        public void Clear() => SpentItems.Clear();

        // Pendant le delai, la piece peut etre lachee et ramassee par un autre : seul son serial la suit.
        private void Consume(ushort serial)
        {
            SpentItems.Release(serial);

            Item carried = Item.Get(serial);

            if (carried is not null && !carried.IsDestroyed && carried.CurrentOwner is Player owner)
            {
                owner.RemoveItem(carried);
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
