using System.Collections.Generic;
using System.ComponentModel;

namespace AmbientCassie
{
    public sealed class Translation
    {
        [Description("Annonces d'ambiance. collection check signale un mot que C.A.S.S.I.E ne connait pas.")]
        public List<AmbientLine> Lines { get; set; } = new List<AmbientLine>
        {
            new AmbientLine
            {
                Cassie = "attention . all personnel",
                Subtitles = "Attention. Tout le personnel doit appliquer les procedures de confinement.",
                Phase = AmbientPhase.Early,
            },
            new AmbientLine
            {
                Cassie = "attention . security personnel",
                Subtitles = "Personnel de securite : maintenez vos positions.",
            },
            new AmbientLine
            {
                Cassie = "attention . facility system",
                Subtitles = "Verification des systemes du site en cours.",
            },
            new AmbientLine
            {
                Cassie = "warning . containment status unknown",
                Subtitles = "Avertissement. Etat du confinement inconnu.",
                Phase = AmbientPhase.Late,
            },
            new AmbientLine
            {
                Cassie = "attention . facility status unknown",
                Subtitles = "Attention. Conditions anormales detectees sur le site.",
                Phase = AmbientPhase.Mutator,
            },
            new AmbientLine
            {
                Cassie = "danger . containment status unknown",
                Subtitles = "Danger. Une anomalie gagne en puissance.",
                Phase = AmbientPhase.ScpLevel,
            },
            new AmbientLine
            {
                Cassie = "danger . facility status unknown",
                Subtitles = "Danger. Situation du site incertaine. Evacuation recommandee.",
                Phase = AmbientPhase.Late,
            },
        };
    }
}
