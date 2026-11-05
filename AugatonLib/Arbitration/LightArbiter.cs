using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MapGeneration;
using UnityEngine;

namespace AugatonLib.Arbitration
{
    public static class LightArbiter
    {
        private sealed class BlackoutClaim
        {
            public BlackoutClaim(string owner, FacilityZone[] zones, float expiry)
            {
                Owner = owner;
                Zones = zones;
                Expiry = expiry;
            }

            public string Owner { get; }

            public FacilityZone[] Zones { get; }

            public float Expiry { get; }
        }

        private sealed class TintClaim
        {
            public TintClaim(string owner, Color color)
            {
                Owner = owner;
                Color = color;
            }

            public string Owner { get; }

            public Color Color { get; }
        }

        private sealed class RoomTintClaim
        {
            public RoomTintClaim(string owner, Room room, Color color)
            {
                Owner = owner;
                Room = room;
                Color = color;
            }

            public string Owner { get; }

            public Room Room { get; }

            public Color Color { get; }
        }

        public static readonly FacilityZone[] AllZones =
        {
            FacilityZone.Surface,
            FacilityZone.Entrance,
            FacilityZone.HeavyContainment,
            FacilityZone.LightContainment,
        };

        private static readonly List<BlackoutClaim> Blackouts = new List<BlackoutClaim>(4);
        private static readonly List<TintClaim> Tints = new List<TintClaim>(4);
        private static readonly List<RoomTintClaim> RoomTints = new List<RoomTintClaim>(32);

        public static int BlackoutCount
        {
            get
            {
                Purge();
                return Blackouts.Count;
            }
        }

        public static int TintCount => Tints.Count;

        public static void Blackout(string owner, float duration, FacilityZone[] zones = null)
        {
            if (string.IsNullOrEmpty(owner) || float.IsNaN(duration) || duration <= 0f)
                return;

            FacilityZone[] target = Normalize(zones);

            if (target.Length == 0)
                return;

            Purge();

            float now = Time.realtimeSinceStartup;

            Blackouts.RemoveAll(claim => string.Equals(claim.Owner, owner, StringComparison.Ordinal));
            Blackouts.Add(new BlackoutClaim(owner, target, now + duration));

            foreach (FacilityZone zone in target)
                Map.TurnOffLights(LatestExpiry(zone) - now, zone);
        }

        public static void Restore(string owner)
        {
            if (string.IsNullOrEmpty(owner))
                return;

            Purge();

            FacilityZone[] released = null;

            foreach (BlackoutClaim claim in Blackouts)
            {
                if (!string.Equals(claim.Owner, owner, StringComparison.Ordinal))
                    continue;

                released = claim.Zones;
                break;
            }

            if (released is null)
                return;

            Blackouts.RemoveAll(claim => string.Equals(claim.Owner, owner, StringComparison.Ordinal));

            float now = Time.realtimeSinceStartup;

            foreach (FacilityZone zone in released)
            {
                float remaining = LatestExpiry(zone) - now;

                if (remaining > 0f)
                    Map.TurnOffLights(remaining, zone);
                else
                    Map.TurnOnLights(zone);
            }
        }

        public static bool IsBlackedOut(FacilityZone zone)
        {
            Purge();
            return IsClaimed(zone);
        }

        public static void Tint(string owner, Color color)
        {
            if (string.IsNullOrEmpty(owner))
                return;

            Tints.RemoveAll(claim => string.Equals(claim.Owner, owner, StringComparison.Ordinal));
            Tints.Add(new TintClaim(owner, color));

            Map.SetColorOfLights(color);
            RepaintRoomClaims();
        }

        public static void ReleaseTint(string owner)
        {
            if (string.IsNullOrEmpty(owner))
                return;

            if (Tints.RemoveAll(claim => string.Equals(claim.Owner, owner, StringComparison.Ordinal)) == 0)
                return;

            if (Tints.Count == 0)
                Map.ResetColorOfLights();
            else
                Map.SetColorOfLights(Tints[Tints.Count - 1].Color);

            RepaintRoomClaims();
        }

        public static void TintRoom(string owner, Room room, Color color)
        {
            if (string.IsNullOrEmpty(owner) || room is null)
                return;

            RemoveRoomClaim(owner, room);
            RoomTints.Add(new RoomTintClaim(owner, room, color));

            Paint(room, color);
        }

