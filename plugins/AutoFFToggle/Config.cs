using System.ComponentModel;

namespace AutoFFToggle
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Coupe le tir allie au demarrage du round.")]
        public bool DisableDuringRound { get; set; } = true;

        [Description("Reactive le tir allie a la fin du round.")]
        public bool EnableOnRoundEnd { get; set; } = true;

        [Description("Delai en secondes avant la reactivation du tir allie a la fin du round.")]
        public float EnableDelay { get; set; } = 0f;

        [Description("Restaure l'etat initial du tir allie au redemarrage du round et a la desactivation du plugin.")]
        public bool RestoreOnRoundRestart { get; set; } = true;

        [Description("Affiche un broadcast quand le tir allie est reactive.")]
        public bool AnnounceOnEnable { get; set; } = true;

        [Description("Duree du broadcast d'annonce, en secondes.")]
        public ushort AnnounceDuration { get; set; } = 8;
    }
}
