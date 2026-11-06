using System.ComponentModel;

namespace ScpLevels
{
    public sealed class ScpLevel
    {
        [Description("Kills necessaires pour atteindre ce niveau.")]
        public int Kills { get; set; }

        [Description("Vitesse supplementaire a ce niveau, en pourcentage. Ne s'additionne pas d'un niveau a l'autre.")]
        public int MovementBoost { get; set; }

        [Description("PV maximum ajoutes en atteignant ce niveau, et soignes aussitot. S'additionnent d'un niveau a l'autre.")]
        public float BonusHealth { get; set; }

        [Description("PV regeneres par seconde a ce niveau. Ne s'additionnent pas d'un niveau a l'autre.")]
        public float RegenPerSecond { get; set; }
    }
}
