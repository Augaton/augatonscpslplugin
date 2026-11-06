using System.ComponentModel;

namespace AmbientCassie
{
    public sealed class AmbientLine
    {
        [Description("Phrase prononcee par C.A.S.S.I.E, en mots qu'elle connait.")]
        public string Cassie { get; set; } = string.Empty;

        [Description("Sous-titres affiches aux joueurs.")]
        public string Subtitles { get; set; } = string.Empty;

        [Description("Moment du round : Any, Early ou Late. Mutator et ScpLevel repondent a un evenement.")]
        public AmbientPhase Phase { get; set; } = AmbientPhase.Any;

        [Description("Poids dans le tirage.")]
        public int Weight { get; set; } = 10;
    }
}
