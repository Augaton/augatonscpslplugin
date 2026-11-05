using Footprinting;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.API
{
    public static class PickupDrop
    {
        public static void AtFeet(Player player, ItemType type)
        {
            Pickup pickup = Pickup.Create(
                type,
                player.Position + (Vector3.up * 0.2f),
                Quaternion.identity,
                Vector3.one,
                false);

            if (pickup is null)
                return;

            pickup.Base.PreviousOwner = new Footprint(player.ReferenceHub);
            pickup.Spawn();
        }

        // Le jeu refuse un objet au-dela de 8 emplacements : la recompense tombe au sol plutot que de se perdre.
        public static void Give(Player player, ItemType type)
        {
            if (player.AddItem(type) is null)
                AtFeet(player, type);
        }
    }
}
