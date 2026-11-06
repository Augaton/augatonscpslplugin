using AugatonLib.Hints;
using System.Collections.Generic;
using System.ComponentModel;

namespace BetterCoinflipsRewritten
{
    public sealed class Config : IHintSettings
    {
        [Description("Affiche des logs de debug supplementaires.")]
        public bool Debug { get; set; } = false;

        [Description("Ecrit chaque lancer dans le log du serveur. Coute une ecriture disque par lancer.")]
        public bool LogFlips { get; set; } = false;

        [Description("Cooldown global, en secondes, des effets qui touchent tout le site, comme la panne de lumiere : des lancers en chaine ne peuvent pas garder le site dans le noir.")]
        public float FacilityEffectCooldown { get; set; } = 60f;

        [Description("Cooldown entre deux lancers, en secondes.")]
        public float CoinCooldown { get; set; } = 5f;

        [Description("Detruit la piece apres le lancer. C'est la principale protection contre les abus : une piece vaut un lancer.")]
        public bool ConsumeCoinOnFlip { get; set; } = true;

        [Description("Delai en secondes avant la destruction de la piece. La retirer pendant l'evenement casse l'animation du client : rester au-dessus d'une seconde.")]
        public float CoinConsumeDelay { get; set; } = 1.5f;

        [Description("Duree du message d'effet, en secondes.")]
        public float EffectMessageDuration { get; set; } = 5f;

        [Description("Duree du message de cooldown, en secondes.")]
        public float CooldownMessageDuration { get; set; } = 3f;


        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 750f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;

        [Description("Poids de l'effet carte d'acces aleatoire.")]
        public int KeycardChance { get; set; } = 20;

        [Description("Poids de l'effet trousse de soins.")]
        public int MedicalKitChance { get; set; } = 35;

        [Description("Poids de l'effet teleportation vers la sortie.")]
        public int TpToEscapeChance { get; set; } = 5;

        [Description("Poids de l'effet soin.")]
        public int HealChance { get; set; } = 10;

        [Description("Poids de l'effet hausse de la vie maximale.")]
        public int MoreHpChance { get; set; } = 10;

        [Description("Poids de la recompense SCP-268.")]
        public int HatChance { get; set; } = 10;

        [Description("Poids de la recompense Logicer a une balle.")]
        public int OneAmmoLogicerChance { get; set; } = 1;

        [Description("Poids de la recompense SCP-2176.")]
        public int LightbulbChance { get; set; } = 15;

        [Description("Poids de la recompense bonbon rose.")]
        public int PinkCandyChance { get; set; } = 10;

        [Description("Poids de l'effet taille aleatoire.")]
        public int SizeChangeChance { get; set; } = 20;

        [Description("Poids de l'effet bouclier de vie artificielle.")]
        public int AhpShieldChance { get; set; } = 12;

        [Description("Vie artificielle donnee par l'effet bouclier.")]
        public float AhpShieldAmount { get; set; } = 40f;

        [Description("Poids de l'effet fantome.")]
        public int GhostlyChance { get; set; } = 8;

        [Description("Duree de l'effet fantome, en secondes.")]
        public float GhostlyDuration { get; set; } = 8f;

        [Description("Poids de l'effet coup de sucre.")]
        public int SugarRushChance { get; set; } = 12;

        [Description("Duree de l'effet coup de sucre, en secondes.")]
        public float SugarRushDuration { get; set; } = 15f;

        [Description("Poids de l'effet qui rend deux pieces. C'est le seul moyen de finir un lancer avec plus de pieces qu'au depart : garder un poids faible.")]
        public int DoubleCoinChance { get; set; } = 4;

        [Description("Nombre d'emplacements d'inventaire au-dela duquel un effet pose sa recompense au sol au lieu de la donner.")]
        public int MaxInventorySlots { get; set; } = 8;

        [Description("Vie rendue par l'effet soin.")]
        public float HealAmount { get; set; } = 25f;

        [Description("Pourcentage de hausse de la vie maximale du joueur.")]
        public float MoreHpPercent { get; set; } = 10f;

        [Description("Poids de la carte de concierge dans le tirage de cartes.")]
        public int JanitorKeycardChance { get; set; } = 40;

        [Description("Poids de la carte de scientifique dans le tirage de cartes.")]
        public int ScientistKeycardChance { get; set; } = 25;

        [Description("Poids de la carte de chef de zone dans le tirage de cartes.")]
        public int ZoneManagerKeycardChance { get; set; } = 18;

        [Description("Poids de la carte de garde du site dans le tirage de cartes.")]
        public int GuardKeycardChance { get; set; } = 10;

        [Description("Poids de la carte de coordinateur de recherche dans le tirage de cartes.")]
        public int ResearchCoordinatorKeycardChance { get; set; } = 6;

        [Description("Poids de la carte d'ingenieur de confinement dans le tirage de cartes.")]
        public int ContainmentEngineerKeycardChance { get; set; } = 1;

        [Description("Poids de l'effet perte de vie.")]
        public int HpReductionChance { get; set; } = 20;

