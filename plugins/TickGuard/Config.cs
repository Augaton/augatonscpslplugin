using System.ComponentModel;

namespace TickGuard
{
    public sealed class Config
    {
        [Description("TPS sous lequel le serveur est considere en souffrance.")]
        public float MinTps { get; set; } = 30f;

        [Description("TPS a retrouver pour reprendre les evenements automatiques. Doit depasser min_tps.")]
        public float RecoveryTps { get; set; } = 45f;

        [Description("Secondes entre deux mesures.")]
        public float CheckSeconds { get; set; } = 5f;

        [Description("Mesures basses de suite avant l'alerte.")]
        public int ChecksBeforeAlert { get; set; } = 3;

        [Description("Suspend les evenements automatiques tant que le serveur souffre. Si false, le plugin ne fait qu'alerter.")]
        public bool PauseWorldEvents { get; set; } = true;
    }
}
