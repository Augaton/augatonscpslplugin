using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Hints;

namespace SpectatorVote
{
    public sealed class Config : IHintSettings
    {
        [Description("Delai avant le premier vote, en secondes.")]
        public float FirstVoteSeconds { get; set; } = 240f;

        [Description("Delai entre deux votes, en secondes. Au moins la duree d'un vote plus 30 secondes.")]
        public float VoteIntervalSeconds { get; set; } = 240f;

        [Description("Duree d'un vote, en secondes, 10 au plus bas.")]
        public float VoteDurationSeconds { get; set; } = 25f;

        [Description("Choix proposes a chaque vote, 2 au plus bas.")]
        public int OptionCount { get; set; } = 3;

        [Description("Spectateurs requis pour ouvrir un vote.")]
        public int MinSpectators { get; set; } = 2;

        [Description("Delai entre deux commandes .vi d'un meme joueur, en secondes. 1 au plus bas.")]
        public float VoteCommandCooldownSeconds { get; set; } = 1f;

        [Description("Choix proposes au vote. CameraOutage n'y est pas : sans SCP-079, il ne ferait rien.")]
        public List<IncidentOption> Options { get; set; } = new List<IncidentOption>
        {
            new IncidentOption { Incident = "Blackout", Label = "Panne d'eclairage" },
            new IncidentOption { Incident = "Alert", Label = "Alerte rouge" },
            new IncidentOption { Incident = "Lockdown", Label = "Portes bloquees" },
            new IncidentOption { Incident = "GasLeak", Label = "Fuite de gaz" },
            new IncidentOption { Incident = "TeslaSurge", Label = "Surcharge des portes Tesla" },
            new IncidentOption { Incident = "ElevatorOutage", Label = "Ascenseurs hors service" },
            new IncidentOption { Kind = VoteOptionKind.SupplyDrop, Incident = "mtf", Label = "Largage MTF" },
            new IncidentOption { Kind = VoteOptionKind.SupplyDrop, Incident = "chaos", Label = "Largage du Chaos" },
            new IncidentOption { Kind = VoteOptionKind.SupplyDrop, Incident = "neutre", Label = "Largage neutre" },
            new IncidentOption { Kind = VoteOptionKind.Mutator, Incident = "SwiftScps", Label = "SCP acceleres" },
            new IncidentOption { Kind = VoteOptionKind.Mutator, Incident = "NightFall", Label = "Nuit noire" },
            new IncidentOption { Kind = VoteOptionKind.Blessing, Incident = "HealHumans", Label = "Soigner les vivants" },
            new IncidentOption { Kind = VoteOptionKind.Blessing, Incident = "BlindScps", Label = "Aveugler les SCP" },
        };

        [Description("Vie rendue aux humains par le choix HealHumans.")]
        public float BlessingHeal { get; set; } = 30f;

        [Description("Duree de l'aveuglement des SCP par le choix BlindScps, en secondes, de 1 a 25.")]
        public float BlessingBlindSeconds { get; set; } = 5f;

        [Description("Suspend les votes pendant un mode de jeu.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Duree du hint de resultat, en secondes.")]
        public float HintDuration { get; set; } = 5f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 250f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
