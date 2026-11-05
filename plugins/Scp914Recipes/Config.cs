using System.Collections.Generic;
using System.ComponentModel;
using Scp914;

namespace Scp914Recipes
{
    public sealed class Config
    {
        [Description("Rend a SCP-914 son fonctionnement normal pendant un mode arene de RandomGamemode.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Recettes, testees dans l'ordre. Une pilule de SCP500s n'est pas une entree valable : le plugin la protege.")]
        public List<Recipe> Recipes { get; set; } = new List<Recipe>
        {
            new Recipe { Input = ItemType.Coin, Setting = Scp914KnobSetting.Fine, Output = ItemType.SCP500, Chance = 25f },
            new Recipe { Input = ItemType.Coin, Setting = Scp914KnobSetting.VeryFine, Output = ItemType.Medkit, Chance = 50f },
            new Recipe { Input = ItemType.Painkillers, Setting = Scp914KnobSetting.Rough, Output = ItemType.Coin },
            new Recipe { Input = ItemType.Medkit, Setting = Scp914KnobSetting.Rough, Output = ItemType.Coin },
        };
    }
}
