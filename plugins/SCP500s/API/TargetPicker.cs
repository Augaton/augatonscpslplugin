using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MapGeneration;

namespace SCP500s.API
{
    public sealed class TargetPicker
    {
        private readonly List<Player> buffer = new List<Player>(32);

        private static Config Config => Plugin.Instance.Config;

        public bool IsEligible(Player player)
        {
            if (player is null || player.IsDestroyed || !player.IsAlive)
                return false;

            if (!Config.AllowScpTargets && player.IsSCP)
                return false;

            return !Config.IgnoredRoles.Contains(player.Role);
        }

        public static bool IsInDecontaminatedLcz(Player player)
        {
            return Decontamination.IsDecontaminating && player is not null && player.Zone == FacilityZone.LightContainment;
        }

        public Player PickRandomAlive(Player exclude, bool avoidDecontaminatedLcz = false)
        {
            buffer.Clear();

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player == exclude || !IsEligible(player))
                    continue;

                if (avoidDecontaminatedLcz && IsInDecontaminatedLcz(player))
                    continue;

                buffer.Add(player);
            }

            if (buffer.Count == 0)
                return null;

            Player chosen = buffer[UnityEngine.Random.Range(0, buffer.Count)];
            buffer.Clear();
            return chosen;
        }

        public void Clear() => buffer.Clear();
    }
}
