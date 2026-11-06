using System.ComponentModel;

namespace ArcadeShop
{
    public sealed class ShopItem
    {
        [Description("Mot a taper apres .shop pour acheter cet objet.")]
        public string Key { get; set; } = string.Empty;

        [Description("Nom affiche au joueur.")]
        public string Label { get; set; } = string.Empty;

        [Description("Objet vendu. SCP500 donne une vraie pilule si SCP500s est installe.")]
        public ItemType Item { get; set; } = ItemType.None;

        [Description("Nombre d'objets, ou de munitions pour une boite.")]
        public int Quantity { get; set; } = 1;

        [Description("Prix en points.")]
        public int Price { get; set; } = 10;

        [Description("Achats par joueur et par round. 0 = sans limite.")]
        public int Limit { get; set; }
    }
}
