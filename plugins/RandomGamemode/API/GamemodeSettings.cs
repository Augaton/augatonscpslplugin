using System.ComponentModel;

namespace RandomGamemode.API
{
    public sealed class GamemodeSettings
    {
        [Description("Active ou desactive ce mode.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Poids relatif dans le tirage. 0 = jamais tire aleatoirement, mais reste forcable a la main.")]
        public int Weight { get; set; } = 10;

        [Description("Nombre minimum de joueurs pour que ce mode puisse etre tire.")]
        public int MinPlayers { get; set; } = 4;

        [Description("Nombre maximum de joueurs. 0 = sans limite.")]
        public int MaxPlayers { get; set; } = 0;

        [Description("Consigne affichee aux joueurs au demarrage du mode.")]
        public string Description { get; set; } = string.Empty;

        [Description("Met en pause les vagues de respawn pendant le mode.")]
        public bool PauseRespawnWaves { get; set; } = true;

        [Description("Active le tir allie pendant le mode.")]
        public bool ForceFriendlyFire { get; set; } = false;

        [Description("Duree maximale du mode en secondes avant fin forcee du round. 0 = pas de limite.")]
        public float RoundTimeLimit { get; set; } = 0f;

        [Description("Suspend les annonces d'extinction d'equipe pendant le mode. Les conversions de role scriptees ne sont pas des genocides.")]
        public bool SuppressGenocideAnnouncements { get; set; } = true;
    }
}
