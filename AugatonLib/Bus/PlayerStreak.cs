using LabApi.Features.Wrappers;

namespace AugatonLib.Bus
{
    public sealed class PlayerStreak
    {
        public PlayerStreak(Player player, int kills)
        {
            Player = player;
            Kills = kills;
        }

        public Player Player { get; }

        public int Kills { get; }
    }
}
