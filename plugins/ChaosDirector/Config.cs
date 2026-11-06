using System.Collections.Generic;
using System.ComponentModel;

namespace ChaosDirector
{
    public sealed class Config
    {
        [Description("Ne compte rien pendant un mode arene de RandomGamemode.")]
        public bool PauseDuringGamemodes { get; set; } = true;

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

        [Description("Points de chaos par contrat rempli.")]
        public int PointsPerContract { get; set; } = 1;

        [Description("Points de chaos par niveau gagne par un SCP.")]
        public int PointsPerScpLevel { get; set; } = 3;

        [Description("Points de chaos par echange a SCP-1162.")]
        public int PointsPerExchange { get; set; } = 1;

        [Description("Points de chaos par palier atteint dans une serie de kills.")]
        public int PointsPerStreak { get; set; } = 2;

        [Description("Points de chaos par utilisation de SCP-261.")]
        public int PointsPerVending { get; set; } = 1;

        [Description("Points de chaos par equipe eliminee.")]
        public int PointsPerTeamWipe { get; set; } = 10;

        [Description("Points necessaires pour gagner un niveau de chaos.")]
        public int PointsPerLevel { get; set; } = 30;

        [Description("Niveau de chaos maximal.")]
        public int MaxBonusLevel { get; set; } = 5;

        [Description("Incidents de SiteIncidents tires quand le chaos atteint son niveau maximal. Vide = aucun.")]
        public List<string> ApogeeIncidents { get; set; } = new List<string> { "Blackout", "Alert", "Lockdown", "TeslaSurge" };

        [Description("Profils de SupplyDrop tires quand le chaos atteint son niveau maximal. Vide = aucun.")]
        public List<string> ApogeeDrops { get; set; } = new List<string> { "mtf", "chaos", "neutre" };

        [Description("Annonce chaque nouveau niveau de chaos aux joueurs.")]
        public bool AnnounceLevels { get; set; } = true;

        [Description("Duree du broadcast de niveau, en secondes.")]
        public ushort BroadcastDuration { get; set; } = 4;
    }
}
