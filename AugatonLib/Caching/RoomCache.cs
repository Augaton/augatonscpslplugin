using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MapGeneration;

namespace AugatonLib.Caching
{
    public sealed class RoomCache
    {
        private readonly List<Room> rooms = new List<Room>(64);

        public int Count => rooms.Count;

        public void Rebuild()
        {
            rooms.Clear();

            foreach (Room room in Room.List)
            {
                if (room is null || IsExcluded(room))
                    continue;

                rooms.Add(room);
            }
        }

        public void Clear() => rooms.Clear();

        public Room PickRandom()
        {
            if (rooms.Count == 0)
                Rebuild();

            if (rooms.Count == 0)
                return null;

            if (!Decontamination.IsDecontaminating)
                return rooms[UnityEngine.Random.Range(0, rooms.Count)];

            int safe = 0;

            foreach (Room room in rooms)
            {
                if (room.Zone != FacilityZone.LightContainment)
                    safe++;
            }

            if (safe == 0)
                return null;

            int target = UnityEngine.Random.Range(0, safe);

            foreach (Room room in rooms)
            {
                if (room.Zone == FacilityZone.LightContainment)
                    continue;

                if (target == 0)
                    return room;

                target--;
            }

            return null;
        }

        private static bool IsExcluded(Room room)
        {
            return room.Zone == FacilityZone.None
                || room.Zone == FacilityZone.Surface
                || room.Name == RoomName.Pocket;
        }
    }
}
