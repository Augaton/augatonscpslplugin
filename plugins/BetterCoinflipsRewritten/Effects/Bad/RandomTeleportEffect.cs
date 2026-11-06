using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;
using MapGeneration;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class RandomTeleportEffect : ICoinEffect
    {
        private string selectedRoomName;

        public RandomTeleportEffect()
        {
            selectedRoomName =
                Plugin.Instance.Translation.UnknownRoomName;
        }

        public string Name
        {
            get { return "RandomTeleport"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .RandomTeleportEffectMessage
                    .Replace(
                        "{room}",
                        selectedRoomName);
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   !Warhead.IsDetonated &&
                   RoomCache.Count > 0;
        }

        public void Execute(Player player)
        {
            Room selectedRoom = RoomCache.PickRandom();

            if (selectedRoom is null)
            {
                selectedRoomName =
                    Plugin.Instance.Translation.UnknownRoomName;
                return;
            }

            selectedRoomName = DisplayName(selectedRoom);
            player.Position = selectedRoom.Position + Vector3.up;
        }

        // Tous les couloirs portent le nom Unnamed : la zone et la forme les distinguent.
        private static string DisplayName(Room room)
        {
            return room.Name == RoomName.Unnamed
                ? room.Zone + " " + room.Shape
                : room.Name.ToString();
        }
    }
}
