using CustomPlayerEffects;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using SingleUseKeycardItem = InventorySystem.Items.Keycards.SingleUseKeycardItem;

namespace RemoteKeycard.API.Extensions
{
    public static class PlayerExtensions
    {
        public static bool HasKeycardPermission(this Player player, DoorPermissionFlags required, bool requiresAll = false)
        {
            if (player is null || required == DoorPermissionFlags.None)
                return false;

            Config.Config config = Plugin.Instance.Config;

            if (config.AmnesiaMatters && player.HasEffect<AmnesiaItems>())
                return false;

            if (config.IgnoredRoles.Contains(player.Role))
                return false;

            if (config.RequireHeldKeycard)
                return player.CurrentItem is KeycardItem held && Matches(held, required, requiresAll);

            foreach (Item item in player.Items)
            {
                if (item is KeycardItem keycard && Matches(keycard, required, requiresAll))
                    return true;
            }

            return false;
        }

        private static bool Matches(KeycardItem keycard, DoorPermissionFlags required, bool requiresAll)
        {
            if (keycard.Base is SingleUseKeycardItem)
                return false;

            DoorPermissionFlags owned = keycard.Permissions;

            return requiresAll ? owned.HasFlagAll(required) : owned.HasFlagAny(required);
        }
    }
}
