using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace BetterCoinflipsRewritten
{
    public sealed class CoinSpawnService
    {
        private readonly Plugin plugin;

        public CoinSpawnService(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void SpawnMapCoins()
        {
            ReplaceLootWithCoins();
            AddBonusCoins();
        }

        private void ReplaceLootWithCoins()
        {
            if (!plugin.Config.SpawnCoinsOnMap)
                return;

            int requestedAmount = plugin.Config.MapCoinAmount;

            if (requestedAmount <= 0)
                return;

            List<Pickup> candidates = new List<Pickup>(64);

            foreach (Pickup pickup in Pickup.List)
            {
                if (!IsValidReplacement(pickup))
                    continue;

                candidates.Add(pickup);
            }

            Shuffle(candidates);

            int amountToReplace =
                Math.Min(requestedAmount, candidates.Count);

            int spawnedCoins = 0;

            for (int i = 0; i < amountToReplace; i++)
            {
                Pickup oldPickup = candidates[i];

                if (oldPickup is null ||
                    !oldPickup.IsSpawned)
                {
                    continue;
                }

                Vector3 position =
                    oldPickup.Position + (Vector3.up * 0.03f);

                Quaternion rotation = oldPickup.Rotation;

                try
                {
                    if (!SpawnCoin(position, rotation))
                        continue;

                    Logger.Debug(
                        "[BetterCoinflipsRewritten] Remplace " +
                        oldPickup.Type +
                        " par une piece dans la salle " +
                        GetRoomName(oldPickup) +
                        ".",
                        plugin.Config.Debug);

                    oldPickup.Destroy();
                    spawnedCoins++;
                }
                catch (Exception exception)
                {
                    Logger.Error(
                        "[BetterCoinflipsRewritten] Piece impossible a poser : " +
                        exception);
                }
            }

            Logger.Info(
                "[BetterCoinflipsRewritten] Pose " +
                spawnedCoins +
                "/" +
                requestedAmount +
                " pieces sur la carte.");
        }

        private void AddBonusCoins()
        {
            if (!plugin.Config.AddBonusCoinsToLoot)
                return;

            int requestedAmount = plugin.Config.BonusCoinAmount;

            if (requestedAmount <= 0)
                return;

            List<ItemType> sourceItems =
                plugin.Config.BonusCoinSourceItems;

            if (sourceItems is null || sourceItems.Count == 0)
            {
                Logger.Warn(
                    "[BetterCoinflipsRewritten] bonus_coin_source_items est " +
                    "vide : aucune piece en plus dans le loot.");

                return;
            }

            List<Pickup> anchors = new List<Pickup>(64);

            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup is null ||
                    !pickup.IsSpawned ||
                    pickup.Type == ItemType.Coin)
                {
                    continue;
                }

                if (!sourceItems.Contains(pickup.Type))
                    continue;

                anchors.Add(pickup);
            }

            if (anchors.Count == 0)
            {
                Logger.Warn(
                    "[BetterCoinflipsRewritten] Aucun objet de " +
                    "bonus_coin_source_items trouve sur la carte.");

                return;
            }

            Shuffle(anchors);

            Dictionary<ItemType, int> perType =
                new Dictionary<ItemType, int>(sourceItems.Count);

            int perTypeLimit = plugin.Config.BonusCoinsPerSourceType;
            int spawnedCoins = 0;

            foreach (Pickup anchor in anchors)
            {
                if (spawnedCoins >= requestedAmount)
                    break;

                if (anchor is null || !anchor.IsSpawned)
                    continue;

                perType.TryGetValue(anchor.Type, out int placed);

                if (perTypeLimit > 0 && placed >= perTypeLimit)
                    continue;

                try
                {
                    if (!SpawnCoin(anchor.Position + BonusCoinOffset(), anchor.Rotation))
                        continue;
                }
                catch (Exception exception)
                {
                    Logger.Error(
                        "[BetterCoinflipsRewritten] Piece bonus impossible " +
                        "a ajouter : " +
                        exception);

                    continue;
                }

                perType[anchor.Type] = placed + 1;
                spawnedCoins++;

                Logger.Debug(
                    "[BetterCoinflipsRewritten] Piece ajoutee a cote de " +
                    anchor.Type +
                    " dans la salle " +
                    GetRoomName(anchor) +
                    ".",
                    plugin.Config.Debug);
            }

            Logger.Info(
                "[BetterCoinflipsRewritten] Ajoute " +
                spawnedCoins +
                "/" +
                requestedAmount +
                " pieces en plus au loot.");
        }

        // Cette surcharge de Pickup.Create fait deja le spawn reseau : un Spawn() de plus ferait
        // seulement journaliser "was already spawned" par Mirror.
        internal static bool SpawnCoin(Vector3 position, Quaternion rotation)
        {
            if (Pickup.Create(ItemType.Coin, position, rotation) is not null)
                return true;

            Logger.Error("[BetterCoinflipsRewritten] Le jeu a refuse de creer une piece.");
            return false;
        }

        private static Vector3 BonusCoinOffset()
        {
            return new Vector3(
                (API.Rng.NextUnit() - 0.5f) * 0.4f,
                0.05f,
                (API.Rng.NextUnit() - 0.5f) * 0.4f);
        }

        private bool IsValidReplacement(Pickup pickup)
        {
            if (pickup is null ||
                !pickup.IsSpawned ||
                pickup.Type == ItemType.Coin)
            {
                return false;
            }

            switch (pickup.Type)
            {
                case ItemType.Medkit:
                    return plugin.Config.ReplaceMedkitsWithCoins;

                case ItemType.Painkillers:
                    return plugin.Config.ReplacePainkillersWithCoins;

                case ItemType.Flashlight:
                    return plugin.Config.ReplaceFlashlightsWithCoins;

                case ItemType.KeycardJanitor:
                case ItemType.KeycardScientist:
                case ItemType.KeycardZoneManager:
                    return plugin.Config.ReplaceWeakKeycardsWithCoins;

                default:
                    return false;
            }
        }

        private void Shuffle(List<Pickup> pickups)
        {
            for (int i = pickups.Count - 1; i > 0; i--)
            {
                int selectedIndex = API.Rng.Next(0, i + 1);

                Pickup temporary = pickups[i];
                pickups[i] = pickups[selectedIndex];
                pickups[selectedIndex] = temporary;
            }
        }

        private string GetRoomName(Pickup pickup)
        {
            Room room = pickup?.Room;

            return room is null ? "Unknown" : room.Name.ToString();
        }
    }
}
