using System.ComponentModel;

namespace AutoFFToggle
{
    public sealed class Translation
    {
        [Description("Broadcast diffuse a tous quand le tir allie est reactive en fin de round.")]
        public string FriendlyFireEnabled { get; set; } = "<color=red><b>Le tir allie est active.</b></color>";
    }
}
