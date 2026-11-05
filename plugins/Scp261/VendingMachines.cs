using System.Collections.Generic;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Object = UnityEngine.Object;

namespace Scp261
{
    public sealed class VendingMachines
    {
        public const string InteractableName = "Vending";

        private const string SchematicName = "Scp261";

        private static readonly Vector3 MachineSize = new Vector3(1f, 2f, 1f);

        private readonly Plugin plugin;
        private readonly List<GameObject> spawned = new List<GameObject>(2);

        public VendingMachines(Plugin plugin) => this.plugin = plugin;

        public int Count => spawned.Count;

        public void SpawnForRound()
        {
            Clear();

            List<SpawnLocation> locations = plugin.Config.SpawnLocations;

            if (locations is null || locations.Count == 0)
                return;

            if (plugin.Config.SpawnOnlyOnce)
            {
                SpawnLocation picked = AugatonLib.Random.Chance.PickWeighted(locations, location => Mathf.CeilToInt(location.Chance));

                if (picked is not null)
                    Spawn(picked);

                return;
            }

            foreach (SpawnLocation location in locations)
            {
                if (location is not null && AugatonLib.Random.Chance.Roll(location.Chance))
                    Spawn(location);
            }
        }

        public void Clear()
        {
            foreach (GameObject machine in spawned)
            {
                if (machine != null)
                    Object.Destroy(machine);
            }

            spawned.Clear();
        }

        private void Spawn(SpawnLocation location)
        {
            Room room = FindRoom(location);

            if (room is null)
            {
                Logger.Warn($"SCP-261 non place : salle {location.RoomName} introuvable.");
                return;
            }

            Vector3 position = room.Transform.TransformPoint(location.Position);
            Quaternion rotation = room.Transform.rotation * Quaternion.Euler(location.Rotation);

            if (ProjectMerBridge.TrySpawn(SchematicName, position, rotation, out GameObject schematic))
                spawned.Add(schematic);
            else
                SpawnFallback(position + (Vector3.up * (MachineSize.y / 2f)), rotation);
        }

        private void SpawnFallback(Vector3 center, Quaternion rotation)
        {
            if (!ColorUtility.TryParseHtmlString(plugin.Config.FallbackColor, out Color color))
                color = Color.red;

            PrimitiveObjectToy body = PrimitiveObjectToy.Create(center, rotation, MachineSize);

            body.Type = PrimitiveType.Cube;
            body.Color = color;

            // Un peu plus grand que le corps, pour que le regard du joueur l'atteigne en premier.
            InteractableToy button = InteractableToy.Create(center, rotation, MachineSize * 1.05f);

            button.GameObject.name = InteractableName;

            spawned.Add(body.GameObject);
            spawned.Add(button.GameObject);
        }

        private static Room FindRoom(SpawnLocation location)
        {
            foreach (Room room in Room.List)
            {
                if (room.Name == location.RoomName)
                    return room;
            }

            return null;
        }
    }
}
