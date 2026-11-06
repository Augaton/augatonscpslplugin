using AugatonLib.Hints;
using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode
{
    public sealed class Config : IHintSettings
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Chance en pourcent qu un mode special soit tire au debut de chaque round.")]
        public int GamemodeChance { get; set; } = 10;

        [Description("Quand un mode est tire, les joueurs votent dans le lobby au lieu de le subir.")]
        public bool EnableVote { get; set; } = true;

        [Description("Nombre de modes proposes au vote (2-9).")]
        public int VoteOptions { get; set; } = 3;

        [Description("Duree du broadcast de vote, en secondes.")]
        public ushort VoteBroadcastDuration { get; set; } = 10;

        [Description("Delai minimal en secondes entre deux votes d'un joueur.")]
        public float VoteCommandCooldownSeconds { get; set; } = 1f;

        [Description("Texte annonce au demarrage d un mode. {0} est remplace par son nom.")]
        public string StartText { get; set; } = "<size=35><color=red>Mode special : {0}</color></size>";

        [Description("Texte annonce a la fin d un mode. {0} est remplace par son nom.")]
        public string EndText { get; set; } = "<size=30><color=red>Fin du mode {0}.</color></size>";

        [Description("Duree des annonces de debut et de fin, en secondes.")]
        public ushort HeaderDuration { get; set; } = 8;

        [Description("Position verticale du hint dans HintServiceMeow. Doit differer des autres plugins.")]
        public float HintYCoordinate { get; set; } = 450f;

        [Description("Taille de police du hint.")]
        public int HintFontSize { get; set; } = 20;

        [Description("Duree en secondes de chaque cycle de panne de lumiere.")]
        public float BlackoutDuration { get; set; } = 60f;

        [Description("Nombre d objets aleatoires donnes par le mode Randomiseur.")]
        public int RandomizerItemCount { get; set; } = 6;

        [Description("Objets pouvant etre tires par le mode Randomiseur.")]
        public List<ItemType> RandomizerItemPool { get; set; } = new List<ItemType>
        {
            ItemType.GunCOM18,
            ItemType.GunFSP9,
            ItemType.GunCrossvec,
            ItemType.GunE11SR,
            ItemType.GunAK,
            ItemType.GunLogicer,
            ItemType.GunRevolver,
            ItemType.GunShotgun,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Adrenaline,
            ItemType.SCP500,
            ItemType.SCP207,
            ItemType.SCP268,
            ItemType.SCP018,
            ItemType.SCP330,
            ItemType.SCP1853,
            ItemType.GrenadeHE,
            ItemType.GrenadeFlash,
            ItemType.ArmorLight,
            ItemType.ArmorCombat,
            ItemType.ArmorHeavy,
            ItemType.Radio,
            ItemType.Flashlight,
            ItemType.Jailbird,
            ItemType.MicroHID,
        };

        [Description("Vie des zombies du mode Infection.")]
        public float InfectionZombieHealth { get; set; } = 400f;

        [Description("Echelle des zombies du mode Infection.")]
        public float InfectionZombieScale { get; set; } = 1.15f;

        [Description("Cache-cache : secondes laissees aux joueurs pour se cacher.")]
        public float HideAndSeekHideSeconds { get; set; } = 45f;

        [Description("Cache-cache : secondes de traque avant que les survivants ne gagnent.")]
        public float HideAndSeekHuntSeconds { get; set; } = 240f;

        [Description("Cache-cache : role du chasseur.")]
        public RoleTypeId HideAndSeekSeekerRole { get; set; } = RoleTypeId.Scp939;

        [Description("Evacuation : secondes avant le depart de l'ogive.")]
        public float EvacuationDelaySeconds { get; set; } = 60f;

        [Description("Patate chaude : secondes avant l'explosion de la bombe.")]
        public float HotPotatoFuseSeconds { get; set; } = 25f;

        [Description("Patate chaude : pause entre deux bombes, en secondes.")]
        public float HotPotatoPauseSeconds { get; set; } = 5f;

        [Description("Patate chaude : bonus de vitesse du porteur, de 0 a 255.")]
        public byte HotPotatoSpeedBoost { get; set; } = 25;

        [Description("Un traitre parmi nous : munitions de chaque joueur.")]
        public ushort TraitorAmmo { get; set; } = 36;

        [Description("Un traitre parmi nous : duree de l'enquete, en secondes.")]
        public float TraitorRoundSeconds { get; set; } = 360f;

        [Description("Roi de la salle : secondes de tenue pour gagner.")]
        public int KingOfTheHillTarget { get; set; } = 120;

        [Description("Roi de la salle : secondes avant de revenir apres une mort.")]
        public float KingOfTheHillRespawnSeconds { get; set; } = 6f;

        [Description("Roi de la salle : couleur de la salle disputee.")]
        public string KingOfTheHillColor { get; set; } = "#FFD700";

        [Description("Escorte : secondes pour faire sortir le scientifique.")]
        public float EscortRoundSeconds { get; set; } = 420f;

        [Description("Zones condamnees : secondes entre deux zones.")]
        public float ClosingZoneSeconds { get; set; } = 120f;

        [Description("Zones condamnees : part de la vie maximale perdue par seconde dans une zone condamnee, en pourcentage.")]
        public float ClosingZoneDamagePercent { get; set; } = 3f;

        [Description("Zones condamnees : couleur d'une zone condamnee.")]
        public string ClosingZoneColor { get; set; } = "#FF2200";

        [Description("Vie du joueur Juggernaut.")]
        public float JuggernautHealth { get; set; } = 2500f;

        [Description("Echelle du joueur Juggernaut.")]
        public float JuggernautScale { get; set; } = 1.3f;

        [Description("Intensite de la reduction de degats du Juggernaut (1-255).")]
        public byte JuggernautDamageReduction { get; set; } = 40;

        [Description("Vie des joueurs en mode Gun Game.")]
        public float GunGameHealth { get; set; } = 120f;

        [Description("Palier d armes du mode Gun Game, dans l ordre de progression.")]
        public List<ItemType> GunGameLadder { get; set; } = new List<ItemType>
        {
            ItemType.GunCOM18,
            ItemType.GunFSP9,
            ItemType.GunCrossvec,
            ItemType.GunRevolver,
            ItemType.GunShotgun,
            ItemType.GunAK,
            ItemType.GunE11SR,
            ItemType.GunLogicer,
            ItemType.Jailbird,
        };

        [Description("Nombre d objets personnalises donnes par le mode Arsenal experimental.")]
        public int CustomItemRushCount { get; set; } = 5;

        [Description("Nombre de pilules donnees par le mode Soiree pilules.")]
        public int PillPartyCount { get; set; } = 6;

        [Description("Chance en pourcent qu une pilule de la Soiree pilules soit un objet personnalise plutot qu un SCP-500 vanilla.")]
        public int PillPartyCustomChance { get; set; } = 70;

        [Description("Nombre de pieces donnees par le mode Pile ou face royale.")]
        public int CoinRoyaleCount { get; set; } = 5;

        [Description("Delai en secondes entre les deux factions du mode Guerre des factions.")]
        public float CustomTeamWarDelay { get; set; } = 20f;

        [Description("Reglages par mode. Une entree absente est recreee avec les valeurs par defaut.")]
        public Dictionary<string, GamemodeSettings> Gamemodes { get; set; } = new Dictionary<string, GamemodeSettings>
        {
            ["dodgeball"] = new GamemodeSettings
            {
                Weight = 10,
                MinPlayers = 6,
                Description = "Eliminez vos adversaires a coups de SCP-018. Le dernier debout gagne.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
                RoundTimeLimit = 0f,
            },
            ["peanutraid"] = new GamemodeSettings
            {
                Weight = 10,
                MinPlayers = 6,
                Description = "Les cacahuetes doivent arreter les Classe-D avant qu'ils ne s'echappent.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["bluescreen"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 8,
                Description = "Les scientifiques ont quinze minutes pour desactiver SCP-079.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = false,
                RoundTimeLimit = 900f,
            },
            ["livingnerd"] = new GamemodeSettings
            {
                Weight = 10,
                MinPlayers = 6,
                Description = "Les Classe-D doivent survivre au scientifique tueur dans le noir.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["randomizer"] = new GamemodeSettings
            {
                Weight = 10,
                MinPlayers = 6,
                Description = "Roles, objets et positions au hasard. Tir allie actif. Le dernier vivant gagne.",
                PauseRespawnWaves = false,
                ForceFriendlyFire = true,
                RoundTimeLimit = 0f,
            },
            ["mimicry"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Les Classe-D n'ont qu'un Jailbird. Mourir vous transforme en SCP-939.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["lockedin"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 8,
                Description = "Les portails sont scelles et la decontamination desactivee. Personne ne sort.",
                PauseRespawnWaves = false,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["infection"] = new GamemodeSettings
            {
                Weight = 10,
                MinPlayers = 6,
                Description = "Chaque mort rejoint les rangs de SCP-049-2. Survivez le plus longtemps possible.",
                PauseRespawnWaves = false,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["larry"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "SCP-106 traque les Classe-D dans la zone de confinement legere.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["customrolechaos"] = new GamemodeSettings
            {
                Weight = 6,
                MinPlayers = 8,
                Description = "Tout le monde recoit un role personnalise au hasard. Bonne chance.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["customteamwar"] = new GamemodeSettings
            {
                Weight = 5,
                MinPlayers = 12,
                Description = "Deux factions personnalisees sont larguees dans le complexe. Choisissez votre camp.",
                PauseRespawnWaves = false,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["customitemrush"] = new GamemodeSettings
            {
                Weight = 6,
                MinPlayers = 6,
                Description = "Inventaire rempli d'objets personnalises. Personne ne sait ce qu'il tient.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
                RoundTimeLimit = 0f,
            },
            ["pillparty"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Tout le monde est bourre de pilules SCP-500. Avalez et priez.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
                RoundTimeLimit = 0f,
            },
            ["coinroyale"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Une poignee de pieces chacun. Que le sort decide.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
                RoundTimeLimit = 0f,
            },
            ["juggernaut"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Un joueur surpuissant contre tous les autres. Abattez-le.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
                RoundTimeLimit = 0f,
            },
            ["blackout"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Panne totale et definitive. Lampes torches distribuees.",
                PauseRespawnWaves = false,
                ForceFriendlyFire = false,
                RoundTimeLimit = 0f,
            },
            ["hideandseek"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 5,
                Description = "Cachez-vous et taisez-vous : le chasseur arrive. Survivez jusqu'au bout.",
                PauseRespawnWaves = true,
            },
            ["evacuation"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "L'ogive va partir, et rien ne l'arretera. Rejoignez la surface.",
                PauseRespawnWaves = true,
            },
            ["hotpotato"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 4,
                Description = "Une bombe circule : tirez sur quelqu'un pour la lui passer. Le dernier survivant gagne.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
            },
            ["traitor"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Un traitre se cache parmi vous. Demasquez-le, mais chaque innocent abattu desarme son tireur.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
            },
            ["kingofthehill"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Tenez la salle designee plus longtemps que l'autre camp.",
                PauseRespawnWaves = true,
            },
            ["escort"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Le NTF protege le scientifique, le Chaos doit l'abattre avant sa sortie.",
                PauseRespawnWaves = true,
            },
            ["closingzones"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Les zones vont etre condamnees une par une. Rejoignez la surface.",
                PauseRespawnWaves = true,
            },
            ["gungame"] = new GamemodeSettings
            {
                Weight = 8,
                MinPlayers = 6,
                Description = "Chaque elimination ameliore votre arme. Le premier au bout du palier gagne.",
                PauseRespawnWaves = true,
                ForceFriendlyFire = true,
                RoundTimeLimit = 0f,
            },
        };
    }
}
