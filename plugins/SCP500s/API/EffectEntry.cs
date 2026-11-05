using System.ComponentModel;

namespace SCP500s.API
{
    public sealed class EffectEntry
    {
        [Description("Nom de la classe de l'effet (CustomPlayerEffects), par exemple Scp207, Blindness ou Sinkhole. Un nom inconnu est signale au chargement puis ignore.")]
        public string Type { get; set; } = string.Empty;

        [Description("Intensite de l'effet (1-255).")]
        public byte Intensity { get; set; } = 1;

        [Description("Duree en secondes. 0 = actif jusqu'a la mort.")]
        public float Duration { get; set; } = 5f;
    }
}
