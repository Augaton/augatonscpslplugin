using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class OneAmmoLogicerEffect : ICoinEffect
    {
        public string Name
        {
            get { return "OneAmmoLogicer"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .OneAmmoLogicerEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive;
        }

        // LabAPI ne sait pas charger la chambre d'un pickup, et ReceivedFlags garde cette munition par
        // numero de serie : il faut une instance simulee sur l'hote, comme le faisait Item.Create.
        public void Execute(Player player)
        {
            ushort serial = ItemSerialGenerator.GenerateNext();

            Firearm firearm =
                Server.Host?.Inventory.CreateItemInstance(
                    new ItemIdentifier(ItemType.GunLogicer, serial),
                    false) as Firearm;

            if (firearm == null)
                return;

            firearm.OnAdded(null);
            firearm.InstantiationStatus = AutosyncInstantiationStatus.SimulatedInstance;

            if (firearm.TryGetModule(out AutomaticActionModule action))
            {
                action.AmmoStored = 1;
                action.ServerResync();
            }

            ItemPickupBase pickup = InventoryExtensions.ServerCreatePickup(
                firearm,
                new PickupSyncInfo(ItemType.GunLogicer, firearm.Weight, serial),
                player.Position,
                Quaternion.identity);

            firearm.OnRemoved(pickup);
            UnityEngine.Object.Destroy(firearm.gameObject);
        }
    }
}
