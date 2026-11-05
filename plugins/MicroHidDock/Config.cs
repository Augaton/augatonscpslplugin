using System.ComponentModel;
using AugatonLib.Hints;

namespace MicroHidDock
{
    public sealed class Config : IHintSettings
    {
        [Description("Distance maximale au socle, en metres, pour y poser le Micro-HID en le lachant.")]
        public float DockRange { get; set; } = 2f;

        [Description("Delai entre deux depots d'un meme joueur, en secondes.")]
        public float DockCooldownSeconds { get; set; } = 5f;

        [Description("Energie rendue par seconde sur le socle. 0.01 = 1 % par seconde. 0 = pas de recharge.")]
        public float RechargePerSecond { get; set; } = 0.005f;

        [Description("Niveau d'energie ou la recharge s'arrete, de 0 a 1.")]
        public float RechargeLimit { get; set; } = 1f;

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 4f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 850f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
