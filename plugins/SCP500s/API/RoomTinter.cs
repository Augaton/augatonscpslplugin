using System.Collections.Generic;
using AugatonLib.Arbitration;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SCP500s.API
{
    public sealed class RoomTinter
    {
        private readonly Dictionary<Room, int> versions = new Dictionary<Room, int>();

        public int Tint(Room room, Color color)
        {
            if (room is null)
                return 0;

            versions.TryGetValue(room, out int version);
            versions[room] = ++version;

            LightArbiter.TintRoom(Interop.Owner, room, color);
            return version;
        }

        public void Reset(Room room, int version)
        {
            if (room is null || !versions.TryGetValue(room, out int current) || current != version)
                return;

            versions.Remove(room);
            LightArbiter.ReleaseRoomTint(Interop.Owner, room);
        }

        public void Clear()
        {
            versions.Clear();
            LightArbiter.ReleaseRoomTints(Interop.Owner);
        }
    }
}
