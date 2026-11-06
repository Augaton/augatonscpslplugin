using AugatonLib.Bus;
using InventorySystem;
using LabApi.Features.Wrappers;
using UnityEngine;
using GameAmmoItem = InventorySystem.Items.Firearms.Ammo.AmmoItem;

namespace AugatonLib.World
{
    /// <summary>
    /// Donne un objet a un joueur sans qu'il puisse se perdre : une boite de munitions va dans sa reserve,
    /// une pilule passe par SCP500s s'il est installe, et un inventaire plein laisse l'objet a ses pieds.
    /// </summary>
    public static class ItemGrant
    {
        private const string Source = "AugatonLib";
        private const int MaxItems = 8;

        public static void Give(Player player, ItemType type, int quantity = 1)
        {
            if (player is null || player.IsDestroyed || type == ItemType.None)
                return;

            if (InventoryItemLoader.TryGetItem(type, out GameAmmoItem _))
            {
                player.AddAmmo(type, (ushort)Mathf.Clamp(quantity, 1, ushort.MaxValue));
                return;
            }

            int count = Mathf.Clamp(quantity, 1, MaxItems);

            for (int i = 0; i < count; i++)
                GiveOne(player, type);
        }

        private static void GiveOne(Player player, ItemType type)
        {
            if (type == ItemType.SCP500 && PluginBus.Publish(BusTopics.PillGiveRequested, Source, player) > 0)
                return;

            if (player.AddItem(type) is null)
                Pickup.Create(type, player.Position, Quaternion.identity);
        }
    }
}
