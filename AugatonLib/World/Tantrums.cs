using Hazards;
using Mirror;
using RelativePositioning;
using UnityEngine;

namespace AugatonLib.World
{
    public static class Tantrums
    {
        // TantrumHazard.Spawn de LabAPI n'appelle pas NetworkServer.Spawn : la flaque n'existerait que
        // cote serveur, et aucun joueur ne la verrait.
        public static bool TrySpawn(Vector3 position)
        {
            TantrumEnvironmentalHazard prefab = FindPrefab();

            if (prefab == null)
                return false;

            TantrumEnvironmentalHazard tantrum = Object.Instantiate(prefab);

            tantrum.SynchronizedPosition = new RelativePosition(position + (Vector3.up * 0.25f));
            NetworkServer.Spawn(tantrum.gameObject);
            return true;
        }

        private static TantrumEnvironmentalHazard FindPrefab()
        {
            foreach (GameObject prefab in NetworkClient.prefabs.Values)
            {
                if (prefab.TryGetComponent(out TantrumEnvironmentalHazard hazard))
                    return hazard;
            }

            return null;
        }
    }
}
