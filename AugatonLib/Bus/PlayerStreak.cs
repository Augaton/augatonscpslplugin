using LabApi.Features.Wrappers;

namespace AugatonLib.Bus
{
    public sealed class PlayerStreak
    {
        public PlayerStreak(Player player, int kills, bool isMilestone)
        {
            Player = player;
            Kills = kills;
            IsMilestone = isMilestone;
        }

        public Player Player { get; }

        public int Kills { get; }

        public bool IsMilestone { get; }
    }
}
