using System.ComponentModel;

namespace RealisticSizes.API
{
    public sealed class SizeRange
    {
        [Description("Taille verticale minimale.")]
        public float MinHeight { get; set; } = 0.9f;

        [Description("Taille verticale maximale.")]
        public float MaxHeight { get; set; } = 1.1f;

        [Description("Largeur minimale, appliquee aux axes X et Z.")]
        public float MinWidth { get; set; } = 0.9f;

        [Description("Largeur maximale, appliquee aux axes X et Z.")]
        public float MaxWidth { get; set; } = 1.1f;
    }
}
