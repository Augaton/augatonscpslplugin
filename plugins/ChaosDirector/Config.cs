using System.ComponentModel;

namespace ChaosDirector
{
    public sealed class Config
    {
        [Description("Points de chaos par pilule consommee.")]
        public int PointsPerPill { get; set; } = 1;

        [Description("Points de chaos par piece lancee.")]
        public int PointsPerCoinFlip { get; set; } = 1;

        [Description("Points de chaos par mort.")]
        public int PointsPerDeath { get; set; } = 1;

        [Description("Points de chaos par largage.")]
        public int PointsPerDrop { get; set; } = 3;

        [Description("Points de chaos par incident de site.")]
        public int PointsPerIncident { get; set; } = 3;

        [Description("Points de chaos par utilisation de SCP-261.")]
        public int PointsPerVending { get; set; } = 1;

        [Description("Points de chaos par equipe eliminee.")]
        public int PointsPerTeamWipe { get; set; } = 10;

        [Description("Points necessaires pour gagner un niveau de chaos.")]
        public int PointsPerLevel { get; set; } = 30;

        [Description("Niveau de chaos maximal.")]
        public int MaxBonusLevel { get; set; } = 5;

        [Description("Annonce chaque nouveau niveau de chaos aux joueurs.")]
        public bool AnnounceLevels { get; set; } = true;

        [Description("Duree du broadcast de niveau, en secondes.")]
        public ushort BroadcastDuration { get; set; } = 4;
    }
}
