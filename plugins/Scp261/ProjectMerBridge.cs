using System;
using System.Reflection;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace Scp261
{
    // ProjectMER reste optionnel : sans reference de compilation, ce plugin se charge aussi sans lui.
    internal static class ProjectMerBridge
    {
        private const string SpawnerType = "ProjectMER.Features.ObjectSpawner";

        private static MethodInfo spawnSchematic;

        public static bool TrySpawn(string schematic, Vector3 position, Quaternion rotation, out GameObject spawned)
        {
            spawned = null;
            spawnSchematic ??= FindSpawnMethod();

            if (spawnSchematic is null)
                return false;

            try
            {
                spawned = (spawnSchematic.Invoke(null, new object[] { schematic, position, rotation }) as Component)?.gameObject;
            }
            catch (TargetInvocationException e)
            {
                Logger.Warn($"Schema {schematic} indisponible : {e.InnerException?.Message}");
            }

            return spawned != null;
        }

        private static MethodInfo FindSpawnMethod()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetName().Name == "ProjectMER" ? assembly.GetType(SpawnerType, false) : null;

                if (type is not null)
                    return type.GetMethod("SpawnSchematic", new[] { typeof(string), typeof(Vector3), typeof(Quaternion) });
            }

            return null;
        }
    }
}
