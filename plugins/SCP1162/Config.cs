using AugatonLib.Hints;
using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using SCP1162.API;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP1162
{
    public sealed class Config : IHintSettings
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Suspend l'anomalie pendant un mode arene de RandomGamemode.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Distance maximale, en metres, a laquelle un objet lache est aspire par SCP-1162.")]
        public float UseDistance { get; set; } = 2f;

        [Description("Chance, en pourcentage, que SCP-1162 apparaisse au debut du round.")]
        public float SpawnChance { get; set; } = 100f;

        [Description("Nombre d'emplacements utilises simultanement, dans la limite de la liste ci-dessous.")]
        public int SpawnCount { get; set; } = 1;

        [Description("Chance, en pourcentage, que SCP-1162 arrache les mains au lieu d'echanger l'objet. Zero desactive.")]
        public float CutHandsChance { get; set; } = 30f;

        [Description("Duree de l'effet Mains arrachees, en secondes. Zero rend l'effet permanent.")]
        public float SeveredHandsDuration { get; set; } = 0f;

        [Description("Chance, en pourcentage, que l'objet soit avale sans contrepartie. Zero desactive.")]
        public float DeleteChance { get; set; } = 10f;

        [Description("Les objets lances a distance ne font jamais perdre les mains.")]
        public bool IgnoreThrownItems { get; set; } = true;

        [Description("Delai minimal, en secondes, entre deux utilisations par un meme joueur.")]
        public float CooldownSeconds { get; set; } = 2f;

        [Description("Roles qui ne peuvent pas utiliser SCP-1162.")]
        public List<RoleTypeId> IgnoredRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Scp3114,
        };

        [Description("Objets que SCP-1162 refuse d'avaler.")]
        public List<ItemType> IgnoredItems { get; set; } = new List<ItemType>
        {
            ItemType.SCP330,
            ItemType.MicroHID,
        };

        [Description("Objets rendus par SCP-1162, avec leur poids de tirage.")]
        public List<RewardEntry> Rewards { get; set; } = new List<RewardEntry>
        {
            new RewardEntry { Item = ItemType.Coin, Weight = 20 },
            new RewardEntry { Item = ItemType.Flashlight, Weight = 15 },
            new RewardEntry { Item = ItemType.Ammo9x19, Weight = 12 },
            new RewardEntry { Item = ItemType.Ammo556x45, Weight = 10 },
            new RewardEntry { Item = ItemType.Ammo762x39, Weight = 10 },
            new RewardEntry { Item = ItemType.Medkit, Weight = 10 },
            new RewardEntry { Item = ItemType.KeycardJanitor, Weight = 8 },
            new RewardEntry { Item = ItemType.KeycardScientist, Weight = 6 },
            new RewardEntry { Item = ItemType.KeycardGuard, Weight = 6 },
            new RewardEntry { Item = ItemType.GunCOM15, Weight = 5 },
            new RewardEntry { Item = ItemType.GunCOM18, Weight = 5 },
            new RewardEntry { Item = ItemType.GrenadeFlash, Weight = 5 },
            new RewardEntry { Item = ItemType.Adrenaline, Weight = 5 },
            new RewardEntry { Item = ItemType.SCP207, Weight = 3 },
            new RewardEntry { Item = ItemType.KeycardContainmentEngineer, Weight = 2 },
            new RewardEntry { Item = ItemType.KeycardZoneManager, Weight = 1 },
            new RewardEntry { Item = ItemType.KeycardResearchCoordinator, Weight = 1 },
            new RewardEntry { Item = ItemType.SCP500, Weight = 1 },
        };

        [Description("Noms affiches aux joueurs. Un objet absent de cette liste garde son nom technique.")]
        public Dictionary<ItemType, string> ItemNames { get; set; } = new Dictionary<ItemType, string>
        {
            { ItemType.Coin, "une piece" },
            { ItemType.Flashlight, "une lampe torche" },
            { ItemType.Ammo9x19, "des munitions 9x19" },
            { ItemType.Ammo556x45, "des munitions 5.56x45" },
            { ItemType.Ammo762x39, "des munitions 7.62x39" },
            { ItemType.Ammo12gauge, "des munitions calibre 12" },
            { ItemType.Ammo44cal, "des munitions .44" },
            { ItemType.Medkit, "un medkit" },
            { ItemType.Painkillers, "des antidouleurs" },
            { ItemType.Adrenaline, "une adrenaline" },
            { ItemType.Radio, "une radio" },
            { ItemType.KeycardJanitor, "une carte de concierge" },
            { ItemType.KeycardScientist, "une carte de scientifique" },
            { ItemType.KeycardGuard, "une carte de garde" },
            { ItemType.KeycardContainmentEngineer, "une carte d'ingenieur de confinement" },
            { ItemType.KeycardZoneManager, "une carte de chef de zone" },
            { ItemType.KeycardResearchCoordinator, "une carte de superviseur de recherche" },
            { ItemType.KeycardFacilityManager, "une carte de directeur du site" },
            { ItemType.KeycardChaosInsurgency, "une carte de l'Insurrection du Chaos" },
            { ItemType.KeycardMTFPrivate, "une carte de soldat FIM" },
            { ItemType.KeycardMTFOperative, "une carte d'operateur FIM" },
            { ItemType.KeycardMTFCaptain, "une carte de capitaine FIM" },
            { ItemType.GunCOM15, "un COM-15" },
            { ItemType.GunCOM18, "un COM-18" },
            { ItemType.GunFSP9, "un FSP-9" },
            { ItemType.GunCrossvec, "un Crossvec" },
            { ItemType.GunE11SR, "un MTF-E11-SR" },
            { ItemType.GunLogicer, "un Logicer" },
            { ItemType.GunRevolver, "un revolver" },
            { ItemType.GunAK, "une AK" },
            { ItemType.GunShotgun, "un fusil a pompe" },
            { ItemType.GrenadeFlash, "une grenade aveuglante" },
            { ItemType.GrenadeHE, "une grenade explosive" },
            { ItemType.SCP018, "SCP-018" },
            { ItemType.SCP207, "SCP-207" },
            { ItemType.SCP268, "SCP-268" },
            { ItemType.SCP500, "SCP-500" },
            { ItemType.SCP1576, "SCP-1576" },
            { ItemType.SCP1853, "SCP-1853" },
            { ItemType.SCP2176, "SCP-2176" },
            { ItemType.SCP244a, "SCP-244" },
            { ItemType.AntiSCP207, "de l'anti-SCP-207" },
            { ItemType.ArmorLight, "un gilet leger" },
            { ItemType.ArmorCombat, "un gilet de combat" },
            { ItemType.ArmorHeavy, "un gilet lourd" },
            { ItemType.MicroHID, "un Micro-HID" },
        };

        [Description("Les armes rendues arrivent chargeur plein.")]
        public bool FillMagazine { get; set; } = true;

        [Description("Les armes rendues arrivent avec des accessoires tires au hasard.")]
        public bool RandomAttachments { get; set; } = true;

        [Description("Couleur de l'anomalie, au format hexadecimal.")]
        public string ColorHex { get; set; } = "#000000";

        [Description("Opacite de l'anomalie, entre 0 et 1.")]
        public float Alpha { get; set; } = 0.95f;

        [Description("Emplacements possibles de SCP-1162.")]
        public List<SpawnLocation> Locations { get; set; } = new List<SpawnLocation>
        {
            new SpawnLocation(),
        };

        [Description("Duree d'affichage des messages, en secondes.")]
        public float HintDuration { get; set; } = 3f;

        [Description("Hauteur du hint dans HintServiceMeow. Voir HINTS.md.")]
        public float HintYCoordinate { get; set; } = 800f;

        [Description("Taille de police du hint.")]
        public int HintFontSize { get; set; } = 20;

        public Color ResolveColor()
        {
            if (!ColorUtility.TryParseHtmlString(ColorHex, out Color parsed))
                parsed = Color.black;

            parsed.a = Alpha;

            return parsed;
        }

        public void Validate()
        {
            UseDistance = Clamp(UseDistance, 0.5f, 30f, nameof(UseDistance));
            SpawnChance = Clamp(SpawnChance, 0f, 100f, nameof(SpawnChance));
            CutHandsChance = Clamp(CutHandsChance, 0f, 100f, nameof(CutHandsChance));
            DeleteChance = Clamp(DeleteChance, 0f, 100f, nameof(DeleteChance));
            SeveredHandsDuration = Clamp(SeveredHandsDuration, 0f, 3600f, nameof(SeveredHandsDuration));
            CooldownSeconds = Clamp(CooldownSeconds, 0f, 300f, nameof(CooldownSeconds));
            HintDuration = Clamp(HintDuration, 1f, 30f, nameof(HintDuration));
            Alpha = Clamp(Alpha, 0f, 1f, nameof(Alpha));
            HintFontSize = (int)Clamp(HintFontSize, 8f, 60f, nameof(HintFontSize));

            if (Locations is null || Locations.Count == 0)
            {
                Locations = new List<SpawnLocation> { new SpawnLocation() };
                Logger.Warn("SCP1162: aucun emplacement configure, l'emplacement par defaut est utilise.");
            }

            SpawnCount = (int)Clamp(SpawnCount, 1f, Locations.Count, nameof(SpawnCount));

            if (Rewards is null || Rewards.Count == 0)
            {
                Rewards = new List<RewardEntry> { new RewardEntry() };
                Logger.Warn("SCP1162: aucune recompense configuree, une piece est utilisee par defaut.");
            }

            if (IgnoredRoles is null)
                IgnoredRoles = new List<RoleTypeId>();

            if (IgnoredItems is null)
                IgnoredItems = new List<ItemType>();

            if (ItemNames is null)
                ItemNames = new Dictionary<ItemType, string>();

            if (!ColorUtility.TryParseHtmlString(ColorHex, out Color _))
                Logger.Warn($"SCP1162: couleur \"{ColorHex}\" illisible, le noir est utilise.");
        }

        private static float Clamp(float value, float min, float max, string name)
        {
            if (value >= min && value <= max)
                return value;

            float clamped = Mathf.Clamp(value, min, max);
            Logger.Warn($"SCP1162: {name} = {value} hors bornes [{min}, {max}], ramene a {clamped}.");

            return clamped;
        }
    }
}
