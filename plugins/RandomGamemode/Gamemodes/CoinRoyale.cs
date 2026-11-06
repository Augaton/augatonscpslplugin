using AugatonLib.Runtime;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class CoinRoyale : GamemodeBase
    {
        public override bool CanStart(int playerCount) => PluginDirectory.IsLoaded("BetterCoinflipsRewritten");

        public override string Key => "coinroyale";

        public override string DisplayName => "Pile ou face royale";

        public override bool HumansOnly => true;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(2.5f);

            ConvertScpsToHumans();

            yield return Timing.WaitForSeconds(1.5f);

            int served = 0;

            foreach (Player player in ReadyPlayers())
            {
                if (player is null || player.IsDestroyed || !player.IsAlive || player.IsSCP)
                    continue;

                player.ClearInventory();

                for (int i = 0; i < Config.CoinRoyaleCount; i++)
                    player.AddItem(ItemType.Coin);

                player.AddItem(ItemType.Flashlight);

                if (++served % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }
    }
}
