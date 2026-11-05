using System;
using System.Collections.Generic;
using AugatonLib.Chaos;
using AugatonLib.World;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public sealed class ContinuousSpawnService
    {
        private const float MinIntervalSeconds = 30f;

        private readonly PillRegistry registry;
        private readonly LootReplacementService spawner;
        private readonly LootSpots spots = new LootSpots();
        private readonly List<Vector3> pillsOnGround = new List<Vector3>(32);

        private CoroutineHandle loop;
        private bool running;

        public ContinuousSpawnService(PillRegistry registry, LootReplacementService spawner)
        {
            this.registry = registry;
            this.spawner = spawner;
        }

        private static Config Config => Plugin.Instance.Config;

        public int SpotCount => spots.Count;

        public void Start()
        {
            Stop();

            if (!Config.ContinuousSpawn || Config.ContinuousSpawnTarget <= 0)
                return;

            spots.Collect();

            if (spots.Count == 0)
            {
                Logger.Warn("Aucun emplacement de loot trouve, pas d'apparition continue de pilules pour ce round.");
                return;
            }

            running = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public void Stop()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;
            spots.Clear();
            pillsOnGround.Clear();
        }

        private IEnumerator<float> Loop()
        {
            while (running)
            {
                yield return Timing.WaitForSeconds(CurrentInterval());

                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"ContinuousSpawnService.Tick: {e}");
                }
            }
        }

        private void Tick()
        {
            if (!Round.IsRoundStarted || Round.IsRoundEnded)
                return;

            int level = CurrentLevel();
            int target = Mathf.Min(Escalation.Grow(Config.ContinuousSpawnTarget, Config.ChaosTargetPerLevel, level), Config.MaxPillsOnGround);
            int batch = Escalation.Grow(Config.ContinuousSpawnBatch, Config.ChaosBatchPerLevel, level);

            SpawnPills(Mathf.Min(target - CountPillsOnGround(), batch));
        }

        private static int CurrentLevel()
        {
            return Config.ChaosEscalation
                ? Escalation.Level(Round.Duration.TotalSeconds, Config.ChaosStepSeconds, Config.ChaosMaxLevel)
                : 0;
        }

        private static float CurrentInterval()
        {
            return Escalation.Shrink(Config.ContinuousSpawnIntervalSeconds, Config.ChaosIntervalPerLevel, CurrentLevel(), MinIntervalSeconds);
        }

        private void SpawnPills(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!spots.TryPick(IsCrowded, out Vector3 position))
                    return;

                PillBase pill = spawner.SpawnAt(position);

                if (pill is null)
                    return;

                pillsOnGround.Add(position);

                Logger.Debug($"Pilule {pill.Key} apparue en continu en {position}.", Config.Debug);
            }
        }

        private int CountPillsOnGround()
        {
            pillsOnGround.Clear();

            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup is null || !pickup.IsSpawned || pickup.Type != ItemType.SCP500)
                    continue;

                if (!registry.TryGet(pickup, out _))
                    continue;

                pillsOnGround.Add(pickup.Position);
            }

            return pillsOnGround.Count;
        }

        private bool IsCrowded(Vector3 position)
        {
            float pillSpacing = Config.ContinuousSpawnSpacing * Config.ContinuousSpawnSpacing;

            foreach (Vector3 pill in pillsOnGround)
            {
                if ((pill - position).sqrMagnitude < pillSpacing)
                    return true;
            }

            return LootSpots.IsNearLivingPlayer(position, Config.ContinuousSpawnPlayerDistance);
        }
    }
}
