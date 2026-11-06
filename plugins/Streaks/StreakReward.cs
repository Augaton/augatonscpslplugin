using System.ComponentModel;

namespace Streaks
{
    public sealed class StreakReward
    {
        [Description("Kills d'affilee pour obtenir cette recompense.")]
        public int Kills { get; set; }

        [Description("Objet donne. SCP500 donne une vraie pilule si SCP500s est installe.")]
        public ItemType Item { get; set; } = ItemType.Medkit;

        [Description("Nombre d'objets, ou de munitions pour une boite.")]
        public int Quantity { get; set; } = 1;

        [Description("Nom de la recompense affiche au joueur.")]
        public string Label { get; set; } = string.Empty;
    }
}
