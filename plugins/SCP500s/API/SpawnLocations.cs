using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace SCP500s.API
{
    public static class SpawnLocations
    {
        public static bool TryGetPosition(SpawnLocationType location, out Vector3 position)
        {
            position = default;

            string doorName = DoorName(location);

            if (doorName is null || !DoorNametagExtension.NamedDoors.TryGetValue(doorName, out DoorNametagExtension door))
                return false;

            Transform transform = door.transform;

            position = transform.position + (Vector3.up * 1.5f) + (transform.forward * (IsReversed(location) ? -1.5f : 3f));
            return true;
        }

        private static bool IsReversed(SpawnLocationType location)
        {
            switch (location)
            {
                case SpawnLocationType.InsideHczArmory:
                case SpawnLocationType.Inside079First:
                case SpawnLocationType.InsideHidLab:
                case SpawnLocationType.Inside173Gate:
                case SpawnLocationType.InsideGateA:
                case SpawnLocationType.InsideGateB:
                case SpawnLocationType.InsideLczWc:
                case SpawnLocationType.InsideGr18:
                case SpawnLocationType.Inside914:
                case SpawnLocationType.Inside049Armory:
                case SpawnLocationType.InsideLczCafe:
                case SpawnLocationType.Inside939Cryo:
                    return true;
                default:
                    return false;
            }
        }

        private static string DoorName(SpawnLocationType location)
        {
            switch (location)
            {
                case SpawnLocationType.Inside330: return "330";
                case SpawnLocationType.Inside096: return "096";
                case SpawnLocationType.Inside914: return "914";
                case SpawnLocationType.InsideHidChamber: return "HID_CHAMBER";
                case SpawnLocationType.InsideGr18: return "GR18";
                case SpawnLocationType.InsideGateA: return "GATE_A";
                case SpawnLocationType.InsideGateB: return "GATE_B";
                case SpawnLocationType.InsideLczWc: return "LCZ_WC";
                case SpawnLocationType.InsideLczCafe: return "LCZ_CAFE";
                case SpawnLocationType.Inside173Gate: return "173_GATE";
                case SpawnLocationType.InsideIntercom: return "INTERCOM";
                case SpawnLocationType.InsideHidLab: return "HID_LAB";
                case SpawnLocationType.Inside079First: return "079_FIRST";
                case SpawnLocationType.Inside330Chamber: return "330_CHAMBER";
                case SpawnLocationType.Inside049Armory: return "049_ARMORY";
                case SpawnLocationType.Inside173Armory: return "173_ARMORY";
                case SpawnLocationType.Inside173Bottom: return "173_BOTTOM";
                case SpawnLocationType.InsideLczArmory: return "LCZ_ARMORY";
                case SpawnLocationType.InsideHczArmory: return "HCZ_ARMORY";
                case SpawnLocationType.InsideSurfaceNuke: return "SURFACE_NUKE";
                case SpawnLocationType.Inside079Secondary: return "079_SECOND";
                case SpawnLocationType.Inside173Connector: return "173_CONNECTOR";
                case SpawnLocationType.InsideEscapePrimary: return "ESCAPE_PRIMARY";
                case SpawnLocationType.InsideEscapeSecondary: return "ESCAPE_SECONDARY";
                case SpawnLocationType.InsideGr18Glass: return "GR18_INNER";
                case SpawnLocationType.Inside106Primary: return "106_PRIMARY";
                case SpawnLocationType.Inside106Secondary: return "106_SECONDARY";
                case SpawnLocationType.Inside939Cryo: return "939_CRYO";
                case SpawnLocationType.Inside079Armory: return "079_ARMORY";
                case SpawnLocationType.Inside127Lab: return "HCZ_127_LAB";
                default: return null;
            }
        }
    }
}
