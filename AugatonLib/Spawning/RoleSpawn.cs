using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace AugatonLib.Spawning
{
    public static class RoleSpawn
    {
        public static bool TryGetPosition(RoleTypeId role, out Vector3 position) => TryGet(role, out position, out _);

        public static bool TryGet(RoleTypeId role, out Vector3 position, out float horizontalRotation)
        {
            position = default;
            horizontalRotation = 0f;

            return role.TryGetRoleTemplate(out FpcStandardRoleBase template)
                && template.SpawnpointHandler is not null
                && template.SpawnpointHandler.TryGetSpawnpoint(out position, out horizontalRotation);
        }
    }
}
