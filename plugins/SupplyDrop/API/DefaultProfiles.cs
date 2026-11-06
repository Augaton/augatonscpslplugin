using MapGeneration;
using System.Collections.Generic;
using PlayerRoles;

namespace SupplyDrop.API
{
    public static class DefaultProfiles
    {
        public static List<DropProfile> Create()
        {
            return new List<DropProfile>
            {
                new DropProfile
                {
                    Key = "mtf",
                    GeneratorsTrigger = 3,
                    DisplayName = "Largage MTF",
                    Broadcast = "<size=35><i><color=#0080FF>Un helicoptere MTF</color> <color=#5c5c5c>vient de larguer</color> <color=#7a7a7a>du ravitaillement</color></i></size>",
                    BroadcastDuration = 10,
                    CassieAnnouncement = "mtf supply drop incoming",
                    CassieSubtitles = "Largage de ravitaillement MTF en approche",
                    FirstDropSeconds = 600f,
                    IntervalSeconds = 600f,
                    AnnouncementLeadSeconds = 15f,
                    MaxDropsPerRound = -1,
                    Faction = SpawnableFaction.NtfWave,
                    FallbackRole = RoleTypeId.NtfPrivate,
                    ScatterRadius = 2.5f,
                    EnableBeacon = true,
                    BeaconColor = "#0080FF",
                    BeaconDuration = 60f,
                    Items = new List<DropItem>
                    {
                        new DropItem { Item = ItemType.GunCOM18, Quantity = 1, Chance = 100 },
                        new DropItem { Item = ItemType.GunE11SR, Quantity = 1, Chance = 100 },
                        new DropItem { Item = ItemType.Ammo762x39, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.Ammo9x19, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.Medkit, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.Medkit, Quantity = 2, Chance = 20 },
                        new DropItem { Item = ItemType.Adrenaline, Quantity = 1, Chance = 100 },
                        new DropItem { Item = ItemType.ArmorCombat, Quantity = 1, Chance = 40 },
                        new DropItem { Item = ItemType.Coin, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.KeycardO5, Quantity = 1, Chance = 10 },
                    },
                },
                new DropProfile
                {
                    Key = "chaos",
                    DisplayName = "Largage de l'Insurrection du Chaos",
                    Broadcast = "<size=35><i><color=#5c5c5c>Un vehicule de</color> <color=#28AD00>l'Insurrection du Chaos</color> <color=#5c5c5c>vient de deposer</color> <color=#7a7a7a>du ravitaillement</color></i></size>",
                    BroadcastDuration = 10,
                    CassieAnnouncement = "chaos insurgency supply drop detected",
                    CassieSubtitles = "Largage de ravitaillement de l'Insurrection du Chaos detecte",
                    FirstDropSeconds = 900f,
                    IntervalSeconds = 600f,
                    AnnouncementLeadSeconds = 15f,
                    MaxDropsPerRound = -1,
                    Faction = SpawnableFaction.ChaosWave,
                    FallbackRole = RoleTypeId.ChaosRifleman,
                    ScatterRadius = 2.5f,
                    EnableBeacon = true,
                    BeaconColor = "#28AD00",
                    BeaconDuration = 60f,
                    Items = new List<DropItem>
                    {
                        new DropItem { Item = ItemType.GunLogicer, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.Ammo762x39, Quantity = 5, Chance = 100 },
                        new DropItem { Item = ItemType.Medkit, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.ArmorCombat, Quantity = 2, Chance = 20 },
                        new DropItem { Item = ItemType.Adrenaline, Quantity = 1, Chance = 100 },
                        new DropItem { Item = ItemType.Coin, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.KeycardO5, Quantity = 1, Chance = 10 },
                    },
                },
                new DropProfile
                {
                    Key = "neutre",
                    DisplayName = "Largage neutre",
                    Broadcast = "<size=35><i><color=#FFD700>Un largage</color> tombe : %ZONE%. Premier arrive, premier servi.</i></size>",
                    BroadcastDuration = 10,
                    CassieAnnouncement = "attention . supply drop incoming",
                    CassieSubtitles = "Largage de ravitaillement en approche : %ZONE%",
                    FirstDropSeconds = 420f,
                    IntervalSeconds = 480f,
                    AnnouncementLeadSeconds = 20f,
                    MaxDropsPerRound = 3,
                    FallbackRole = RoleTypeId.NtfPrivate,
                    Zones = new List<FacilityZone>
                    {
                        FacilityZone.LightContainment,
                        FacilityZone.HeavyContainment,
                        FacilityZone.Entrance,
                    },
                    ScatterRadius = 0.5f,
                    EnableBeacon = true,
                    BeaconColor = "#FFD700",
                    BeaconDuration = 60f,
                    Items = new List<DropItem>
                    {
                        new DropItem { Item = ItemType.GunCrossvec, Quantity = 1, Chance = 100 },
                        new DropItem { Item = ItemType.Ammo9x19, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.Medkit, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.GrenadeHE, Quantity = 1, Chance = 50 },
                        new DropItem { Item = ItemType.ArmorCombat, Quantity = 1, Chance = 50 },
                        new DropItem { Item = ItemType.Coin, Quantity = 2, Chance = 100 },
                        new DropItem { Item = ItemType.KeycardMTFOperative, Quantity = 1, Chance = 25 },
                    },
                },
            };
        }
    }
}
