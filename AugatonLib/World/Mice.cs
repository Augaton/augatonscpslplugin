using System.Reflection;
using UnityEngine;

namespace AugatonLib.World
{
    public static class Mice
    {
        // Prive dans le jeu et sans accesseur. SyncMouseSpawn lit ce tableau sur chaque client :
        // un type hors bornes y leverait une exception.
        private static readonly FieldInfo MiceField =
            typeof(SqueakSpawner).GetField("mice", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool TrySpawn(byte type)
        {
            SqueakSpawner spawner = Object.FindFirstObjectByType<SqueakSpawner>();

            if (spawner == null || type == 0)
                return false;

            if (!(MiceField?.GetValue(spawner) is GameObject[] mice) || type > mice.Length)
                return false;

            spawner.NetworksyncSpawn = type;
            return true;
        }
    }
}
