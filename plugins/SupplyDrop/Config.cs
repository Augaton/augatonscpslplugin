using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using SupplyDrop.API;

namespace SupplyDrop
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Suspend les largages automatiques pendant un mode de jeu de type arene, comme le Gun Game.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Nombre minimum de joueurs connectes pour qu'un largage se declenche.")]
        public int MinPlayers { get; set; } = 2;

        [Description("Delai en secondes avant de reessayer quand le nombre de joueurs est insuffisant.")]
        public float RetrySeconds { get; set; } = 60f;

        [Description("Intervalle en secondes du planificateur. Une seule coroutine gere tous les largages.")]
        public float SchedulerTick { get; set; } = 1f;

        [Description("Secondes minimales entre deux gros evenements automatiques, incidents et largages confondus. 0 = aucune attente.")]
        public float MinEventGapSeconds { get; set; } = 60f;

        [Description("Chance, en pourcentage, qu'un largage soit piege. 0 = jamais.")]
        public float TrapChancePercent { get; set; } = 15f;

        [Description("Chance ajoutee par niveau du Directeur du chaos, s'il est installe.")]
        public float TrapChancePerChaosLevel { get; set; } = 5f;

        [Description("Objets ajoutes a chaque largage pendant le mutateur ScarceAmmo de RoundMutators.")]
        public List<ItemType> ScarceAmmoBonus { get; set; } = new List<ItemType> { ItemType.Ammo9x19, ItemType.Ammo556x45, ItemType.Ammo762x39 };

        [Description("Objets ajoutes a chaque largage pendant le mutateur ScarceMedical de RoundMutators.")]
        public List<ItemType> ScarceMedicalBonus { get; set; } = new List<ItemType> { ItemType.Medkit, ItemType.Medkit, ItemType.Painkillers };

        [Description("Distance minimale en metres entre un largage de zone et le joueur vivant le plus proche.")]
        public float ZoneDropClearance { get; set; } = 8f;

        [Description("Distance en metres a laquelle un humain declenche le piege.")]
        public float TrapRange { get; set; } = 4f;

        [Description("Secondes entre le declenchement du piege et son explosion : le joueur attentif a le temps de fuir.")]
        public float TrapFuseSeconds { get; set; } = 2f;

        [Description("Duree de l'aveuglement d'un piege aveuglant, en secondes.")]
        public float TrapFlashSeconds { get; set; } = 4f;

        [Description("Secondes apres lesquelles un piege intact se desamorce.")]
        public float TrapLifetimeSeconds { get; set; } = 180f;

        [Description("Journalise chaque largage dans la console serveur.")]
        public bool LogDrops { get; set; } = true;

        [Description("Nombre maximum d'objets places par largage, garde-fou contre une config aberrante.")]
        public int MaxItemsPerDrop { get; set; } = 60;

        [Description("Hauteur en metres ajoutee au point de largage pour eviter que les objets traversent le sol.")]
        public float SpawnHeightOffset { get; set; } = 0.5f;

        [Description("Profils de largage. Ajouter une entree avec une cle inutilisee cree un nouveau type de largage.")]
        public List<DropProfile> Profiles { get; set; } = DefaultProfiles.Create();

        [Description("Largage declenche quand TeamGenocide annonce l'extinction d'une equipe. Cle d'equipe vers cle de profil, vide = desactive.")]
        public Dictionary<Team, string> TeamWipeDrops { get; set; } = new Dictionary<Team, string>();
    }
}
