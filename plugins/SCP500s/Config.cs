using AugatonLib.Hints;
using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using SCP500s.API;

namespace SCP500s
{
    public sealed class Config : IHintSettings
    {
        private List<PillDefinition> pills = DefaultPills.Create();
        private List<ChaosOutcome> chaosOutcomes = DefaultChaosOutcomes.Create();
        private List<string> randomOnlyPills = DefaultPills.RandomOnlyKeys();

        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Mode de spawn : LootReplacement (remplace le loot de la carte), SpawnPoints (emplacements fixes par pilule) ou Both.")]
        public SpawnMode SpawnMode { get; set; } = SpawnMode.LootReplacement;

        [Description("Delai en secondes apres le debut du round avant le remplacement du loot.")]
        public float SpawnDelaySeconds { get; set; } = 5f;

        [Description("Types d'objets pouvant etre remplaces sur la carte.")]
        public List<ItemType> ReplaceableSourceItems { get; set; } = new List<ItemType>
        {
            ItemType.Painkillers,
            ItemType.Medkit,
            ItemType.Flashlight,
            ItemType.Radio,
            ItemType.KeycardJanitor,
            ItemType.KeycardScientist,
        };

        [Description("Pourcentage des objets eligibles effectivement remplaces.")]
        public float ReplacementChancePercent { get; set; } = 60f;

        [Description("Part des remplacements donnant une piece plutot qu'une pilule. Le reste donne une pilule.")]
        public float CoinSharePercent { get; set; } = 10f;

        [Description("Nombre maximum de remplacements par round. 0 = sans limite.")]
        public int MaxReplacementsPerRound { get; set; } = 40;

        [Description("Fait apparaitre des pilules pendant tout le round, un peu partout sur la carte, aux emplacements de loot, pour qu'il y en ait toujours au sol.")]
        public bool ContinuousSpawn { get; set; } = true;

        [Description("Suspend l'apparition de pilules pendant un mode de jeu de type arene, comme le Gun Game.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Fait monter le chaos par paliers : plus de pilules, et plus vite, au fil du round.")]
        public bool ChaosEscalation { get; set; } = true;

        [Description("Duree en secondes d'un palier de chaos.")]
        public float ChaosStepSeconds { get; set; } = 120f;

        [Description("Nombre maximal de paliers de chaos.")]
        public int ChaosMaxLevel { get; set; } = 10;

        [Description("Pilules ajoutees a continuous_spawn_target par palier.")]
        public int ChaosTargetPerLevel { get; set; } = 3;

        [Description("Pilules ajoutees a continuous_spawn_batch par palier.")]
        public int ChaosBatchPerLevel { get; set; } = 1;

        [Description("Secondes retirees a l'intervalle par palier. L'intervalle ne descend jamais sous 30.")]
        public float ChaosIntervalPerLevel { get; set; } = 3f;

        [Description("Points ajoutes par palier a random_pill_share_percent, pour les pilules apparues en continu.")]
        public float ChaosRandomSharePerLevel { get; set; } = 4f;

        [Description("Pilules rendues a la carte pour chaque pilule consommee. 0 = desactive. Requiert continuous_spawn.")]
        public int RenewalPerConsumedPill { get; set; } = 1;

        [Description("Chance en pourcent qu'une pilule consommee en rende une de plus.")]
        public float RenewalBonusChance { get; set; } = 25f;

        [Description("Delai en secondes avant l'apparition des pilules rendues.")]
        public float RenewalDelaySeconds { get; set; } = 20f;

        [Description("Plafond absolu de pilules au sol, pour proteger le serveur.")]
        public int MaxPillsOnGround { get; set; } = 60;

        [Description("Intervalle en secondes entre deux verifications de l'apparition continue. Minimum 30.")]
        public float ContinuousSpawnIntervalSeconds { get; set; } = 60f;

        [Description("Nombre de pilules que l'apparition continue cherche a maintenir au sol en permanence.")]
        public int ContinuousSpawnTarget { get; set; } = 12;

        [Description("Nombre maximum de pilules ajoutees a chaque verification.")]
        public int ContinuousSpawnBatch { get; set; } = 3;

        [Description("Distance minimale en metres entre une nouvelle pilule et un joueur vivant, pour qu'elle n'apparaisse pas sous ses yeux.")]
        public float ContinuousSpawnPlayerDistance { get; set; } = 15f;

