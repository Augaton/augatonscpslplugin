using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Hints;

namespace ArcadeShop
{
    public sealed class Config : IHintSettings
    {
        [Description("Points pour un humain ou un Chaos ennemi tue.")]
        public int PointsPerKill { get; set; } = 10;

        [Description("Points pour un SCP tue.")]
        public int PointsPerScpKill { get; set; } = 40;

        [Description("Points pour le premier joueur qui lance un generateur. Un generateur ne paie qu'une fois.")]
        public int PointsPerGenerator { get; set; } = 15;

        [Description("Points pour une evasion.")]
        public int PointsPerEscape { get; set; } = 30;

        [Description("Solde maximal d'un joueur.")]
        public int MaxBalance { get; set; } = 500;

        [Description("Delai entre deux commandes .shop d'achat d'un meme joueur, en secondes. 1 au plus bas.")]
        public float BuyCooldownSeconds { get; set; } = 2f;

        [Description("Objets en vente.")]
        public List<ShopItem> Items { get; set; } = new List<ShopItem>
        {
            new ShopItem { Key = "munitions", Label = "Munitions 9x19", Item = ItemType.Ammo9x19, Quantity = 30, Price = 5 },
            new ShopItem { Key = "antidouleur", Label = "Antidouleurs", Item = ItemType.Painkillers, Price = 10 },
            new ShopItem { Key = "soin", Label = "Trousse de soins", Item = ItemType.Medkit, Price = 15 },
            new ShopItem { Key = "adrenaline", Label = "Adrenaline", Item = ItemType.Adrenaline, Price = 20 },
            new ShopItem { Key = "flash", Label = "Grenade aveuglante", Item = ItemType.GrenadeFlash, Price = 20, Limit = 3 },
            new ShopItem { Key = "grenade", Label = "Grenade", Item = ItemType.GrenadeHE, Price = 30, Limit = 2 },
            new ShopItem { Key = "armure", Label = "Gilet de combat", Item = ItemType.ArmorCombat, Price = 30, Limit = 1 },
            new ShopItem { Key = "cola", Label = "SCP-207", Item = ItemType.SCP207, Price = 40, Limit = 1 },
            new ShopItem { Key = "pilule", Label = "Pilule SCP-500", Item = ItemType.SCP500, Price = 50, Limit = 1 },
        };

        [Description("Ferme la boutique et ne compte plus aucun point pendant un mode de jeu.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 3f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 900f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
