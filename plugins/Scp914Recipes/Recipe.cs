using System.ComponentModel;
using Scp914;

namespace Scp914Recipes
{
    public sealed class Recipe
    {
        [Description("Objet place dans la machine.")]
        public ItemType Input { get; set; } = ItemType.None;

        [Description("Reglage de la machine : Rough, Coarse, OneToOne, Fine ou VeryFine.")]
        public Scp914KnobSetting Setting { get; set; } = Scp914KnobSetting.OneToOne;

        [Description("Objet obtenu. SCP500 donne une vraie pilule si SCP500s est installe.")]
        public ItemType Output { get; set; } = ItemType.None;

        [Description("Chance de reussite en pourcentage. Sinon, la machine agit normalement.")]
        public float Chance { get; set; } = 100f;
    }
}