        [Description("Distance minimale en metres entre deux pilules au sol, pour les repartir sur toute la carte.")]
        public float ContinuousSpawnSpacing { get; set; } = 10f;

        [Description("Part des pilules placees qui sont la pilule a effet aleatoire. Le reste est tire dans le catalogue par poids.")]
        public float RandomPillSharePercent { get; set; } = 40f;

        [Description("Cle de la pilule a effet aleatoire. Elle ignore les poids de spawn et le plafond par round.")]
        public string RandomPillKey { get; set; } = "chaos";

        [Description("Pilules du catalogue reservees a la pilule aleatoire : elles n'apparaissent jamais sur la carte et leurs effets ne sortent que via la roulette. Le staff peut toujours les donner.")]
        public List<string> RandomOnlyPills
        {
            get => randomOnlyPills;
            set => randomOnlyPills = CatalogMigration.UpgradeRandomOnly(value);
        }

        [Description("Poids de la categorie Good dans le tirage de la pilule aleatoire.")]
        public float ChaosGoodWeight { get; set; } = 34f;

        [Description("Poids de la categorie Bad dans le tirage de la pilule aleatoire.")]
        public float ChaosBadWeight { get; set; } = 33f;

        [Description("Poids de la categorie Neutral dans le tirage de la pilule aleatoire.")]
        public float ChaosNeutralWeight { get; set; } = 33f;

        [Description("Cooldown en secondes entre deux pilules consommees par un meme joueur. Pendant ce delai, la prise est bloquee et la pilule conservee.")]
        public float PillCooldownSeconds { get; set; } = 3f;

        [Description("Autorise les SCP a consommer des pilules.")]
        public bool AllowScpUsage { get; set; } = false;

        [Description("Autorise les SCP a etre cibles par les pilules de permutation, d'invocation et de contagion.")]
        public bool AllowScpTargets { get; set; } = false;

        [Description("Roles ne pouvant ni consommer ni etre cibles par les pilules.")]
        public List<RoleTypeId> IgnoredRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Tutorial,
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker,
        };

        [Description("Duree maximale autorisee pour un effet borne dans le temps, en secondes. Une duree de 0 reste permanente.")]
        public float MaxEffectDuration { get; set; } = 300f;

        [Description("Nombre d'emplacements d'inventaire au-dela duquel un objet donne par une pilule tombe aux pieds du joueur.")]
        public int MaxInventorySlots { get; set; } = 8;

        [Description("Duree d'affichage des messages, en secondes.")]
        public float HintDuration { get; set; } = 5f;

        [Description("Affiche le nom et la description d'une pilule quand on la ramasse ou qu'on la selectionne.")]
        public bool ShowItemInfo { get; set; } = true;

        [Description("Envoie aussi le message d'une pilule consommee en broadcast personnel. A activer si les hints ne s'affichent pas sur le serveur.")]
        public bool BroadcastMessages { get; set; } = false;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer des autres plugins.")]
        public float HintYCoordinate { get; set; } = 700f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;

        [Description("Active le halo lumineux sur les pilules posees au sol.")]
        public bool EnableGlow { get; set; } = true;

        [Description("Intensite du halo lumineux.")]
        public float GlowIntensity { get; set; } = 0.7f;

        [Description("Portee du halo lumineux.")]
        public float GlowRange { get; set; } = 0.5f;

        [Description("Nombre maximum de halos actifs simultanement. Chaque halo est un objet reseau.")]
        public int MaxActiveGlows { get; set; } = 60;

        [Description("Echelle du joueur retreci.")]
        public float RakunScale { get; set; } = 0.5f;

        [Description("Echelle du joueur agrandi.")]
        public float GiantScale { get; set; } = 1.4f;

        [Description("Hauteur en metres a laquelle le joueur est projete.")]
        public float SkyLaunchHeight { get; set; } = 50f;

        [Description("Delai en secondes avant l'explosion qui suit la projection.")]
        public float SkyLaunchFuse { get; set; } = 5f;

        [Description("Vagues de renfort pouvant etre declenchees. Une est tiree au hasard.")]
        public List<SpawnableFaction> ForceWaveFactions { get; set; } = new List<SpawnableFaction>
        {
            SpawnableFaction.NtfWave,
            SpawnableFaction.ChaosWave,
        };

