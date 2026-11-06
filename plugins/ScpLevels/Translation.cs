using System.ComponentModel;

namespace ScpLevels
{
    public sealed class Translation
    {
        [Description("Hint d'un nouveau niveau. %ROLE% = SCP, %LEVEL% = niveau.")]
        public string LevelUp { get; set; } = "%ROLE% passe au niveau %LEVEL% !";
    }
}
