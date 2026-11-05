using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class TeleportToEscapeEffect : ICoinEffect
    {
        public string Name
        {
            get { return "TeleportToEscape"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .TeleportToEscapeEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            if (player is null ||
                player.IsDestroyed ||
                !player.IsAlive)
            {
                return false;
            }

            return Door.Get(
                DoorName.SurfaceEscapeSecondary) is not null;
        }

        public void Execute(Player player)
        {
            Door escapeDoor =
                Door.Get(DoorName.SurfaceEscapeSecondary);

            if (escapeDoor is null)
                return;

            player.Position = escapeDoor.Position + Vector3.up;
        }
    }
}
