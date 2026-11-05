using System.ComponentModel;

namespace RoundRecap
{
    public sealed class Config
    {
        [Description("Duree du broadcast de bilan, en secondes.")]
        public ushort BroadcastDuration { get; set; } = 10;
    }
}
