using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SCP500s.API
{
    public abstract class PillBase
    {
        private const ItemType PillItem = ItemType.SCP500;
        public const float PickupWeight = 1.5f;
        private const float GiveMessageDelay = 0.05f;

        private bool usesSpawnPoints;

        protected PillBase(PillDefinition definition, Color glowColor)
        {
            Definition = definition;
            GlowColor = glowColor;
            IsRandomOnly = IsListedAsRandomOnly(definition.Key);
        }

        public uint Id => Definition.Id;

        public string Name => Definition.DisplayName;

        public string Description => Definition.Description;

        public PillDefinition Definition { get; }

        public Color GlowColor { get; }

        public bool IsRandomOnly { get; }

        public string Key => Definition.Key;

        public int SpawnWeight => IsRandomOnly ? 0 : Definition.SpawnWeight;

        public int MaxPerRound => Definition.MaxPerRound;

        protected static Config Config => Plugin.Instance.Config;

        protected static Translation Text => Plugin.Instance.Translation;

        public abstract void Consume(Player player);

        public Pickup Spawn(Vector3 position)
        {
            Pickup pickup = Pickup.Create(PillItem, position);

            if (pickup is null)
                return null;

            pickup.Weight = PickupWeight;
            Plugin.Instance.Registry.Track(pickup.Serial, this);
            return pickup;
        }

        public bool Give(Player player)
        {
            Item item = player.AddItem(PillItem);

            if (item is null)
                return false;

            Plugin.Instance.Registry.Track(item.Serial, this);
            Plugin.Instance.Delayed.Run(GiveMessageDelay, () => ShowItemInfo(player));
            return true;
        }

        public void BuildSpawnPoints()
        {
            usesSpawnPoints = !IsRandomOnly && Definition.SpawnLocations is not null && Definition.SpawnLocations.Count > 0;
        }

        public void ClearSpawnPoints() => usesSpawnPoints = false;

        public void SpawnAtLocations()
        {
            if (!usesSpawnPoints)
                return;

            int limit = Mathf.Max(0, Definition.SpawnLimit);
            int placed = 0;

            foreach (SpawnLocationType location in Definition.SpawnLocations)
            {
                if (limit != 0 && placed >= limit)
                    return;

                if (SpawnLocations.TryGetPosition(location, out Vector3 position) && Spawn(position) is not null)
                    placed++;
            }
        }

        public void ShowItemInfo(Player player)
        {
            if (player is null || !Config.ShowItemInfo || string.IsNullOrEmpty(Text.ItemInfo))
                return;

            string text = Text.ItemInfo
                .Replace("{name}", Name ?? string.Empty)
                .Replace("{description}", Description ?? string.Empty)
                .Replace("{color}", "#" + ColorUtility.ToHtmlStringRGB(GlowColor))
                .TrimEnd('\n');

            Plugin.Instance.Hints.Show(player, text, Config.HintDuration);
        }

        private static bool IsListedAsRandomOnly(string key)
        {
            List<string> keys = Plugin.Instance?.Config?.RandomOnlyPills;

            if (keys is null || string.IsNullOrEmpty(key))
                return false;

            foreach (string listed in keys)
            {
                if (string.Equals(listed, key, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
