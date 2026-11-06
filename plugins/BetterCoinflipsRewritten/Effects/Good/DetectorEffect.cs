using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class DetectorEffect : ICoinEffect
    {
        private float distance;

        public string Name
        {
            get { return "Detector"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .DetectorEffectMessage
                    .Replace("{distance}", Mathf.RoundToInt(distance).ToString());
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   TryFindNearest(player, out _);
        }

        public void Execute(Player player)
        {
            TryFindNearest(player, out distance);
        }

        // SCP-079 n'a pas de corps a localiser : seuls les roles FPC comptent.
        private static bool TryFindNearest(Player player, out float nearest)
        {
            nearest = float.MaxValue;

            foreach (Player scp in Player.ReadyList)
            {
                if (!scp.IsSCP || scp.RoleBase is not IFpcRole)
                    continue;

                nearest = Mathf.Min(nearest, Vector3.Distance(scp.Position, player.Position));
            }

            return nearest < float.MaxValue;
        }
    }
}
