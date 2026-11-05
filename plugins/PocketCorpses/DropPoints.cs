using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PocketCorpses
{
    internal static class DropPoints
    {
        private const float RoofSearchRange = 30f;
        private const float RoofClearance = 0.3f;

        public static bool IsInPocket(Vector3 position)
        {
            return position.TryGetRoom(out RoomIdentifier room) && room.Name == RoomName.Pocket;
        }

        // Comme pour les objets du jeu, le point de chute est le plafond au-dessus d'un joueur vivant.
        public static bool TryFindRoof(out Vector3 roof)
        {
            roof = default;
            int candidates = 0;

            foreach (Player player in Player.List)
            {
                if (player.RoleBase is not IFpcRole || IsInPocket(player.Position))
                    continue;

                if (!TryGetRoofAbove(player.Position, out Vector3 candidate))
                    continue;

                candidates++;

                if (Random.Range(0, candidates) == 0)
                    roof = candidate;
            }

            return candidates > 0;
        }

        public static bool TryGetRoofAbove(Vector3 position, out Vector3 roof)
        {
            roof = default;

            if (!Physics.Raycast(position, Vector3.up, out RaycastHit hit, RoofSearchRange, FpcStateProcessor.Mask))
                return false;

            roof = hit.point + (Vector3.down * RoofClearance);
            return true;
        }
    }
}
