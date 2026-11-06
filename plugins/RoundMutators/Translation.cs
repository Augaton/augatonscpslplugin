using System.ComponentModel;

namespace RoundMutators
{
    public sealed class Translation
    {
        [Description("Hint d'annonce. %NAME% = nom du mutateur.")]
        public string Announcement { get; set; } = "Mutateur du round : %NAME%";

        [Description("Nom du mutateur de gravite.")]
        public string LowGravity { get; set; } = "gravite reduite";

        [Description("Nom du mutateur de vitesse.")]
        public string Swift { get; set; } = "pas rapides";

        [Description("Nom du mutateur de taille.")]
        public string Tiny { get; set; } = "petits gabarits";

        [Description("Nom du mutateur d'endurance.")]
        public string Tireless { get; set; } = "endurance sans fin";
    }
}