        [Description("Pourcentage de la vie actuelle retire par l'effet.")]
        public float HpReductionPercent { get; set; } = 30f;

        [Description("Poids de l'effet teleportation vers les cellules des Classe-D.")]
        public int TpToClassDCellsChance { get; set; } = 5;

        [Description("Poids de l'effet panne de lumiere du site.")]
        public int LightsOutChance { get; set; } = 20;

        [Description("Duree de la panne de lumiere, en secondes.")]
        public float LightsOutDuration { get; set; } = 10f;

        [Description("Poids de l'effet grenade flash sur le joueur.")]
        public int TrollFlashChance { get; set; } = 50;

        [Description("Retardement de la grenade flash, en secondes.")]
        public float TrollFlashFuseTime { get; set; } = 0.8f;

        [Description("Poids de l'effet qui laisse le joueur a 1 PV.")]
        public int OneHpLeftChance { get; set; } = 15;

        [Description("Poids de l'effet qui vide l'inventaire.")]
        public int InventoryResetChance { get; set; } = 20;

        [Description("Poids de l'effet grenade instantanee.")]
        public int InstantExplosionChance { get; set; } = 10;

        [Description("Retardement de la grenade instantanee, en secondes.")]
        public float InstantExplosionFuseTime { get; set; } = 0.1f;

        [Description("Poids de l'effet teleportation dans une salle au hasard.")]
        public int RandomTeleportChance { get; set; } = 15;

        [Description("Poids de l'effet ivresse.")]
        public int DrunkChance { get; set; } = 15;

        [Description("Duree de l'effet ivresse, en secondes.")]
        public float DrunkDuration { get; set; } = 20f;

        [Description("Poids de l'effet prismatique.")]
        public int PrismaticTripChance { get; set; } = 15;

        [Description("Duree de l'effet prismatique, en secondes.")]
        public float PrismaticTripDuration { get; set; } = 12f;

        [Description("Poids de l'effet amnesie.")]
        public int AmnesiaChance { get; set; } = 15;

        [Description("Duree de l'effet amnesie, en secondes.")]
        public float AmnesiaDuration { get; set; } = 10f;

        [Description("Poids de l'effet catapulte.")]
        public int SkyLaunchChance { get; set; } = 8;

        [Description("Hauteur en metres du saut de l'effet catapulte. Les degats a l'atterrissage sont le but de l'effet.")]
        public float SkyLaunchHeight { get; set; } = 25f;

        [Description("Poids de l'effet echange de position. Il deplace un second joueur : il reste rare expres.")]
        public int PositionSwapChance { get; set; } = 8;

        [Description("Poids de l'effet fausse ogive. Purement visuel, sans aucun degat.")]
        public int FakeWarheadChance { get; set; } = 12;

        [Description("Duree en secondes de l'ecran noir qui suit la fausse ogive.")]
        public float FakeWarheadDimDuration { get; set; } = 3f;

        [Description("Poids de l'effet piege tantrum de SCP-173.")]
        public int TantrumTrapChance { get; set; } = 15;

        [Description("Poids de l'effet petrification.")]
        public int PetrifiedChance { get; set; } = 10;

        [Description("Duree de la petrification, en secondes. Le joueur ne peut plus bouger : garder une duree courte.")]
        public float PetrifiedDuration { get; set; } = 4f;

        [Description("Poids de l'effet lumieres disco. Il touche tout le site et passe donc par facility_effect_cooldown.")]
        public int DiscoLightsChance { get; set; } = 10;

        [Description("Duree de l'effet lumieres disco, en secondes.")]
        public float DiscoLightsDuration { get; set; } = 20f;

        [Description("Secondes entre deux changements de couleur des lumieres disco.")]
        public float DiscoLightsInterval { get; set; } = 1.5f;

        [Description("Poids de l'effet invasion de souris. Il touche tout le site et passe donc par facility_effect_cooldown.")]
        public int MiceInvasionChance { get; set; } = 8;

        [Description("Type de souris que l'effet fait apparaitre dans l'abri.")]
        public byte MiceInvasionType { get; set; } = 1;

        [Description("Poids de l'effet annonce moqueuse de C.A.S.S.I.E. Il touche tout le site et passe donc par facility_effect_cooldown.")]
        public int CassieMockChance { get; set; } = 8;

        [Description("Annonces possibles de l'effet annonce moqueuse. Une ligne refusee par le jeu est signalee dans le log au premier lancer de la session et n'est jamais jouee.")]
        public List<string> CassieMockLines { get; set; } = new List<string>
        {
            "ATTENTION ALL PERSONNEL . AN UNKNOWN OBJECT HAS BEEN DETECTED IN LIGHT CONTAINMENT ZONE",
            "WARNING . CONTAINMENT FAILURE DETECTED . ALL REMAINING PERSONNEL ARE ADVISED TO EVACUATE IMMEDIATELY",
            "ATTENTION ALL PERSONNEL . SEVERAL SUBJECTS ARE IN DANGER . GOOD LUCK",
        };

