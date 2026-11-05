using AugatonLib.Bus;
using System.Collections.Generic;
using AugatonLib.Integrations;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class PillParty : GamemodeBase
    {
        public override string Key => "pillparty";

        public override string DisplayName => "Soiree pilules";

        public override bool HumansOnly => true;

        private static void GivePill(Player player)
        {
            if (UnityEngine.Random.Range(0, 100) < Config.PillPartyCustomChance)
            {
                if (PluginBus.Publish(BusTopics.PillGiveRequested, Interop.Owner, player) > 0)
                    return;

                UncomplicatedEntry entry = Bridge.ItemsAvailable ? Bridge.RandomItem() : null;

                if (entry is not null && Bridge.TryGiveItem(player, entry.Id))
                    return;
            }

            player.AddItem(ItemType.SCP500);
        }

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

                for (int i = 0; i < Config.PillPartyCount; i++)
                    GivePill(player);

                player.AddItem(ItemType.Flashlight);
            }

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }
    }
}
