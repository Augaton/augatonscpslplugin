using System.Collections.Generic;
using AugatonLib.Arbitration;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles.FirstPersonControl;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class ClosingZones : GamemodeBase
    {
        private static readonly FacilityZone[] Order =
        {
            FacilityZone.LightContainment,
            FacilityZone.HeavyContainment,
            FacilityZone.Entrance,
        };

        private readonly HashSet<FacilityZone> condemned = new HashSet<FacilityZone>();

        public override string Key => "closingzones";

        public override string DisplayName => "Zones condamnees";

        public override bool CanStart(int playerCount) => playerCount >= 4;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            Announce(Settings.Description);

            int next = 0;
            float condemnAt = Time.time + Config.ClosingZoneSeconds;

            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                if (next < Order.Length && Time.time >= condemnAt)
                {
                    Condemn(Order[next++]);
                    condemnAt += Config.ClosingZoneSeconds;
                }

                HurtOccupants();
            }
        }

        public override void Stop()
        {
            LightArbiter.ReleaseRoomTints(Interop.Owner);
            condemned.Clear();
        }

        private void Condemn(FacilityZone zone)
        {
            if (!ColorUtility.TryParseHtmlString(Config.ClosingZoneColor, out Color color))
                color = Color.red;

            foreach (Room room in Room.Get(zone))
                LightArbiter.TintRoom(Interop.Owner, room, color);

            condemned.Add(zone);
            Announce(Text.ClosingZoneCondemned.Replace("%ZONE%", ZoneName(zone)));
        }

        private static string ZoneName(FacilityZone zone)
        {
            switch (zone)
            {
                case FacilityZone.LightContainment: return Text.ClosingZoneLight;
                case FacilityZone.HeavyContainment: return Text.ClosingZoneHeavy;
                default: return Text.ClosingZoneEntrance;
            }
        }

        // En pourcentage de la vie maximale : une valeur fixe tuerait un humain et chatouillerait un SCP.
        private void HurtOccupants()
        {
            foreach (Player player in Player.ReadyList)
            {
                if (player.RoleBase is IFpcRole && player.IsAlive && !player.IsTutorial && condemned.Contains(player.Zone))
                    player.Damage(player.MaxHealth * Config.ClosingZoneDamagePercent / 100f, Text.ClosingZoneDeath);
            }
        }
    }
}
