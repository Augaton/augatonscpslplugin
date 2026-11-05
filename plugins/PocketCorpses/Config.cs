using System.ComponentModel;

namespace PocketCorpses
{
    public sealed class Config
    {
        [Description("Chance, en pourcentage, qu'un corps laisse dans la dimension de poche retombe dans le complexe.")]
        public float ReturnChance { get; set; } = 100f;

        [Description("Delai minimal avant la chute d'un corps, en secondes.")]
        public float MinDelaySeconds { get; set; } = 30f;

        [Description("Delai maximal avant la chute d'un corps, en secondes.")]
        public float MaxDelaySeconds { get; set; } = 90f;

        [Description("Secondes entre l'alerte et la chute. Le jeu utilise 3 pour les objets.")]
        public float WarningSeconds { get; set; } = 3f;

        [Description("Ouvre un gouffre au sol avant la chute d'un corps.")]
        public bool SinkholeUnderCorpses { get; set; } = true;

        [Description("Ouvre un gouffre au sol avant la chute d'un objet rendu par la dimension de poche.")]
        public bool SinkholeUnderItems { get; set; } = false;

        [Description("Duree de vie d'un gouffre, en secondes.")]
        public float SinkholeSeconds { get; set; } = 8f;
    }
}
