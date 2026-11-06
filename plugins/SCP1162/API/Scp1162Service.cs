using System.Collections.Generic;
using AugatonLib.Caching;
using AugatonLib.Bus;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.World;
using CustomPlayerEffects;
using InventorySystem;
using InventorySystem.Items.Firearms.Attachments;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using PrimitiveFlags = AdminToys.PrimitiveFlags;

namespace SCP1162.API
{
    public sealed class Scp1162Service
    {
        private readonly Plugin plugin;
        private readonly CooldownTracker cooldowns = new CooldownTracker();
        private readonly List<PrimitiveObjectToy> anomalies = new List<PrimitiveObjectToy>(4);
        private readonly List<Vector3> positions = new List<Vector3>(4);
        private readonly List<Room> candidates = new List<Room>(8);

        public Scp1162Service(Plugin plugin) => this.plugin = plugin;

        private Config config => plugin.Config;

        private Translation translation => plugin.Translation;

        public int ActiveCount => anomalies.Count;

        public bool IsSpawned => anomalies.Count > 0;

        public int Spawn()
        {
            Despawn();

            if (config.Locations is null || config.Locations.Count == 0)
                return 0;

            List<SpawnLocation> pool = new List<SpawnLocation>(config.Locations);
            Chance.Shuffle(pool);

            int wanted = Mathf.Clamp(config.SpawnCount, 1, pool.Count);

            foreach (SpawnLocation location in pool)
            {
                if (anomalies.Count >= wanted)
                    break;

                if (location is null)
                    continue;

                Create(location);
            }

            return anomalies.Count;
        }

        public void Despawn()
        {
            try
            {
                foreach (PrimitiveObjectToy anomaly in anomalies)
                    Destroy(anomaly);
            }
            finally
            {
                anomalies.Clear();
                positions.Clear();
            }
        }

        public void Reset()
        {
            Despawn();
            cooldowns.Clear();
        }

        public void ClearCooldowns() => cooldowns.Clear();

        public void Forget(Player player) => cooldowns.Remove(player);

        public bool AllowDrop(Player player, Item item, bool thrown)
        {
            if (!IsSpawned || !Round.IsRoundStarted || player is null || item is null)
                return true;

            if (SpentItems.IsSpent(item.Serial) || (config.PauseDuringGamemodes && WorldEvents.IsPaused))
                return true;

            if (!player.IsAlive || !IsInReach(player) || IsIgnored(player, item))
                return true;

            if (cooldowns.IsOnCooldown(player, config.CooldownSeconds, out _))
                return true;

            cooldowns.Register(player);

            if (!thrown || !config.IgnoreThrownItems)
            {
                if (Chance.Roll(config.CutHandsChance))
                {
                    player.EnableEffect<SeveredHands>(1, config.SeveredHandsDuration);
                    plugin.Hints.Show(player, translation.HandsSevered, config.HintDuration);
                    return true;
                }
            }

            string action = thrown ? translation.ThrownAction : translation.DroppedAction;
            string consumed = NameOf(item.Type);

            if (Chance.Roll(config.DeleteChance))
            {
                if (!TryRemove(player, item))
                    return true;

                plugin.Hints.Show(player, Format(translation.ItemLost, action, consumed, null), config.HintDuration);
                return false;
            }

            RewardEntry entry = Chance.PickWeighted(config.Rewards, reward => reward is null ? 0 : reward.Weight);

            if (entry is null || !InventoryItemLoader.AvailableItems.ContainsKey(entry.Item) || !TryRemove(player, item))
                return true;

            Item granted = player.AddItem(entry.Item);

            if (granted is null)
            {
                plugin.Hints.Show(player, Format(translation.ItemLost, action, consumed, null), config.HintDuration);
                return false;
            }

            Dress(granted);
            PluginBus.Publish(BusTopics.ItemExchanged, plugin.Name, player);
            plugin.Hints.Show(player, Format(translation.ItemExchanged, action, consumed, NameOf(entry.Item)), config.HintDuration);

            return false;
        }

        public string NameOf(ItemType itemType)
        {
            return config.ItemNames.TryGetValue(itemType, out string name) && !string.IsNullOrEmpty(name)
                ? name
                : itemType.ToString();
        }

        // Player.RemoveItem ne dit pas si le retrait a eu lieu : sans ce test, un autre handler qui aurait deja
        // retire l'objet ferait gagner une recompense sans contrepartie.
        private static bool TryRemove(Player player, Item item)
        {
            if (!player.Inventory.UserInventory.Items.ContainsKey(item.Serial))
                return false;

            player.RemoveItem(item);
            return true;
        }

        private void Create(SpawnLocation location)
        {
            Room room = PickRoom(location);

            if (room is null)
            {
                Logger.Warn($"Aucune salle {location.Room} (zone {location.Zone}, forme {location.Shape}) sur cette carte, emplacement SCP-1162 ignore.");
                return;
            }

            Vector3 position = room.Transform.TransformPoint(location.Offset);
            Quaternion rotation = room.Transform.rotation * Quaternion.Euler(location.Rotation);

            PrimitiveObjectToy anomaly = PrimitiveObjectToy.Create(position, rotation, location.Scale, networkSpawn: false);

            anomaly.Type = PrimitiveType.Cylinder;
            anomaly.Flags = PrimitiveFlags.Visible;
            anomaly.Color = config.ResolveColor();
            anomaly.Spawn();

            anomalies.Add(anomaly);
            positions.Add(position);

            Logger.Debug($"SCP-1162 place dans {room.Name} ({room.Zone}, {room.Shape}) a {position}.", config.Debug);
        }

        private Room PickRoom(SpawnLocation location)
        {
            candidates.Clear();

            foreach (Room room in Room.List)
            {
                if (room is not null && location.Matches(room))
                    candidates.Add(room);
            }

            return candidates.Count == 0 ? null : candidates[Random.Range(0, candidates.Count)];
        }

        private bool IsInReach(Player player)
        {
            float squared = config.UseDistance * config.UseDistance;
            Vector3 origin = player.Position;

            foreach (Vector3 position in positions)
            {
                if ((position - origin).sqrMagnitude <= squared)
                    return true;
            }

            return false;
        }

        private bool IsIgnored(Player player, Item item)
        {
            foreach (ItemType ignored in config.IgnoredItems)
            {
                if (ignored == item.Type)
                    return true;
            }

            foreach (PlayerRoles.RoleTypeId ignored in config.IgnoredRoles)
            {
                if (ignored == player.Role)
                    return true;
            }

            return false;
        }

        private void Dress(Item granted)
        {
            if (granted is not FirearmItem firearm)
                return;

            try
            {
                if (config.RandomAttachments)
                    firearm.AttachmentsCode = AttachmentsUtils.GetRandomAttachmentsCode(firearm.Type);

                if (config.FillMagazine)
                    firearm.StoredAmmo = firearm.MaxAmmo;
            }
            catch (System.Exception e)
            {
                Logger.Error($"SCP1162: preparation de {firearm.Type} impossible : {e}");
            }
        }

        private static void Destroy(PrimitiveObjectToy anomaly)
        {
            if (anomaly is null || anomaly.IsDestroyed)
                return;

            try
            {
                anomaly.Destroy();
            }
            catch (System.Exception e)
            {
                Logger.Error($"SCP1162: destruction de l'anomalie impossible : {e}");
            }
        }

        private static string Format(string template, string action, string consumed, string reward)
        {
            return template
                .Replace("{action}", action)
                .Replace("{item}", consumed)
                .Replace("{reward}", reward ?? string.Empty);
        }
    }
}
