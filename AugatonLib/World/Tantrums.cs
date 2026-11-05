using Hazards;
using RelativePositioning;
using UnityEngine;

namespace AugatonLib.World
{
    public static class Tantrums
    {
        public static bool TrySpawn(Vector3 position)
        {
            TantrumEnvironmentalHazard prefab = NetworkPrefabs.Find<TantrumEnvironmentalHazard>();

            if (prefab == null)
                return false;

            TantrumEnvironmentalHazard tantrum = Object.Instantiate(prefab);

            // A poser avant le Start de la flaque, qui la met sur le reseau.
            tantrum.SynchronizedPosition = new RelativePosition(position + (Vector3.up * 0.25f));
            return true;
        }
    }
}
