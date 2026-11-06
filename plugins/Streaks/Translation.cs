using System.ComponentModel;

namespace Streaks
{
    public sealed class Translation
    {
        [Description("Hint du joueur qui atteint un palier. %COUNT% = kills, %REWARD% = recompense.")]
        public string RewardEarned { get; set; } = "Serie de %COUNT% : vous recevez %REWARD% !";

        [Description("Hint general quand un joueur est mis a prix. %PLAYER% = joueur, %COUNT% = kills.")]
        public string BountyPlaced { get; set; } = "Prime sur %PLAYER% : %COUNT% kills d'affilee !";

        [Description("Fin d'une longue serie. %PLAYER% est le joueur, %COUNT% ses kills.")]
        public string StreakEnded { get; set; } = "La serie de %COUNT% kills de %PLAYER% s'arrete.";

        [Description("Prime encaissee. %KILLER% est le tueur, %PLAYER% la cible.")]
        public string BountyClaimed { get; set; } = "%KILLER% a abattu %PLAYER% et empoche la prime !";
    }
}
