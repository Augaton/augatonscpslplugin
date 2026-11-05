using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public sealed class ContinuousSpawnService
    {
        private const int SpotAttempts = 24;

        private readonly PillRegistry registry;
        private readonly LootReplacementService spawner;
        private readonly List<SpawnSpot> spots = new List<SpawnSpot>(256);
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

            CollectSpots();

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
                yield return Timing.WaitForSeconds(Config.ContinuousSpawnIntervalSeconds);

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

            int missing = Mathf.Min(Config.ContinuousSpawnTarget - CountPillsOnGround(), Config.ContinuousSpawnBatch);

            for (int i = 0; i < missing; i++)
            {
                if (!TryPickSpot(out Vector3 position))
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

        private bool TryPickSpot(out Vector3 position)
        {
            for (int attempt = 0; attempt < SpotAttempts; attempt++)
            {
                SpawnSpot spot = spots[UnityEngine.Random.Range(0, spots.Count)];

                if (!IsSafe(spot.Zone) || IsCrowded(spot.Position))
                    continue;

                position = spot.Position + (Vector3.up * 0.1f);
                return true;
            }

            position = default;
            return false;
        }

        private bool IsCrowded(Vector3 position)
        {
            float pillSpacing = Config.ContinuousSpawnSpacing * Config.ContinuousSpawnSpacing;

            foreach (Vector3 pill in pillsOnGround)
            {
                if ((pill - position).sqrMagnitude < pillSpacing)
                    return true;
            }

            float playerDistance = Config.ContinuousSpawnPlayerDistance * Config.ContinuousSpawnPlayerDistance;

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || !player.IsAlive)
                    continue;

                if ((player.Position - position).sqrMagnitude < playerDistance)
                    return true;
            }

            return false;
        }

        private void CollectSpots()
        {
            spots.Clear();

            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup is null || !pickup.IsSpawned)
                    continue;

                Room room = pickup.Room;

                if (room is null || !IsSpawnZone(room.Zone))
                    continue;

                spots.Add(new SpawnSpot(pickup.Position, room.Zone));
            }
        }

        private static bool IsSpawnZone(FacilityZone zone)
        {
            return zone == FacilityZone.LightContainment
                || zone == FacilityZone.HeavyContainment
                || zone == FacilityZone.Entrance
                || zone == FacilityZone.Surface;
        }

        private static bool IsSafe(FacilityZone zone)
        {
            if (Warhead.IsDetonated && zone != FacilityZone.Surface)
                return false;

            return !(Decontamination.IsDecontaminating && zone == FacilityZone.LightContainment);
        }

        private readonly struct SpawnSpot
        {
            public SpawnSpot(Vector3 position, FacilityZone zone)
            {
                Position = position;
                Zone = zone;
            }

            public Vector3 Position { get; }

            public FacilityZone Zone { get; }
        }
    }
}
