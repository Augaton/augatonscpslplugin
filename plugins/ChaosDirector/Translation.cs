using System.ComponentModel;

namespace ChaosDirector
{
    public sealed class Translation
    {
        [Description("Broadcast d'un nouveau niveau. %LEVEL% est remplace par le niveau atteint.")]
        public string LevelUp { get; set; } = "<color=red>Le chaos monte : niveau %LEVEL%</color>";
    }
}
