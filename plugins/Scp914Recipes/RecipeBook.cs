using System;
using AugatonLib.Bus;
using AugatonLib.Random;
using AugatonLib.Runtime;
using InventorySystem;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Features.Wrappers;
using Scp914;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace Scp914Recipes
{
    public sealed class RecipeBook
    {
        private readonly Plugin plugin;

        public RecipeBook(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        public string CheckRecipes()
        {
            foreach (Recipe recipe in Config.Recipes)
            {
                if (recipe is null || !InventoryItemLoader.AvailableItems.ContainsKey(recipe.Output))
                    return $"objet inconnu : {recipe?.Output}";
            }

            return null;
        }

        public void OnProcessingPickup(Scp914ProcessingPickupEventArgs ev)
        {
            try
            {
                if (ev?.Pickup is null || !ev.IsAllowed || !TryRoll(ev.Pickup.Type, ev.KnobSetting, out ItemType output))
                    return;

                ev.IsAllowed = false;
                ev.Pickup.Destroy();
                Spawn(output, ev.NewPosition);
            }
            catch (Exception e)
            {
                Logger.Error($"OnProcessingPickup: {e}");
            }
        }

        public void OnProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || ev.Item is null || !ev.IsAllowed || !TryRoll(ev.Item.Type, ev.KnobSetting, out ItemType output))
                    return;

                ev.IsAllowed = false;
                ev.Player.RemoveItem(ev.Item);
                Give(ev.Player, output);
            }
            catch (Exception e)
            {
                Logger.Error($"OnProcessingInventoryItem: {e}");
            }
        }

        private bool TryRoll(ItemType input, Scp914KnobSetting setting, out ItemType output)
        {
            output = ItemType.None;

            if (Config.PauseDuringGamemodes && WorldEvents.IsPaused)
                return false;

            foreach (Recipe recipe in Config.Recipes)
            {
                if (recipe is null || recipe.Input != input || recipe.Setting != setting)
                    continue;

                if (!InventoryItemLoader.AvailableItems.ContainsKey(recipe.Output) || !Chance.Roll(recipe.Chance))
                    continue;

                output = recipe.Output;
                return true;
            }

            return false;
        }

        private void Spawn(ItemType output, Vector3 position)
        {
            if (output == ItemType.SCP500 && PluginBus.Publish(BusTopics.PillSpawnRequested, plugin.Name, position) > 0)
                return;

            Pickup.Create(output, position, Quaternion.identity);
        }

        private void Give(Player player, ItemType output)
        {
            if (output == ItemType.SCP500 && PluginBus.Publish(BusTopics.PillGiveRequested, plugin.Name, player) > 0)
                return;

            if (player.AddItem(output) is null)
                Pickup.Create(output, player.Position, Quaternion.identity);
        }
    }
}
