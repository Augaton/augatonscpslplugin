using AugatonLib.Hints;
using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using TeamGenocide.API;

namespace TeamGenocide
{
    public sealed class Config : IHintSettings
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Delai en secondes apres le debut du round avant que la detection soit active.")]
        public float ActivationDelay { get; set; } = 5f;

        [Description("N'annonce l'extinction d'une equipe qu'une seule fois par round.")]
        public bool AnnounceOncePerTeam { get; set; } = true;

        [Description("Joueurs requis pour annoncer un dernier survivant. 0 = jamais.")]
        public int LastSurvivorMinPlayers { get; set; } = 8;

        [Description("Equipes dont le dernier survivant est annonce et recompense.")]
        public List<Team> LastSurvivorTeams { get; set; } = new List<Team>
        {
            Team.ClassD,
            Team.Scientists,
            Team.FoundationForces,
            Team.ChaosInsurgency,
        };

        [Description("Bouclier donne au dernier survivant.")]
        public float LastSurvivorShield { get; set; } = 50f;

        [Description("Objet donne au dernier survivant. None = aucun.")]
        public ItemType LastSurvivorItem { get; set; } = ItemType.Adrenaline;

        [Description("Annonce a tout le serveur. %PLAYER% est le pseudo.")]
        public string LastSurvivorBroadcast { get; set; } = "<size=30><color=orange>%PLAYER% est le dernier de son equipe !</color></size>";

        [Description("Duree de cette annonce, en secondes.")]
        public ushort LastSurvivorBroadcastDuration { get; set; } = 5;

        [Description("Position verticale du hint dans HintServiceMeow. Doit differer des autres plugins.")]
        public float HintYCoordinate { get; set; } = 500f;

        [Description("Taille de police du hint.")]
        public int HintFontSize { get; set; } = 20;

        [Description("Annonces diffusees quand une equipe est entierement eliminee. Une entree est tiree au hasard.")]
        public Dictionary<Team, List<Announcement>> Announcements { get; set; } = new Dictionary<Team, List<Announcement>>
        {
            [Team.ClassD] = new List<Announcement>
            {
                new Announcement
                {
                    Cassie = "all class d personnel have been secured .",
                    Subtitle = "Tout le personnel de Classe-D a ete neutralise.",
                    Broadcast = "<color=orange>Tout le personnel de Classe-D a ete neutralise.</color>",
                    Hint = string.Empty,
                    DisplayTime = 12,
                    Lights = new LightEffect { Color = "#000000", Duration = 5f },
                },
            },
            [Team.SCPs] = new List<Announcement>
            {
                new Announcement
                {
                    Cassie = "all remaining s c p subjects have been contained .",
                    Subtitle = "Tous les SCP restants ont ete contenus.",
                    Broadcast = "<color=red>Tous les SCP restants ont ete contenus.</color>",
                    Hint = string.Empty,
                    DisplayTime = 12,
                    Lights = new LightEffect { Color = "#FF0000", Duration = 5f },
                },
            },
            [Team.Scientists] = new List<Announcement>
            {
                new Announcement
                {
                    Cassie = "all scientists have been terminated .",
                    Subtitle = "Tous les scientifiques ont ete elimines.",
                    Broadcast = "<color=yellow>Tous les scientifiques ont ete elimines.</color>",
                    DisplayTime = 12,
                    Lights = new LightEffect { Color = "#FFFF00", Duration = 5f },
                },
            },
            [Team.ChaosInsurgency] = new List<Announcement>
            {
                new Announcement
                {
                    Cassie = "all chaos insurgency forces have been eliminated .",
                    Subtitle = "Toutes les forces de l'Insurrection du Chaos ont ete eliminees.",
                    Broadcast = "<color=#28AD00>Toutes les forces de l'Insurrection du Chaos ont ete eliminees.</color>",
                    DisplayTime = 12,
                    Lights = new LightEffect { Color = "#28AD00", Duration = 5f },
                },
            },
            [Team.FoundationForces] = new List<Announcement>
            {
                new Announcement
                {
                    Cassie = "all foundation forces have been eliminated .",
                    Subtitle = "Toutes les forces de la Fondation ont ete eliminees.",
                    Broadcast = "<color=#0080FF>Toutes les forces de la Fondation ont ete eliminees.</color>",
                    DisplayTime = 12,
                    Lights = new LightEffect { Color = "#0080FF", Duration = 5f },
                },
            },
        };
    }
}
