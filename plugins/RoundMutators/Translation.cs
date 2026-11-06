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

        [Description("Nom du mutateur d'obscurite.")]
        public string NightFall { get; set; } = "nuit noire";

        [Description("Nom du mutateur de munitions.")]
        public string ScarceAmmo { get; set; } = "munitions rares";

        [Description("Nom du mutateur de soins.")]
        public string ScarceMedical { get; set; } = "soins rares";

        [Description("Nom du mutateur de vitesse des SCP.")]
        public string SwiftScps { get; set; } = "SCP rapides";

        [Description("Nom du mutateur de vie.")]
        public string FragileHumans { get; set; } = "humains fragiles";

        [Description("Nom du mutateur Vampire.")]
        public string Vampire { get; set; } = "vampires";

        [Description("Nom du mutateur GlassCannon.")]
        public string GlassCannon { get; set; } = "canons de verre";

        [Description("Nom du mutateur EndlessAmmo.")]
        public string EndlessAmmo { get; set; } = "munitions sans fin";

        [Description("Nom du mutateur Regeneration.")]
        public string Regeneration { get; set; } = "regeneration";
    }
}
