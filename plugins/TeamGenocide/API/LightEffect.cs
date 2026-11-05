using System;
using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Arbitration;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace TeamGenocide.API
{
    public sealed class LightEffect
    {
        private const string Owner = "TeamGenocide";

        [Description("Couleur appliquee aux lumieres, au format hexadecimal.")]
        public string Color { get; set; } = "#000000";

        [Description("Zones affectees : LightContainment, HeavyContainment, Entrance, Surface, Other. None = toute l'installation.")]
        public List<FacilityZone> Zones { get; set; } = new List<FacilityZone>
        {
            FacilityZone.Surface,
            FacilityZone.Entrance,
            FacilityZone.HeavyContainment,
            FacilityZone.LightContainment,
        };

        [Description("Duree de l'effet en secondes.")]
        public float Duration { get; set; } = 5f;

        public CoroutineHandle Apply()
        {
            if (Zones is null)
                return default;

            if (!ColorUtility.TryParseHtmlString(Color, out Color color))
                color = UnityEngine.Color.black;

            if (CoversFacility())
            {
                LightArbiter.Tint(Owner, color);
                return Timing.CallDelayed(Mathf.Max(0.5f, Duration), () => Guard(() => LightArbiter.ReleaseTint(Owner)));
            }

            List<Room> affected = new List<Room>(32);

            foreach (FacilityZone zone in Zones)
            {
                foreach (Room room in Room.Get(zone))
                {
                    LightArbiter.TintRoom(Owner, room, color);
                    affected.Add(room);
                }
            }

            if (affected.Count == 0)
                return default;

            return Timing.CallDelayed(Mathf.Max(0.5f, Duration), () => Guard(() => ResetRooms(affected)));
        }

        public static void Release() => LightArbiter.ReleaseOwner(Owner);

        private bool CoversFacility()
        {
            if (Zones.Contains(FacilityZone.None))
                return true;

            foreach (FacilityZone zone in LightArbiter.AllZones)
            {
                if (!Zones.Contains(zone))
                    return false;
            }

            return true;
        }

        private static void ResetRooms(List<Room> rooms)
        {
            foreach (Room room in rooms)
                LightArbiter.ReleaseRoomTint(Owner, room);
        }

        private static void Guard(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error($"LightEffect: {e}");
            }
        }
    }
}
