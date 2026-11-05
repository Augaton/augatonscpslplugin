using System.Collections.Generic;
using AugatonLib.Integrations;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class CustomItemRush : GamemodeBase
    {
        public override string Key => "customitemrush";

        public override string DisplayName => "Arsenal experimental";

        public override bool CanStart(int playerCount) => Bridge.ItemsAvailable && Bridge.Items.Count > 0;

        public override bool HumansOnly => true;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(2.5f);

            ConvertScpsToHumans();

            yield return Timing.WaitForSeconds(1.5f);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive || player.IsSCP)
                    continue;

                player.ClearInventory();

                for (int i = 0; i < Config.CustomItemRushCount; i++)
                {
                    UncomplicatedEntry entry = Bridge.RandomItem();

                    if (entry is null)
                        break;

                    Bridge.TryGiveItem(player, entry.Id);
                }
            }

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }
    }
}
