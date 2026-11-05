using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public sealed class PillRegistry
    {
        private const float SpawnPointsDelay = 2f;
        private const float EscapeRespawnDelay = 1.5f;
        private const float Scp914TransitDelay = 3.5f;

        private readonly List<PillBase> pills = new List<PillBase>(48);
        private readonly Dictionary<uint, PillBase> byId = new Dictionary<uint, PillBase>();
        private readonly Dictionary<string, float> lastUse = new Dictionary<string, float>();
        private readonly List<ushort> carried = new List<ushort>(8);

        // Un numero de serie survit au passage objet / pickup, et un ramassage detruit le pickup :
        // le suivi ne peut donc pas s'arreter a PickupDestroyed. Il dure jusqu'au round suivant.
        private readonly Dictionary<ushort, PillBase> bySerial = new Dictionary<ushort, PillBase>();

        private static Config Config => Plugin.Instance.Config;

        public IReadOnlyList<PillBase> Pills => pills;

        public void Start()
        {
            PlayerEvents.UsingItem += OnUsingItem;
            PlayerEvents.UsedItem += OnUsedItem;
            PlayerEvents.ChangingItem += OnChangingItem;
            PlayerEvents.ChangedItem += OnChangedItem;
            PlayerEvents.PickedUpItem += OnPickedUpItem;
            PlayerEvents.Dying += OnDying;
            PlayerEvents.Escaping += OnEscaping;
            PlayerEvents.ChangingRole += OnChangingRole;
            PlayerEvents.Left += OnLeft;
            Scp914Events.ProcessingInventoryItem += OnProcessingInventoryItem;
            Scp914Events.ProcessingPickup += OnProcessingPickup;
            ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        }

        public void Stop()
        {
            PlayerEvents.UsingItem -= OnUsingItem;
            PlayerEvents.UsedItem -= OnUsedItem;
            PlayerEvents.ChangingItem -= OnChangingItem;
            PlayerEvents.ChangedItem -= OnChangedItem;
            PlayerEvents.PickedUpItem -= OnPickedUpItem;
            PlayerEvents.Dying -= OnDying;
            PlayerEvents.Escaping -= OnEscaping;
            PlayerEvents.ChangingRole -= OnChangingRole;
            PlayerEvents.Left -= OnLeft;
            Scp914Events.ProcessingInventoryItem -= OnProcessingInventoryItem;
            Scp914Events.ProcessingPickup -= OnProcessingPickup;
            ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;

            pills.Clear();
            byId.Clear();
            bySerial.Clear();
            carried.Clear();
            lastUse.Clear();
        }

        public bool Add(PillBase pill)
        {
            if (byId.ContainsKey(pill.Id))
            {
                Logger.Warn($"Identifiant {pill.Id} deja utilise, la pilule {pill.Key} est ignoree.");
                return false;
            }

            pills.Add(pill);
            byId[pill.Id] = pill;
            return true;
        }

        public PillBase Find(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            foreach (PillBase pill in pills)
            {
                if (string.Equals(pill.Key, key, StringComparison.OrdinalIgnoreCase))
                    return pill;
            }

            return null;
        }

        public void Track(ushort serial, PillBase pill) => bySerial[serial] = pill;

        public bool TryGet(ushort serial, out PillBase pill) => bySerial.TryGetValue(serial, out pill);

        public bool TryGet(Item item, out PillBase pill)
        {
            pill = null;
            return item is not null && bySerial.TryGetValue(item.Serial, out pill);
        }

        public bool TryGet(Pickup pickup, out PillBase pill)
        {
            pill = null;
            return pickup is not null && bySerial.TryGetValue(pickup.Serial, out pill);
        }

        public void ClearCooldowns() => lastUse.Clear();

        private void OnUsingItem(PlayerUsingItemEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || !ev.IsAllowed || !TryGet(ev.UsableItem, out _))
                    return;

                if (!IsOnCooldown(ev.Player))
                    return;

                ev.IsAllowed = false;
                Plugin.Instance.Hints.Show(ev.Player, Plugin.Instance.Translation.PillCooldown, Config.HintDuration);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnUsingItem: {e}");
            }
        }

        private void OnUsedItem(PlayerUsedItemEventArgs ev)
        {
            try
            {
                if (ev?.Player is not null && ev.UsableItem is not null)
                    Consume(ev.Player, ev.UsableItem.Serial);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnUsedItem: {e}");
            }
        }

        private void OnChangingItem(PlayerChangingItemEventArgs ev)
        {
            try
            {
                if (ev?.Player is not null && TryGet(ev.NewItem, out PillBase pill))
                    pill.ShowItemInfo(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnChangingItem: {e}");
            }
        }

        // Rengainer une pilule apres son activation la consomme sans passer par PlayerEvents.UsedItem,
        // que le jeu n'appelle qu'a la fin du temps d'utilisation.
        private void OnChangedItem(PlayerChangedItemEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || !TryGet(ev.OldItem, out _))
                    return;

                ushort serial = ev.OldItem.Serial;

                if (!ev.Player.Inventory.UserInventory.Items.ContainsKey(serial))
                    Consume(ev.Player, serial);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnChangedItem: {e}");
            }
        }

        private void OnPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            try
            {
                if (ev?.Player is not null && TryGet(ev.Item, out PillBase pill))
                    pill.ShowItemInfo(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnPickedUpItem: {e}");
            }
        }

        // SilentDamageHandler empeche le jeu de lacher l'inventaire : la pilule serait perdue.
        private void OnDying(PlayerDyingEventArgs ev)
        {
            try
            {
                if (ev?.Player is not null && ev.IsAllowed)
                    DropCarried(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnDying: {e}");
            }
        }

        private void OnEscaping(PlayerEscapingEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || !ev.IsAllowed || ev.EscapeScenario == Escape.EscapeScenarioType.None)
                    return;

                Player player = ev.Player;

                CollectCarried(player);

                foreach (ushort serial in carried)
                {
                    PillBase pill = bySerial[serial];

                    bySerial.Remove(serial);
                    player.Inventory.ServerRemoveItem(serial, null);

                    Plugin.Instance.Delayed.Run(EscapeRespawnDelay, () =>
                    {
                        if (!player.IsDestroyed)
                            pill.Spawn(player.Position);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnEscaping: {e}");
            }
        }

        private void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            try
            {
                if (ev?.Player is null)
                    return;

                RoleChangeReason reason = ev.ChangeReason;

                if (reason != RoleChangeReason.LateJoin && reason != RoleChangeReason.Escaped && reason != RoleChangeReason.Destroyed)
                    DropCarried(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnChangingRole: {e}");
            }
        }

        private void OnProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev)
        {
            try
            {
                if (TryGet(ev?.Item, out _))
                    ev.IsAllowed = false;
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnProcessingInventoryItem: {e}");
            }
        }

        private void OnProcessingPickup(Scp914ProcessingPickupEventArgs ev)
        {
            try
            {
                if (!TryGet(ev?.Pickup, out _))
                    return;

                ev.IsAllowed = false;

                Pickup pickup = ev.Pickup;
                Vector3 output = ev.NewPosition;

                Plugin.Instance.Delayed.Run(Scp914TransitDelay, () =>
                {
                    if (!pickup.IsDestroyed)
                        pickup.Position = output;
                });
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnProcessingPickup: {e}");
            }
        }

        private void OnWaitingForPlayers()
        {
            try
            {
                bySerial.Clear();
                Plugin.Instance.Delayed.Run(SpawnPointsDelay, SpawnAtLocations);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnWaitingForPlayers: {e}");
            }
        }

        private void SpawnAtLocations()
        {
            foreach (PillBase pill in pills)
                pill.SpawnAtLocations();
        }

        private void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || string.IsNullOrEmpty(ev.Player.UserId))
                    return;

                lastUse.Remove(ev.Player.UserId);
                ScaleArbiter.Forget(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"PillRegistry.OnLeft: {e}");
            }
        }

        private void Consume(Player player, ushort serial)
        {
            if (!bySerial.TryGetValue(serial, out PillBase pill))
                return;

            bySerial.Remove(serial);

            if (!IsEligible(player))
                return;

            if (IsOnCooldown(player))
            {
                Plugin.Instance.Hints.Show(player, Plugin.Instance.Translation.PillCooldown, Config.HintDuration);
                return;
            }

            lastUse[player.UserId] = Time.realtimeSinceStartup;

            pill.Consume(player);

            PluginBus.Publish(BusTopics.PillConsumed, Interop.Owner, pill.Key);

            Logger.Debug($"{player.Nickname} a consomme la pilule {pill.Key}.", Config.Debug);
        }

        private void DropCarried(Player player)
        {
            CollectCarried(player);

            foreach (ushort serial in carried)
            {
                Pickup pickup = player.DropItem(serial);

                if (pickup is not null)
                    pickup.Weight = PillBase.PickupWeight;
            }
        }

        private void CollectCarried(Player player)
        {
            carried.Clear();

            foreach (ushort serial in player.Inventory.UserInventory.Items.Keys)
            {
                if (bySerial.ContainsKey(serial))
                    carried.Add(serial);
            }
        }

        private bool IsEligible(Player player)
        {
            if (player.IsDestroyed || !player.IsAlive || string.IsNullOrEmpty(player.UserId))
                return false;

            if (!Config.AllowScpUsage && player.IsSCP)
                return false;

            return !Config.IgnoredRoles.Contains(player.Role);
        }

        private bool IsOnCooldown(Player player)
        {
            if (Config.PillCooldownSeconds <= 0f)
                return false;

            return lastUse.TryGetValue(player.UserId, out float last)
                && Time.realtimeSinceStartup - last < Config.PillCooldownSeconds;
        }
    }
}
