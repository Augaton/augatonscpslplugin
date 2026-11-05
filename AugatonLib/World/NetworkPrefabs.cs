using Mirror;
using UnityEngine;

namespace AugatonLib.World
{
    public static class NetworkPrefabs
    {
        public static T Find<T>()
            where T : Component
        {
            foreach (GameObject prefab in NetworkClient.prefabs.Values)
            {
                if (prefab.TryGetComponent(out T component))
                    return component;
            }

            return null;
        }
    }
}
