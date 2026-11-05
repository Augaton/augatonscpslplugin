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

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(2.5f);

            ConvertScpsToHumans();

            yield return Timing.WaitForSeconds(1.5f);

            bool custom = Bridge.ItemsAvailable && Bridge.Items.Count > 0;

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive || player.IsSCP)
                    continue;

                player.ClearInventory();

                for (int i = 0; i < Config.PillPartyCount; i++)
                {
                    if (custom && UnityEngine.Random.Range(0, 100) < Config.PillPartyCustomChance)
                    {
                        UncomplicatedEntry entry = Bridge.RandomItem();

                        if (entry is not null && Bridge.TryGiveItem(player, entry.Id))
                            continue;
                    }

                    player.AddItem(ItemType.SCP500);
                }

                player.AddItem(ItemType.Flashlight);
            }

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }
    }
}
