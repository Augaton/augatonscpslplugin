using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MapGeneration;
using UnityEngine;

namespace AugatonLib.World
{
    /// <summary>
    /// Emplacements du loot de la carte, pour faire reapparaitre des objets a des endroits credibles.
    /// A relever en debut de round, avant que les joueurs ne ramassent le loot.
    /// </summary>
    public sealed class LootSpots
    {
        private const int Attempts = 24;

        private readonly List<LootSpot> spots = new List<LootSpot>(256);

        public int Count => spots.Count;

        public void Collect()
        {
            spots.Clear();

            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup is null || !pickup.IsSpawned)
                    continue;

                Room room = pickup.Room;

                if (room is not null && IsSpawnZone(room.Zone))
                    spots.Add(new LootSpot(pickup.Position, room.Zone));
            }
        }

        public void Clear() => spots.Clear();

        public bool TryPick(Func<Vector3, bool> isUnavailable, out Vector3 position)
        {
            return TryPick(null, isUnavailable, out position, out _);
        }

        public bool TryPick(List<FacilityZone> zones, Func<Vector3, bool> isUnavailable, out Vector3 position, out FacilityZone zone)
        {
            if (spots.Count > 0)
            {
                for (int attempt = 0; attempt < Attempts; attempt++)
                {
                    LootSpot spot = spots[UnityEngine.Random.Range(0, spots.Count)];

                    if (!IsSafe(spot.Zone) || (zones is not null && !zones.Contains(spot.Zone)) || isUnavailable(spot.Position))
                        continue;

                    position = spot.Position + (Vector3.up * 0.1f);
                    zone = spot.Zone;
                    return true;
                }
            }

            position = default;
            zone = FacilityZone.None;
            return false;
        }

        public static bool IsNearLivingPlayer(Vector3 position, float distance)
        {
            float squared = distance * distance;

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && player.IsAlive && (player.Position - position).sqrMagnitude < squared)
                    return true;
            }

            return false;
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

        private readonly struct LootSpot
        {
            public LootSpot(Vector3 position, FacilityZone zone)
            {
                Position = position;
                Zone = zone;
            }

            public Vector3 Position { get; }

            public FacilityZone Zone { get; }
        }
    }
}
