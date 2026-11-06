using System.ComponentModel;

namespace SCP1162.API
{
    public sealed class RewardEntry
    {
        [Description("Objet rendu par SCP-1162.")]
        public ItemType Item { get; set; } = ItemType.Coin;

        [Description("Poids relatif du tirage. Un poids nul ou negatif retire l'objet du tirage.")]
        public int Weight { get; set; } = 1;

        [Description("Rang de cette recompense. Un objet depose tire dans le rang que lui donne item_tiers.")]
        public int Tier { get; set; }
    }
}