        public static void ReleaseRoomTint(string owner, Room room)
        {
            if (string.IsNullOrEmpty(owner) || room is null)
                return;

            if (RemoveRoomClaim(owner, room) > 0)
                Paint(room, EffectiveColor(room));
        }

        public static void ReleaseRoomTints(string owner)
        {
            if (string.IsNullOrEmpty(owner))
                return;

            List<Room> released = null;

            foreach (RoomTintClaim claim in RoomTints)
            {
                if (!string.Equals(claim.Owner, owner, StringComparison.Ordinal))
                    continue;

                released ??= new List<Room>(8);
                released.Add(claim.Room);
            }

            if (released is null)
                return;

            RoomTints.RemoveAll(claim => string.Equals(claim.Owner, owner, StringComparison.Ordinal));

            foreach (Room room in released)
                Paint(room, EffectiveColor(room));
        }

        public static void ReleaseOwner(string owner)
        {
            Restore(owner);
            ReleaseTint(owner);
            ReleaseRoomTints(owner);
        }

        public static void Clear()
        {
            Blackouts.Clear();

            if (Tints.Count > 0 || RoomTints.Count > 0)
            {
                Tints.Clear();
                RoomTints.Clear();
                Map.ResetColorOfLights();
            }
        }

        internal static void Forget()
        {
            Blackouts.Clear();
            Tints.Clear();
            RoomTints.Clear();
        }

        public static string Describe()
        {
            Purge();

            if (Blackouts.Count == 0 && Tints.Count == 0 && RoomTints.Count == 0)
                return "libre";

            string blackout = Blackouts.Count == 0 ? "aucune" : Join(Blackouts);
            string tint = Tints.Count == 0 ? "aucune" : Tints[Tints.Count - 1].Owner;

            return $"pannes [{blackout}], teinte [{tint}], {RoomTints.Count} salle(s) teintee(s)";
        }

        private static int RemoveRoomClaim(string owner, Room room)
        {
            return RoomTints.RemoveAll(claim =>
                claim.Room == room && string.Equals(claim.Owner, owner, StringComparison.Ordinal));
        }

        private static Color EffectiveColor(Room room)
        {
            for (int i = RoomTints.Count - 1; i >= 0; i--)
            {
                if (RoomTints[i].Room == room)
                    return RoomTints[i].Color;
            }

            return Tints.Count > 0 ? Tints[Tints.Count - 1].Color : Color.clear;
        }

        private static void RepaintRoomClaims()
        {
            foreach (RoomTintClaim claim in RoomTints)
                Paint(claim.Room, claim.Color);
        }

        private static void Paint(Room room, Color color)
        {
            if (room.IsDestroyed)
                return;

            foreach (LightsController controller in room.AllLightControllers)
                controller.OverrideLightsColor = color;
        }

        private static bool IsClaimed(FacilityZone zone)
        {
            foreach (BlackoutClaim claim in Blackouts)
            {
                foreach (FacilityZone claimed in claim.Zones)
                {
                    if (claimed == zone)
                        return true;
                }
            }

            return false;
        }

        private static float LatestExpiry(FacilityZone zone)
        {
            float latest = 0f;

            foreach (BlackoutClaim claim in Blackouts)
            {
                if (claim.Expiry <= latest)
                    continue;

                foreach (FacilityZone claimed in claim.Zones)
                {
                    if (claimed != zone)
                        continue;

                    latest = claim.Expiry;
                    break;
                }
            }

            return latest;
        }

        private static FacilityZone[] Normalize(FacilityZone[] zones)
        {
            if (zones is null || zones.Length == 0)
                return AllZones;

            List<FacilityZone> result = new List<FacilityZone>(zones.Length);

            foreach (FacilityZone zone in zones)
            {
                if (zone == FacilityZone.None)
                    return AllZones;

                if (!result.Contains(zone))
                    result.Add(zone);
            }

            return result.ToArray();
        }

        private static void Purge()
        {
            float now = Time.realtimeSinceStartup;

            Blackouts.RemoveAll(claim => claim.Expiry <= now);
        }

        private static string Join(List<BlackoutClaim> claims)
        {
            string[] owners = new string[claims.Count];

            for (int i = 0; i < claims.Count; i++)
                owners[i] = claims[i].Owner;

            return string.Join(", ", owners);
        }
    }
}
