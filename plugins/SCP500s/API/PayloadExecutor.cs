using AugatonLib.Effects;
using System.Collections.Generic;
using CustomPlayerEffects;
using Footprinting;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SCP500s.API
{
    public static class PayloadExecutor
    {
        private const float AhpDecay = 1.2f;
        private const float AhpEfficacy = 0.7f;

        private static Config Config => Plugin.Instance.Config;

        public static void Apply(Player player, EffectPayload payload, string sourceKey)
        {
            if (player is null || payload is null)
                return;

            ShowMessage(player, payload.Message);

            if (payload.CureNegativeEffects)
                EffectCatalog.CureNegative(player);

            if (payload.MinimumMaxHealth > 0f && player.MaxHealth < payload.MinimumMaxHealth)
                player.MaxHealth = payload.MinimumMaxHealth;

            if (payload.Heal > 0f)
                player.Heal(payload.Heal);

            if (payload.ArtificialHealth > 0f)
                player.CreateAhpProcess(payload.ArtificialHealth, payload.ArtificialHealth, AhpDecay, AhpEfficacy, 0f, false);

            if (payload.RegenerationPerSecond > 0f && payload.RegenerationSeconds > 0f)
                Plugin.Instance.Regeneration.Start(player, payload.RegenerationPerSecond, Mathf.Min(payload.RegenerationSeconds, Config.MaxEffectDuration));

            if (payload.DropInventory)
                player.DropEverything();

            ApplyEffects(player, payload);
            GrantItems(player, payload);

            PillBehaviours.Run(player, payload, sourceKey);

            ApplyDamage(player, payload);
        }

        public static void ShowMessage(Player player, string message)
        {
            if (player is null || string.IsNullOrEmpty(message))
                return;

            Plugin.Instance.Hints.Show(player, message, Config.HintDuration);

            if (Config.BroadcastMessages)
                player.SendBroadcast(message, (ushort)Mathf.Max(1, Mathf.RoundToInt(Config.HintDuration)));
        }

        public static void ApplyEffects(Player player, EffectPayload payload)
        {
            List<EffectEntry> effects = payload.Effects;

            if (effects is null)
                return;

            foreach (EffectEntry effect in effects)
            {
                if (effect is null || !EffectLookup.TryGet(player, effect.Type, out StatusEffectBase status))
                    continue;

                byte intensity = effect.Intensity == 0 ? (byte)1 : effect.Intensity;
                float duration = effect.Duration <= 0f
                    ? 0f
                    : Mathf.Min(effect.Duration, Config.MaxEffectDuration);

                player.EnableEffect(status, intensity, duration);
            }
        }

        private static void GrantItems(Player player, EffectPayload payload)
        {
            List<ItemType> items = payload.GrantedItems;

            if (items is null || items.Count == 0)
                return;

            if (payload.GrantOneRandomItem)
            {
                Grant(player, items[UnityEngine.Random.Range(0, items.Count)]);
                return;
            }

            foreach (ItemType item in items)
                Grant(player, item);
        }

        private static void Grant(Player player, ItemType type)
        {
            if (type == ItemType.None)
                return;

            if (player.Inventory.UserInventory.Items.Count >= Config.MaxInventorySlots)
            {
                Pickup dropped = Pickup.Create(type, player.Position + (Vector3.up * 0.2f));

                if (dropped is not null)
                    dropped.Base.PreviousOwner = new Footprint(player.ReferenceHub);

                return;
            }

            if (player.AddItem(type) is FirearmItem firearm)
                PillBehaviours.FillAmmo(player, firearm.AmmoType);
        }

        private static void ApplyDamage(Player player, EffectPayload payload)
        {
            if (payload.Damage <= 0f || !player.IsAlive)
                return;

            player.Damage(new PlayerStatsSystem.CustomReasonDamageHandler(Plugin.Instance.Translation.DamageReason, payload.Damage));
        }
    }
}
