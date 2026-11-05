using System.ComponentModel;

namespace MicroHidDock
{
    public sealed class Translation
    {
        [Description("Message au joueur qui repose le Micro-HID.")]
        public string Docked { get; set; } = "<color=#7fd4ff>Micro-HID repose sur son socle.</color>";
    }
}
