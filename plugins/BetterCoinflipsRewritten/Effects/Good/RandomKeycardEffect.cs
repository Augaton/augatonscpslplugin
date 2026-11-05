using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class RandomKeycardEffect : ICoinEffect
    {
        private readonly List<WeightedKeycard> keycards;

        private string selectedKeycardName;

        public RandomKeycardEffect(Config config)
        {
            keycards = new List<WeightedKeycard>();

            Translation translation =
                Plugin.Instance.Translation;

            AddKeycard(
                ItemType.KeycardJanitor,
                translation.JanitorKeycardName,
                config.JanitorKeycardChance);

            AddKeycard(
                ItemType.KeycardScientist,
                translation.ScientistKeycardName,
                config.ScientistKeycardChance);

            AddKeycard(
                ItemType.KeycardZoneManager,
                translation.ZoneManagerKeycardName,
                config.ZoneManagerKeycardChance);

            AddKeycard(
                ItemType.KeycardGuard,
                translation.GuardKeycardName,
                config.GuardKeycardChance);

            AddKeycard(
                ItemType.KeycardResearchCoordinator,
                translation.ResearchCoordinatorKeycardName,
                config.ResearchCoordinatorKeycardChance);

            AddKeycard(
                ItemType.KeycardContainmentEngineer,
                translation.ContainmentEngineerKeycardName,
                config.ContainmentEngineerKeycardChance);
        }

        public string Name
        {
            get { return "RandomKeycard"; }
        }

        public string Message
        {
            get
            {
                Translation translation =
                    Plugin.Instance.Translation;

                if (string.IsNullOrEmpty(selectedKeycardName))
                {
                    return translation
                        .RandomKeycardEffectMessage;
                }

                return translation
                    .RandomKeycardSelectedMessage
                    .Replace(
                        "{keycard}",
                        selectedKeycardName);
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   keycards.Count > 0;
        }

        public void Execute(Player player)
        {
            WeightedKeycard selectedKeycard =
                SelectKeycard();

            if (selectedKeycard is null)
            {
                throw new InvalidOperationException(
                    "No weighted keycard is available.");
            }

            API.PickupDrop.Give(player, selectedKeycard.ItemType);
            selectedKeycardName =
                selectedKeycard.DisplayName;
        }

        private void AddKeycard(
            ItemType itemType,
            string displayName,
            int weight)
        {
            if (weight <= 0)
                return;

            keycards.Add(
                new WeightedKeycard(
                    itemType,
                    displayName,
                    weight));
        }

        private WeightedKeycard SelectKeycard()
        {
            int totalWeight = 0;

            foreach (WeightedKeycard keycard in keycards)
                totalWeight += keycard.Weight;

            if (totalWeight <= 0)
                return null;

            int selectedNumber =
                API.Rng.NextWeight(totalWeight);

            foreach (WeightedKeycard keycard in keycards)
            {
                selectedNumber -= keycard.Weight;

                if (selectedNumber <= 0)
                    return keycard;
            }

            return keycards[keycards.Count - 1];
        }

        private sealed class WeightedKeycard
        {
            public WeightedKeycard(
                ItemType itemType,
                string displayName,
                int weight)
            {
                ItemType = itemType;
                DisplayName = displayName;
                Weight = weight;
            }

            public ItemType ItemType { get; private set; }

            public string DisplayName { get; private set; }

            public int Weight { get; private set; }
        }
    }
}
