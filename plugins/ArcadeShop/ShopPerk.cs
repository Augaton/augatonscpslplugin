using System.ComponentModel;

namespace ArcadeShop
{
    public sealed class ShopPerk
    {
        [Description("Nom a taper apres .shop.")]
        public string Key { get; set; } = string.Empty;

        [Description("Nom affiche dans le catalogue.")]
        public string Label { get; set; } = string.Empty;

        [Description("Effet : Heal, Shield ou Speed.")]
        public PerkKind Kind { get; set; } = PerkKind.Heal;

        [Description("Vie rendue, bouclier donne ou vitesse ajoutee en pourcentage, de 1 a 100.")]
        public float Amount { get; set; } = 50f;

        [Description("Duree de Speed, en secondes.")]
        public float Seconds { get; set; } = 30f;

        [Description("Prix en points.")]
        public int Price { get; set; } = 20;

        [Description("Achats permis par joueur et par round. 0 = sans limite.")]
        public int Limit { get; set; }
    }
}
