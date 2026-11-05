using System.Collections.Generic;

namespace SCP500s.API
{
    public static class DefaultPills
    {
        public static List<PillDefinition> Create()
        {
            return new List<PillDefinition>
            {
                new PillDefinition
                {
                    Id = 1,
                    Key = "shadow",
                    DisplayName = "SCP500-Shadow",
                    Description = "Traverse les portes, silhouette d'ombre, pendant dix secondes.",
                    GlowColor = "#FFA500",
                    SpawnWeight = 10,
                    MaxPerRound = 2,
                    Message = Messages.Shadow,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Ghostly", Intensity = 1, Duration = 10f },
                        new EffectEntry { Type = "Fade", Intensity = 150, Duration = 10f },
                    },
                },
                new PillDefinition
                {
                    Id = 2,
                    Key = "sonic",
                    DisplayName = "SCP500-Sonic",
                    Description = "Bonus de vitesse extreme de courte duree.",
                    GlowColor = "#FF00FF",
                    SpawnWeight = 10,
                    MaxPerRound = 2,
                    Message = Messages.Sonic,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "MovementBoost", Intensity = 30, Duration = 10f },
                        new EffectEntry { Type = "Invigorated", Intensity = 1, Duration = 10f },
                    },
                },
                new PillDefinition
                {
                    Id = 3,
                    Key = "traker",
                    DisplayName = "SCP500-Traker",
                    Description = "Permet de voir tous les joueurs a travers les murs.",
                    GlowColor = "#8E7CC3",
                    SpawnWeight = 8,
                    MaxPerRound = 1,
                    Message = Messages.Traker,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Scp1344", Intensity = 1, Duration = 15f },
                    },
                },
                new PillDefinition
                {
                    Id = 4,
                    Key = "super",
                    DisplayName = "Super SCP500",
                    Description = "Invisible, rapide et silencieux.",
                    GlowColor = "#00FFFF",
                    SpawnWeight = 5,
                    MaxPerRound = 1,
                    Message = Messages.Super,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Invisible", Intensity = 1, Duration = 7f },
                        new EffectEntry { Type = "SilentWalk", Intensity = 10, Duration = 7f },
                        new EffectEntry { Type = "MovementBoost", Intensity = 20, Duration = 7f },
                    },
                },
                new PillDefinition
                {
                    Id = 5,
                    Key = "santa",
                    DisplayName = "SCP500-Santa",
                    Description = "Offre un objet tire au hasard.",
                    GlowColor = "#FF69B4",
                    SpawnWeight = 8,
                    MaxPerRound = 1,
                    Message = Messages.Santa,
                    GrantOneRandomItem = true,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.ArmorHeavy,
                        ItemType.ArmorLight,
                        ItemType.GunAK,
                        ItemType.GunE11SR,
                        ItemType.Adrenaline,
                        ItemType.KeycardO5,
                        ItemType.SCP330,
                    },
                },
                new PillDefinition
                {
                    Id = 8,
                    Key = "yamato",
                    DisplayName = "SCP500-Yamato",
                    Description = "Augmente durablement votre vie maximale.",
                    GlowColor = "#00FF00",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Yamato,
                    MinimumMaxHealth = 150f,
                    Heal = 150f,
                },
                new PillDefinition
                {
                    Id = 13,
                    Key = "vitality",
                    DisplayName = "SCP500-Vitalite",
                    Description = "Immunite aux effets negatifs pendant trente secondes.",
                    GlowColor = "#7CFC00",
                    SpawnWeight = 12,
                    MaxPerRound = 3,
                    Message = Messages.Vitality,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Vitality", Intensity = 1, Duration = 30f },
                    },
                },
                new PillDefinition
                {
                    Id = 14,
                    Key = "adrenaline",
                    DisplayName = "SCP500-Adrenaline",
                    Description = "Resistance aux degats et vitesse accrue.",
                    GlowColor = "#FF4500",
                    SpawnWeight = 10,
                    MaxPerRound = 2,
                    Message = Messages.Adrenaline,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "DamageReduction", Intensity = 50, Duration = 15f },
                        new EffectEntry { Type = "MovementBoost", Intensity = 15, Duration = 15f },
                        new EffectEntry { Type = "Invigorated", Intensity = 1, Duration = 15f },
                    },
                },
                new PillDefinition
                {
                    Id = 15,
                    Key = "stamina",
                    DisplayName = "SCP500-Endurance",
                    Description = "Endurance illimitee pendant trente secondes.",
                    GlowColor = "#ADFF2F",
                    SpawnWeight = 12,
                    MaxPerRound = 3,
                    Message = Messages.Stamina,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Invigorated", Intensity = 1, Duration = 30f },
                    },
                },
                new PillDefinition
                {
                    Id = 17,
                    Key = "surgeon",
                    DisplayName = "SCP500-Chirurgien",
                    Description = "Soins, rechargement et maniement des armes acceleres pendant 45 secondes.",
                    GlowColor = "#40E0D0",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Surgeon,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Scp1853", Intensity = 1, Duration = 45f },
                    },
                },
                new PillDefinition
                {
                    Id = 18,
                    Key = "armor",
                    DisplayName = "SCP500-Blindage",
                    Description = "Vous equipe d'un gilet lourd.",
                    GlowColor = "#708090",
                    SpawnWeight = 6,
                    MaxPerRound = 1,
                    Message = Messages.Armor,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.ArmorHeavy,
                    },
                },
                new PillDefinition
                {
                    Id = 19,
                    Key = "arsenal",
                    DisplayName = "SCP500-Arsenal",
                    Description = "Une arme tiree au hasard.",
                    GlowColor = "#B8860B",
                    SpawnWeight = 4,
                    MaxPerRound = 1,
                    Message = Messages.Arsenal,
                    GrantOneRandomItem = true,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.GunCOM18,
                        ItemType.GunFSP9,
                        ItemType.GunCrossvec,
                        ItemType.GunE11SR,
                        ItemType.GunAK,
                    },
                },
                new PillDefinition
                {
                    Id = 20,
                    Key = "panacea",
                    DisplayName = "SCP500-Panacee",
                    Description = "Dissipe tous vos effets et vous soigne entierement.",
                    GlowColor = "#98FB98",
                    SpawnWeight = 6,
                    MaxPerRound = 1,
                    Message = Messages.Panacea,
                    CureNegativeEffects = true,
                    Heal = 200f,
                },
                new PillDefinition
                {
                    Id = 21,
                    Key = "juggernaut",
                    DisplayName = "SCP500-Juggernaut",
                    Description = "Vie maximale portee a 200 et degats reduits pendant une minute.",
                    GlowColor = "#4682B4",
                    SpawnWeight = 3,
                    MaxPerRound = 1,
                    Message = Messages.Juggernaut,
                    MinimumMaxHealth = 200f,
                    Heal = 200f,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "BodyshotReduction", Intensity = 4, Duration = 60f },
                        new EffectEntry { Type = "DamageReduction", Intensity = 40, Duration = 60f },
                    },
                },
                new PillDefinition
                {
                    Id = 38,
                    Key = "nightvision",
                    DisplayName = "SCP500-Nyctalope",
                    Description = "Vision nocturne et visee stabilisee.",
                    GlowColor = "#7FFF00",
                    SpawnWeight = 10,
                    MaxPerRound = 2,
                    Message = Messages.NightVision,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "NightVision", Intensity = 50, Duration = 60f },
                        new EffectEntry { Type = "FocusedVision", Intensity = 1, Duration = 60f },
                    },
                },
                new PillDefinition
                {
                    Id = 39,
                    Key = "passepartout",
                    DisplayName = "SCP500-Passe-partout",
                    Description = "Vous remet une carte O5.",
                    GlowColor = "#FFDAB9",
                    SpawnWeight = 4,
                    MaxPerRound = 1,
                    Message = Messages.Passepartout,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.KeycardO5,
                    },
                },
                new PillDefinition
                {
                    Id = 40,
                    Key = "feather",
                    DisplayName = "SCP500-Plume",
                    Description = "Vous ne pesez presque plus rien.",
                    GlowColor = "#F0FFFF",
                    SpawnWeight = 10,
                    MaxPerRound = 2,
                    Message = Messages.Feather,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Lightweight", Intensity = 60, Duration = 30f },
                        new EffectEntry { Type = "MovementBoost", Intensity = 10, Duration = 30f },
                    },
                },
                new PillDefinition
                {
                    Id = 41,
                    Key = "regeneration",
                    DisplayName = "SCP500-Regeneration",
                    Description = "Regenere 3 points de vie par seconde pendant vingt secondes.",
                    GlowColor = "#90EE90",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Regeneration,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "AnomalousRegeneration", Intensity = 1, Duration = 20f },
                    },
                    RegenerationPerSecond = 3f,
                    RegenerationSeconds = 20f,
                },
                new PillDefinition
                {
                    Id = 16,
                    Key = "cola",
                    DisplayName = "SCP500-Cola",
                    Description = "Vitesse et endurance infinie de SCP-207, qui ronge la vie en courant, jusqu'au prochain SCP-500. Ne pas boire d'Anti-SCP-207 par-dessus.",
                    GlowColor = "#FF1493",
                    SpawnWeight = 10,
                    MaxPerRound = 2,
                    Message = Messages.Cola,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Scp207", Intensity = 1, Duration = 0f },
                    },
                },
                new PillDefinition
                {
                    Id = 30,
                    Key = "anticola",
                    DisplayName = "SCP500-AntiCola",
                    Description = "Regeneration et bouclier d'Anti-SCP-207 jusqu'au prochain SCP-500. Ne pas boire de SCP-207 par-dessus.",
                    GlowColor = "#C71585",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.AntiCola,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "AntiScp207", Intensity = 1, Duration = 0f },
                    },
                },
                new PillDefinition
                {
                    Id = 8000,
                    Key = "lumien",
                    DisplayName = "SCP500-Lumien",
                    Description = "Vous fait passer definitivement dans le camp adverse.",
                    GlowColor = "#DC143C",
                    SpawnWeight = 4,
                    MaxPerRound = 1,
                    Message = Messages.Lumien,
                    Behaviour = PillBehaviour.Betrayal,
                },
                new PillDefinition
                {
                    Id = 7,
                    Key = "xerneas",
                    DisplayName = "SCP500-Xerneas",
                    Description = "Teleportation definitive dans une salle au hasard.",
                    GlowColor = "#0000FF",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Xerneas,
                    Behaviour = PillBehaviour.RandomTeleport,
                },
                new PillDefinition
                {
                    Id = 12,
                    Key = "disguise",
                    DisplayName = "SCP500-47",
                    Description = "Vous donne l'apparence d'un SCP jusqu'a votre mort.",
                    GlowColor = "#DA70D6",
                    SpawnWeight = 6,
                    MaxPerRound = 2,
                    Message = Messages.Disguise,
                    Behaviour = PillBehaviour.Disguise,
                },
                new PillDefinition
                {
                    Id = 42,
                    Key = "wave",
                    DisplayName = "SCP500-Renfort",
                    Description = "Declenche immediatement une vague de renfort, camp tire au sort.",
                    GlowColor = "#FFA07A",
                    SpawnWeight = 3,
                    MaxPerRound = 1,
                    Message = Messages.ForceWave,
                    Behaviour = PillBehaviour.ForceWave,
                },
                new PillDefinition
                {
                    Id = 6,
                    Key = "rakun",
                    DisplayName = "SCP500-Rakun",
                    Description = "Retrecit votre corps jusqu'a votre prochaine reapparition.",
                    GlowColor = "#9400D3",
                    SpawnWeight = 8,
                    MaxPerRound = 1,
                    Message = Messages.Rakun,
                    Behaviour = PillBehaviour.Shrink,
                },
                new PillDefinition
                {
                    Id = 43,
                    Key = "titan",
                    DisplayName = "SCP500-Titan",
                    Description = "Vous agrandit jusqu'a votre prochaine reapparition.",
                    GlowColor = "#DAA520",
                    SpawnWeight = 8,
                    MaxPerRound = 1,
                    Message = Messages.Titan,
                    Behaviour = PillBehaviour.Enlarge,
                },
                new PillDefinition
                {
                    Id = 44,
                    Key = "flamingo",
                    DisplayName = "SCP500-Flamant",
                    Description = "Vous transforme en flamant rose jusqu'a la mort. Evenementiel : Noel et premier avril uniquement.",
                    IsEnabled = false,
                    GlowColor = "#FF77A8",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Flamingo,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "BecomingFlamingo", Intensity = 1, Duration = 0f },
                    },
                },
                new PillDefinition
                {
                    Id = 45,
                    Key = "marshmallow",
                    DisplayName = "SCP500-Chamallow",
                    Description = "Vous transforme en guimauve jusqu'a la mort. Evenementiel : Halloween uniquement.",
                    IsEnabled = false,
                    GlowColor = "#FFF0F5",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Marshmallow,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "MarshmallowEffect", Intensity = 1, Duration = 0f },
                    },
                },
                new PillDefinition
                {
                    Id = 46,
                    Key = "metal",
                    DisplayName = "SCP500-Ferraille",
                    Description = "Votre peau devient metallique jusqu'a la mort. Evenementiel : Halloween uniquement.",
                    IsEnabled = false,
                    GlowColor = "#B0C4DE",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Metal,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Metal", Intensity = 1, Duration = 0f },
                    },
                },
                new PillDefinition
                {
                    Id = 47,
                    Key = "prism",
                    DisplayName = "SCP500-Prisme",
                    Description = "Le monde prend toutes les couleurs a la fois.",
                    GlowColor = "#FF00AA",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Prism,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "RainbowTaste", Intensity = 2, Duration = 45f },
                        new EffectEntry { Type = "Blurred", Intensity = 1, Duration = 45f },
                    },
                },
                new PillDefinition
                {
                    Id = 48,
                    Key = "snow",
                    DisplayName = "SCP500-Blizzard",
                    Description = "Il se met a neiger autour de vous. Evenementiel : Noel et premier avril uniquement.",
                    IsEnabled = false,
                    GlowColor = "#E0FFFF",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Snow,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Snowed", Intensity = 1, Duration = 40f },
                    },
                },
                new PillDefinition
                {
                    Id = 49,
                    Key = "sugar",
                    DisplayName = "SCP500-Sucre",
                    Description = "Une montee de sucre incontrolable. Evenementiel : Halloween uniquement.",
                    IsEnabled = false,
                    GlowColor = "#FFB6C1",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Sugar,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "SugarRush", Intensity = 1, Duration = 25f },
                        new EffectEntry { Type = "SugarHigh", Intensity = 1, Duration = 25f },
                    },
                },
                new PillDefinition
                {
                    Id = 57,
                    Key = "fog",
                    DisplayName = "SCP500-Brouillard",
                    Description = "Noie le site dans un brouillard epais a vos yeux.",
                    GlowColor = "#C0C0C0",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Fog,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "FogControl", Intensity = 5, Duration = 30f },
                    },
                },
                new PillDefinition
                {
                    Id = 58,
                    Key = "spectre",
                    DisplayName = "SCP500-Spectre",
                    Description = "Vous fait entendre les joueurs deja morts.",
                    GlowColor = "#9370DB",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Spectre,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Scp1576", Intensity = 1, Duration = 30f },
                    },
                },
                new PillDefinition
                {
                    Id = 50,
                    Key = "disco",
                    DisplayName = "SCP500-Disco",
                    Description = "Repeint les lumieres de la salle.",
                    GlowColor = "#FF3EB5",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Disco,
                    Behaviour = PillBehaviour.DiscoLights,
                },
                new PillDefinition
                {
                    Id = 51,
                    Key = "blackout",
                    DisplayName = "SCP500-Panne",
                    Description = "Coupe les lumieres de votre zone.",
                    GlowColor = "#36454F",
                    SpawnWeight = 6,
                    MaxPerRound = 1,
                    Message = Messages.Blackout,
                    Behaviour = PillBehaviour.Blackout,
                },
                new PillDefinition
                {
                    Id = 52,
                    Key = "mice",
                    DisplayName = "SCP500-Souris",
                    Description = "Fait apparaitre des souris dans l'abri.",
                    GlowColor = "#D2B48C",
                    SpawnWeight = 6,
                    MaxPerRound = 1,
                    Message = Messages.Mice,
                    Behaviour = PillBehaviour.Mice,
                },
                new PillDefinition
                {
                    Id = 53,
                    Key = "tantrum",
                    DisplayName = "SCP500-Tantrum",
                    Description = "Laisse une flaque de SCP-173 derriere vous.",
                    GlowColor = "#4F7942",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Tantrum,
                    Behaviour = PillBehaviour.Tantrum,
                },
                new PillDefinition
                {
                    Id = 54,
                    Key = "fireworks",
                    DisplayName = "SCP500-Feu d'artifice",
                    Description = "Une explosion purement decorative.",
                    GlowColor = "#FFD700",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Fireworks,
                    Behaviour = PillBehaviour.Fireworks,
                },
                new PillDefinition
                {
                    Id = 55,
                    Key = "cassie",
                    DisplayName = "SCP500-Cassie",
                    Description = "Declenche une annonce absurde dans tout le site.",
                    GlowColor = "#87CEEB",
                    SpawnWeight = 6,
                    MaxPerRound = 1,
                    Message = Messages.Cassie,
                    Behaviour = PillBehaviour.CassiePrank,
                },
                new PillDefinition
                {
                    Id = 56,
                    Key = "contagion",
                    DisplayName = "SCP500-Contagion",
                    Description = "Tout le monde autour de vous prend le meme effet.",
                    GlowColor = "#9ACD32",
                    SpawnWeight = 6,
                    MaxPerRound = 1,
                    Message = Messages.Contagion,
                    Behaviour = PillBehaviour.Contagion,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "RainbowTaste", Intensity = 2, Duration = 20f },
                        new EffectEntry { Type = "Blurred", Intensity = 1, Duration = 20f },
                    },
                },
                new PillDefinition
                {
                    Id = 36,
                    Key = "swap",
                    DisplayName = "SCP500-Permutation",
                    Description = "Echange votre position avec un joueur au hasard.",
                    GlowColor = "#00CED1",
                    SpawnWeight = 5,
                    MaxPerRound = 1,
                    Message = Messages.Swap,
                    Behaviour = PillBehaviour.Swap,
                },
                new PillDefinition
                {
                    Id = 37,
                    Key = "summon",
                    DisplayName = "SCP500-Invocation",
                    Description = "Attire un joueur au hasard jusqu'a vous.",
                    GlowColor = "#E9967A",
                    SpawnWeight = 5,
                    MaxPerRound = 1,
                    Message = Messages.Summon,
                    Behaviour = PillBehaviour.Summon,
                },
                new PillDefinition
                {
                    Id = 60,
                    Key = "bouclier",
                    DisplayName = "SCP500-Bouclier",
                    Description = "Vous entoure de points de vie artificiels qui s'erodent lentement.",
                    GlowColor = "#5F9EA0",
                    SpawnWeight = 12,
                    MaxPerRound = 3,
                    Message = Messages.Shield,
                    ArtificialHealth = 60f,
                },
                new PillDefinition
                {
                    Id = 61,
                    Key = "munitions",
                    DisplayName = "SCP500-Munitions",
                    Description = "Remplit vos reserves de munitions pour vos armes, ou de tous calibres sans arme.",
                    GlowColor = "#DAA520",
                    SpawnWeight = 10,
                    MaxPerRound = 3,
                    Message = Messages.Ammo,
                    Behaviour = PillBehaviour.AmmoRefill,
                },
                new PillDefinition
                {
                    Id = 62,
                    Key = "recensement",
                    DisplayName = "SCP500-Recensement",
                    Description = "Revele combien de survivants compte chaque camp.",
                    GlowColor = "#87CEFA",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = string.Empty,
                    Behaviour = PillBehaviour.Census,
                },
                new PillDefinition
                {
                    Id = 63,
                    Key = "radar",
                    DisplayName = "SCP500-Radar",
                    Description = "Indique a quelle distance se trouvent les SCP encore en vie.",
                    GlowColor = "#FF6347",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = string.Empty,
                    Behaviour = PillBehaviour.ScpRadar,
                },
                new PillDefinition
                {
                    Id = 64,
                    Key = "promotion",
                    DisplayName = "SCP500-Promotion",
                    Description = "Remplace votre meilleure carte d'acces par la suivante.",
                    GlowColor = "#FFD700",
                    SpawnWeight = 6,
                    MaxPerRound = 2,
                    Message = Messages.Promotion,
                    Behaviour = PillBehaviour.KeycardUpgrade,
                },
                new PillDefinition
                {
                    Id = 65,
                    Key = "infirmier",
                    DisplayName = "SCP500-Infirmier",
                    Description = "Donne une trousse de soins et des antidouleurs.",
                    GlowColor = "#FF7F7F",
                    SpawnWeight = 10,
                    MaxPerRound = 3,
                    Message = Messages.Medic,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.Medkit,
                        ItemType.Painkillers,
                    },
                },
                new PillDefinition
                {
                    Id = 66,
                    Key = "sourdine",
                    DisplayName = "SCP500-Sourdine",
                    Description = "Rend vos pas silencieux pendant une minute.",
                    GlowColor = "#778899",
                    SpawnWeight = 10,
                    MaxPerRound = 2,
                    Message = Messages.Silence,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "SilentWalk", Intensity = 10, Duration = 60f },
                    },
                },
                new PillDefinition
                {
                    Id = 67,
                    Key = "camouflage",
                    DisplayName = "SCP500-Camouflage",
                    Description = "Rend votre silhouette translucide pendant trente secondes.",
                    GlowColor = "#E6E6FA",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Camouflage,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Fade", Intensity = 180, Duration = 30f },
                    },
                },
                new PillDefinition
                {
                    Id = 68,
                    Key = "vaccin",
                    DisplayName = "SCP500-Vaccin",
                    Description = "Guerit vos maux et vous protege des effets negatifs pendant une minute.",
                    GlowColor = "#00FA9A",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Vaccine,
                    CureNegativeEffects = true,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Vitality", Intensity = 1, Duration = 60f },
                        new EffectEntry { Type = "RainbowTaste", Intensity = 3, Duration = 60f },
                    },
                },
                new PillDefinition
                {
                    Id = 69,
                    Key = "ration",
                    DisplayName = "SCP500-Ration",
                    Description = "Petit soin immediat et quelques points de vie artificiels.",
                    GlowColor = "#F4A460",
                    SpawnWeight = 16,
                    MaxPerRound = 5,
                    Message = Messages.Ration,
                    Heal = 40f,
                    ArtificialHealth = 20f,
                },
                new PillDefinition
                {
                    Id = 70,
                    Key = "marathon",
                    DisplayName = "SCP500-Marathon",
                    Description = "Endurance infinie et foulee allongee pendant une minute.",
                    GlowColor = "#32CD32",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Marathon,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "Invigorated", Intensity = 1, Duration = 60f },
                        new EffectEntry { Type = "MovementBoost", Intensity = 10, Duration = 60f },
                    },
                },
                new PillDefinition
                {
                    Id = 71,
                    Key = "pieces",
                    DisplayName = "SCP500-Tirelire",
                    Description = "Deux pieces pour tenter votre chance a pile ou face.",
                    GlowColor = "#C0C0C0",
                    SpawnWeight = 8,
                    MaxPerRound = 3,
                    Message = Messages.Coins,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.Coin,
                        ItemType.Coin,
                    },
                },
                new PillDefinition
                {
                    Id = 72,
                    Key = "bonbons",
                    DisplayName = "SCP500-Friandise",
                    Description = "Un sachet de bonbons SCP-330.",
                    GlowColor = "#FFB6C1",
                    SpawnWeight = 6,
                    MaxPerRound = 2,
                    Message = Messages.Candy,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.SCP330,
                    },
                },
                new PillDefinition
                {
                    Id = 73,
                    Key = "radio",
                    DisplayName = "SCP500-Talkie",
                    Description = "Une radio pour rester en contact.",
                    GlowColor = "#6495ED",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Radio,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.Radio,
                    },
                },
                new PillDefinition
                {
                    Id = 74,
                    Key = "lampe",
                    DisplayName = "SCP500-Lanterne",
                    Description = "Une lampe torche pour les zones sans lumiere.",
                    GlowColor = "#FFFACD",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Lamp,
                    GrantedItems = new List<ItemType>
                    {
                        ItemType.Flashlight,
                    },
                },
                new PillDefinition
                {
                    Id = 75,
                    Key = "clairvoyance",
                    DisplayName = "SCP500-Clairvoyance",
                    Description = "Dissipe le brouillard a vos yeux pendant une minute : vous voyez bien plus loin.",
                    GlowColor = "#E0FFFF",
                    SpawnWeight = 8,
                    MaxPerRound = 2,
                    Message = Messages.Clairvoyance,
                    Effects = new List<EffectEntry>
                    {
                        new EffectEntry { Type = "FogControl", Intensity = 1, Duration = 60f },
                    },
                },
                new PillDefinition
                {
                    Id = 99,
                    Key = "chaos",
                    DisplayName = "SCP500-Roulette",
                    Description = "Effet tire au sort : bon, mauvais ou completement absurde.",
                    GlowColor = "#FFFF00",
                    SpawnWeight = 0,
                    MaxPerRound = 0,
                    Message = Messages.Chaos,
                },
            };
        }

        public static List<string> RandomOnlyKeys()
        {
            return new List<string>
            {
                "lumien",
                "xerneas",
                "swap",
                "summon",
                "wave",
                "blackout",
                "tantrum",
                "contagion",
                "disguise",
                "prism",
                "fog",
            };
        }

        public static List<PillDefinition> AddMissing(List<PillDefinition> current)
        {
            if (current is null)
                return null;

            HashSet<uint> ids = new HashSet<uint>();
            HashSet<string> keys = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (PillDefinition pill in current)
            {
                if (pill is null)
                    continue;

                ids.Add(pill.Id);

                if (!string.IsNullOrEmpty(pill.Key))
                    keys.Add(pill.Key);
            }

            foreach (PillDefinition pill in Create())
            {
                if (ids.Contains(pill.Id) || keys.Contains(pill.Key))
                    continue;

                current.Add(pill);
            }

            return current;
        }
    }
}

#pragma warning restore CS0618
