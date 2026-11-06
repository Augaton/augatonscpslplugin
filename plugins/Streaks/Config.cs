using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Hints;

namespace Streaks
{
    public sealed class Config : IHintSettings
    {
        [Description("Recompenses de la serie. Un meme seuil ne compte qu'une fois.")]
        public List<StreakReward> Rewards { get; set; } = new List<StreakReward>
        {
            new StreakReward { Kills = 3, Item = ItemType.Medkit, Label = "une trousse de soins" },
            new StreakReward { Kills = 5, Item = ItemType.GrenadeHE, Label = "une grenade" },
            new StreakReward { Kills = 8, Item = ItemType.SCP500, Label = "une pilule SCP-500" },
            new StreakReward { Kills = 12, Item = ItemType.SCP207, Label = "un SCP-207" },
        };

        [Description("Kills d'affilee qui mettent une prime sur le joueur. 0 = jamais.")]
        public int BountyKills { get; set; } = 5;

        [Description("Objet donne a celui qui abat un joueur mis a prix.")]
        public ItemType BountyItem { get; set; } = ItemType.Adrenaline;

        [Description("Nombre d'objets de la prime, ou de munitions pour une boite.")]
        public int BountyQuantity { get; set; } = 1;

        [Description("Kills d'avance au round suivant pour le vainqueur d'un mode de jeu. 0 = aucun.")]
        public int KillsForGamemodeWin { get; set; } = 2;

        [Description("Ne compte rien pendant un mode de jeu.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 4f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 400f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
