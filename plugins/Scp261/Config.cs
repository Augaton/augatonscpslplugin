using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Hints;
using MapGeneration;
using UnityEngine;

namespace Scp261
{
    public sealed class Config : IHintSettings
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Emplacements possibles de la machine.")]
        public List<SpawnLocation> SpawnLocations { get; set; } = new List<SpawnLocation>
        {
            new SpawnLocation
            {
                RoomName = RoomName.EzOfficeSmall,
                Position = new Vector3(6.816f, -1.428f, 6.550f),
                Rotation = new Vector3(0f, -180f, 0f),
                Chance = 100f,
            },
        };

        [Description("Ne place qu'une machine par round, tiree parmi les emplacements.")]
        public bool SpawnOnlyOnce { get; set; } = false;

        [Description("Couleur de la machine simple, utilisee sans le schema ProjectMER.")]
        public string FallbackColor { get; set; } = "#8B0000";

        [Description("Objets acceptes par la machine.")]
        public List<ItemType> ActivationItems { get; set; } = new List<ItemType> { ItemType.Coin };

        [Description("Autorise SCP-3114 a utiliser la machine.")]
        public bool CanScp3114Use { get; set; } = false;

        [Description("Delai minimal en secondes entre deux utilisations par un meme joueur.")]
        public float UseCooldownSeconds { get; set; } = 1f;

        [Description("Rend la machine inutilisable pendant un mode de jeu de type arene.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Active les effets. Desactive, ce tirage donne un objet.")]
        public bool EnableEffects { get; set; } = true;

        [Description("Active le retrecissement. Desactive, ce tirage donne un objet.")]
        public bool EnableSizeChange { get; set; } = true;

        [Description("Active l'explosion. Desactivee, ce tirage donne un objet.")]
        public bool EnableExplode { get; set; } = true;

        [Description("Active la grenade flash. Desactivee, ce tirage donne un objet.")]
        public bool EnableFlashbang { get; set; } = true;

        [Description("Duree de l'aveuglement du resultat flash, en secondes.")]
        public float FlashbangSeconds { get; set; } = 4f;

        [Description("Active la flaque de tantrum. Desactivee, ce tirage donne un objet.")]
        public bool EnableTantrum { get; set; } = true;

        [Description("Active la teleportation. Desactivee, ce tirage donne un objet.")]
        public bool RandomTeleport { get; set; } = true;

        [Description("Poids du tirage d'un objet. Les poids n'ont pas a totaliser 100.")]
        public int ItemChance { get; set; } = 50;

        [Description("Poids du tirage d'un effet.")]
        public int EffectChance { get; set; } = 30;

        [Description("Poids du retrecissement.")]
        public int SizeChangeChance { get; set; } = 5;

        [Description("Poids de l'explosion.")]
        public int ExplodeChance { get; set; } = 1;

        [Description("Poids de la grenade flash.")]
        public int FlashbangChance { get; set; } = 5;

        [Description("Poids de la flaque de tantrum.")]
        public int TantrumChance { get; set; } = 9;

        [Description("Poids de la teleportation.")]
        public int RandomTeleportChance { get; set; } = 10;

        [Description("Effets possibles, par nom de classe du jeu.")]
        public List<string> EffectList { get; set; } = new List<string>
        {
            "Invigorated",
            "AntiScp207",
            "Scp207",
            "Asphyxiated",
            "Flashed",
            "Scp1853",
            "Hypothermia",
            "MovementBoost",
            "Invisible",
        };

        [Description("Objets que la machine peut donner.")]
        public List<ItemType> VendingMachineStock { get; set; } = new List<ItemType>
        {
            ItemType.KeycardChaosInsurgency,
            ItemType.KeycardContainmentEngineer,
            ItemType.KeycardFacilityManager,
            ItemType.GunA7,
            ItemType.GunFSP9,
            ItemType.GunLogicer,
            ItemType.GunE11SR,
            ItemType.GunRevolver,
            ItemType.GunCrossvec,
            ItemType.GunCom45,
            ItemType.Jailbird,
            ItemType.ParticleDisruptor,
            ItemType.SCP018,
            ItemType.SCP207,
            ItemType.AntiSCP207,
            ItemType.KeycardJanitor,
            ItemType.KeycardScientist,
            ItemType.KeycardZoneManager,
            ItemType.KeycardGuard,
            ItemType.MicroHID,
            ItemType.SCP500,
            ItemType.SCP268,
            ItemType.SCP1853,
        };

        [Description("Desactive, les armes sortent de la machine sans munitions.")]
        public bool SpawnWeaponsWithAmmo { get; set; } = true;

        [Description("Duree minimale des effets, en secondes.")]
        public int MinEffectDuration { get; set; } = 10;

        [Description("Duree maximale des effets, en secondes.")]
        public int MaxEffectDuration { get; set; } = 25;

        [Description("Taille appliquee par le retrecissement.")]
        public Vector3 GnomedSize { get; set; } = new Vector3(1.13f, 0.5f, 1.13f);

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 5f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer des autres plugins.")]
        public float HintYCoordinate { get; set; } = 650f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
