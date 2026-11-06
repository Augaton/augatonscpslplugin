using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.API
{
    public static class SecondChance
    {
        private static readonly HashSet<string> Armed = new HashSet<string>();

        public static bool IsArmed(Player player) => player?.UserId is not null && Armed.Contains(player.UserId);

        public static void Arm(Player player)
        {
            if (player?.UserId is not null)
                Armed.Add(player.UserId);
        }

        public static bool TryConsume(Player player) => player?.UserId is not null && Armed.Remove(player.UserId);

        public static void Clear() => Armed.Clear();
    }
}
