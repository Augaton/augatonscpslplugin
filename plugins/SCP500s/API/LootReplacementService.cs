using System;
using System.Collections.Generic;
using AugatonLib.Random;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public sealed class LootReplacementService
    {
        // Un remplacement detruit un objet, en cree un autre et allume son halo : au debut du round,
        // 40 d'un coup enverraient une centaine de messages reseau a chaque client dans le meme tick.
        private const int ReplacementsPerFrame = 5;

        private readonly PillRegistry registry;

        private readonly List<Pickup> candidates = new List<Pickup>(128);
        private readonly List<PillBase> pool = new List<PillBase>(48);
        private readonly Dictionary<uint, int> placedThisRound = new Dictionary<uint, int>();

        private PillBase randomPill;
        private CoroutineHandle loop;
        private bool running;
        private int coins;
        private int pills;
        private int randoms;

        public LootReplacementService(PillRegistry registry) => this.registry = registry;

        private static Config Config => Plugin.Instance.Config;

        public void Reset()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;
            placedThisRound.Clear();
            candidates.Clear();
            pool.Clear();
            randomPill = null;
        }

        public void Run()
        {
            try
            {
                if (Config.SpawnMode == SpawnMode.SpawnPoints)
                    return;

                placedThisRound.Clear();
                CollectCandidates();

                if (candidates.Count == 0)
                {
                    Logger.Warn("Aucun objet remplacable trouve sur la carte. Verifier replaceable_source_items.");
                    return;
                }

                Shuffle(candidates);

                randomPill = registry.Find(Config.RandomPillKey);
                running = true;
                loop = Timing.RunCoroutine(ReplaceCandidates());
            }
            catch (Exception e)
            {
                Logger.Error($"LootReplacementService.Run: {e}");
            }
        }

        private IEnumerator<float> ReplaceCandidates()
        {
            int limit = Config.MaxReplacementsPerRound > 0
                ? Config.MaxReplacementsPerRound
                : candidates.Count;

            coins = 0;
            pills = 0;
            randoms = 0;

            for (int i = 0; i < candidates.Count && pills + coins < limit; i++)
            {
                int before = pills + coins;

                ReplaceOne(candidates[i]);

                if (pills + coins > before && (pills + coins) % ReplacementsPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            running = false;
            candidates.Clear();
            pool.Clear();
            randomPill = null;

            Logger.Info($"Remplacement du loot termine : {pills} pilules dont {randoms} aleatoires, et {coins} pieces placees.");
        }

        private void ReplaceOne(Pickup source)
        {
            try
            {
                if (source is null || source.IsDestroyed || !source.IsSpawned)
                    return;

                if (UnityEngine.Random.Range(0f, 100f) >= Config.ReplacementChancePercent)
                    return;

                if (UnityEngine.Random.Range(0f, 100f) < Config.CoinSharePercent)
                {
                    if (SpawnCoin(source))
                        coins++;

                    return;
                }

                if (SpawnPill(source, out bool wasRandom))
                {
                    pills++;

                    if (wasRandom)
                        randoms++;
                }
                else if (SpawnCoin(source))
                {
                    coins++;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"LootReplacementService.ReplaceOne: {e}");
            }
        }

        public PillBase SpawnAt(Vector3 position, float bonusRandomShare = 0f)
        {
            PillBase pill = PickPill(bonusRandomShare);

            if (pill is null || pill.Spawn(position) is null)
                return null;

            placedThisRound.TryGetValue(pill.Id, out int placed);
            placedThisRound[pill.Id] = placed + 1;
            return pill;
        }

        public bool GiveTo(Player player)
        {
            PillBase pill = PickPill(0f);

            return pill is not null && pill.Give(player);
        }

        private PillBase PickPill(float bonusRandomShare)
        {
            PillBase random = registry.Find(Config.RandomPillKey);
            bool useRandom = random is not null && Chance.Roll(Config.RandomPillSharePercent + bonusRandomShare);

            return useRandom ? random : PickWeightedPill() ?? random;
        }

        private void CollectCandidates()
        {
            candidates.Clear();

            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup is null || !pickup.IsSpawned)
                    continue;

                if (!Config.ReplaceableSourceItems.Contains(pickup.Type))
                    continue;

                candidates.Add(pickup);
            }
        }

        private bool SpawnCoin(Pickup source)
        {
            Vector3 position = source.Position;
            Quaternion rotation = source.Rotation;

            source.Destroy();
            Pickup.Create(ItemType.Coin, position, rotation);
            return true;
        }

        private bool SpawnPill(Pickup source, out bool wasRandom)
        {
            wasRandom = randomPill is not null && Chance.Roll(Config.RandomPillSharePercent);

            PillBase pill = wasRandom ? randomPill : PickWeightedPill();

            if (pill is null)
            {
                wasRandom = false;
                return false;
            }

            if (pill.Spawn(source.Position) is null)
            {
                wasRandom = false;
                return false;
            }

            source.Destroy();

            placedThisRound.TryGetValue(pill.Id, out int placed);
            placedThisRound[pill.Id] = placed + 1;
            return true;
        }

        private PillBase PickWeightedPill()
        {
            pool.Clear();

            int total = 0;

            foreach (PillBase pill in registry.Pills)
            {
                if (pill.SpawnWeight <= 0 || pill == randomPill)
                    continue;

                if (pill.MaxPerRound > 0)
                {
                    placedThisRound.TryGetValue(pill.Id, out int placed);

                    if (placed >= pill.MaxPerRound)
                        continue;
                }

                pool.Add(pill);
                total += pill.SpawnWeight;
            }

            if (pool.Count == 0 || total <= 0)
                return null;

            int roll = UnityEngine.Random.Range(0, total);

            foreach (PillBase pill in pool)
            {
                roll -= pill.SpawnWeight;

                if (roll < 0)
                    return pill;
            }

            return pool[pool.Count - 1];
        }

        private static void Shuffle(List<Pickup> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                Pickup temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
