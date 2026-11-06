using LabApi.Features.Wrappers;

namespace AugatonLib.Bus
{
    public sealed class ScpLevelUp
    {
        public ScpLevelUp(Player player, int level)
        {
            Player = player;
            Level = level;
        }

        public Player Player { get; }

        public int Level { get; }
    }
}