        [Description("Duree en secondes du broadcast annoncant une vague forcee. 0 = pas de broadcast.")]
        public ushort ForceWaveBroadcastDuration { get; set; } = 6;

        [Description("Intensite de l'effet purement aleatoire (1-255).")]
        public byte RandomEffectIntensity { get; set; } = 50;

        [Description("Duree en secondes de l'effet purement aleatoire.")]
        public float RandomEffectDuration { get; set; } = 10f;

        [Description("Duree en secondes de la coupure de lumiere.")]
        public float BlackoutDuration { get; set; } = 20f;

        [Description("Incidents de SiteIncidents que la pilule d'incident peut provoquer.")]
        public List<string> PillIncidents { get; set; } = new List<string> { "Blackout", "Alert", "Lockdown", "GasLeak", "TeslaSurge" };

        [Description("Profils de SupplyDrop que la pilule de largage peut appeler.")]
        public List<string> PillDropProfiles { get; set; } = new List<string> { "mtf", "chaos" };

        [Description("Secondes minimales entre deux incidents ou largages demandes par des pilules, tous joueurs confondus, 1 au plus bas.")]
        public float SiteRequestCooldownSeconds { get; set; } = 60f;

        [Description("Duree en secondes de la recoloration des lumieres d'une salle.")]
        public float DiscoDuration { get; set; } = 30f;

        [Description("Type de souris invoquees dans l'abri.")]
        public byte MiceType { get; set; } = 1;

        [Description("Rayon en metres de la contagion.")]
        public float ContagionRadius { get; set; } = 12f;

        [Description("Nombre maximum de SCP listes par la pilule radar, du plus proche au plus eloigne.")]
        public int RadarMaxEntries { get; set; } = 5;

        [Description("Echelle des cartes d'acces pour la pilule de promotion, de la plus faible a la plus forte. Sans carte, le joueur recoit la premiere.")]
        public List<ItemType> KeycardUpgradePath { get; set; } = new List<ItemType>
        {
            ItemType.KeycardJanitor,
            ItemType.KeycardScientist,
            ItemType.KeycardResearchCoordinator,
            ItemType.KeycardZoneManager,
            ItemType.KeycardGuard,
            ItemType.KeycardMTFPrivate,
            ItemType.KeycardContainmentEngineer,
            ItemType.KeycardMTFOperative,
            ItemType.KeycardMTFCaptain,
            ItemType.KeycardFacilityManager,
            ItemType.KeycardO5,
        };

        [Description("Annonces declenchables par la pilule Cassie. Une entree refusee par le jeu est signalee dans les logs au premier round.")]
        public List<string> CassieLines { get; set; } = new List<string>
        {
            "ATTENTION ALL PERSONNEL . SCP 5 0 0 CONTAINMENT BREACH DETECTED IN LIGHT CONTAINMENT ZONE",
            "WARNING . UNKNOWN ENTITY DETECTED IN HEAVY CONTAINMENT ZONE",
            "ALL REMAINING PERSONNEL ARE ADVISED TO EVACUATE IMMEDIATELY",
            "SCP 1 7 3 SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM",
        };

        [Description("Probabilite de glitch de l'annonce Cassie (0-1).")]
        public float CassieGlitchChance { get; set; } = 0.2f;

        [Description("Probabilite de brouillage de l'annonce Cassie (0-1).")]
        public float CassieJamChance { get; set; } = 0.1f;

        [Description("Catalogue des pilules. Mettre is_enabled a false pour en desactiver une, ou ajouter une entree pour en creer une nouvelle avec un id inutilise. Les pilules par defaut absentes de la liste y sont ajoutees au chargement.")]
        public List<PillDefinition> Pills
        {
            get => pills;
            set => pills = DefaultPills.AddMissing(CatalogMigration.UpgradePills(value));
        }

        [Description("Issues possibles de la pilule a effet aleatoire, reparties en Good, Bad et Neutral. Les issues par defaut absentes de la liste y sont ajoutees au chargement ; mettre weight a 0 pour en retirer une.")]
        public List<ChaosOutcome> ChaosOutcomes
        {
            get => chaosOutcomes;
            set => chaosOutcomes = DefaultChaosOutcomes.AddMissing(CatalogMigration.UpgradeOutcomes(value));
        }
    }
}