        [Description("Probabilite que l'annonce moqueuse soit deformee (0-1).")]
        public float CassieMockGlitchChance { get; set; } = 0.2f;

        [Description("Probabilite que l'annonce moqueuse soit brouillee (0-1).")]
        public float CassieMockJamChance { get; set; } = 0.1f;

        [Description("Poids de l'effet fausse explosion, sans danger.")]
        public int FakeExplosionChance { get; set; } = 12;

        [Description("Duree en secondes de la surdite et de la commotion laissees par la fausse explosion.")]
        public float FakeExplosionStunDuration { get; set; } = 5f;

        [Description("Poids de l'effet seconde chance. Il annule la prochaine mort du joueur.")]
        public int SecondChanceChance { get; set; } = 3;

        [Description("Vie du joueur quand la seconde chance annule sa mort.")]
        public float SecondChanceHealth { get; set; } = 25f;

        [Description("Poids de l'effet jackpot.")]
        public int JackpotChance { get; set; } = 4;

        [Description("Nombre d'objets du jackpot.")]
        public int JackpotAmount { get; set; } = 3;

        [Description("Objets possibles du jackpot. Eviter Coin : DoubleCoin est le seul effet qui rend des pieces.")]
        public List<ItemType> JackpotItems { get; set; } = new List<ItemType>
        {
            ItemType.Adrenaline,
            ItemType.Painkillers,
            ItemType.Medkit,
            ItemType.GrenadeFlash,
            ItemType.KeycardGuard,
            ItemType.Flashlight,
            ItemType.Radio,
        };

        [Description("Fait reapparaitre des pieces sur la carte pendant le round, dans des emplacements surs, loin des joueurs.")]
        public bool CoinRenewal { get; set; } = true;

        [Description("Suspend le renouvellement et les pieces de debut de round pendant un mode arene comme Gun Game.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Secondes entre deux verifications du renouvellement. Minimum 30.")]
        public float CoinRenewalIntervalSeconds { get; set; } = 90f;

        [Description("Nombre de pieces gardees au sol.")]
        public int CoinRenewalTarget { get; set; } = 6;

        [Description("Nombre maximal de pieces ajoutees par verification.")]
        public int CoinRenewalBatch { get; set; } = 2;

        [Description("Les pieces n'apparaissent jamais plus pres d'un joueur vivant que cette distance.")]
        public float CoinRenewalPlayerDistance { get; set; } = 15f;

        [Description("Duree d'un palier de chaos, en secondes. 0 coupe l'escalade.")]
        public float CoinChaosStepSeconds { get; set; } = 180f;

        [Description("Nombre maximal de paliers de chaos.")]
        public int CoinChaosMaxLevel { get; set; } = 5;

        [Description("Pieces ajoutees a coin_renewal_target par palier de chaos.")]
        public int CoinChaosTargetPerLevel { get; set; } = 2;

        [Description("Remplace du loot de la carte par des pieces. Laisser false si SCP500s s'en charge, sinon les deux plugins se disputent les memes objets et aucun n'atteint son quota.")]
        public bool SpawnCoinsOnMap { get; set; } = false;

        [Description("Nombre de pieces a placer sur la carte.")]
        public int MapCoinAmount { get; set; } = 6;

        [Description("Delai avant de placer les pieces apres le debut du round, en secondes.")]
        public float CoinSpawnDelay { get; set; } = 4f;

        [Description("Autorise le remplacement des trousses de soins par des pieces.")]
        public bool ReplaceMedkitsWithCoins { get; set; } = true;

        [Description("Autorise le remplacement des antidouleurs par des pieces.")]
        public bool ReplacePainkillersWithCoins { get; set; } = true;

        [Description("Autorise le remplacement des cartes de bas niveau par des pieces.")]
        public bool ReplaceWeakKeycardsWithCoins { get; set; } = true;

        [Description("Autorise le remplacement des lampes torches par des pieces.")]
        public bool ReplaceFlashlightsWithCoins { get; set; } = true;

        [Description("Pose une piece de plus a cote d'une partie du loot. Contrairement a spawn_coins_on_map, rien n'est detruit : cela cohabite avec SCP500s.")]
        public bool AddBonusCoinsToLoot { get; set; } = true;

        [Description("Nombre de pieces ajoutees sur la carte. Les pieces se consomment a l'usage : c'est ce qui les garde disponibles.")]
        public int BonusCoinAmount { get; set; } = 10;

        [Description("Types d'objets dont le loot recoit une piece de plus. Elle est posee a cote de l'objet, qui reste en place.")]
        public List<ItemType> BonusCoinSourceItems { get; set; } = new List<ItemType>
        {
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Flashlight,
            ItemType.Radio,
            ItemType.KeycardJanitor,
            ItemType.KeycardScientist,
        };

        [Description("Nombre maximal de pieces posees a cote d'objets du meme type. Evite que toutes les pieces tombent au meme endroit.")]
        public int BonusCoinsPerSourceType { get; set; } = 3;
    }
}