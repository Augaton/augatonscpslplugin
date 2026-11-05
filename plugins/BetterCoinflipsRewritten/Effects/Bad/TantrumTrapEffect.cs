using System;
using Hazards;
using LabApi.Features.Wrappers;
using Mirror;
using RelativePositioning;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class TantrumTrapEffect : ICoinEffect
    {
        public string Name
        {
            get { return "TantrumTrap"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .TantrumTrapEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive;
        }

        public void Execute(Player player)
        {
            TantrumEnvironmentalHazard prefab = FindPrefab();

            if (prefab == null)
            {
                throw new InvalidOperationException(
                    "The tantrum prefab was not found.");
            }

            TantrumEnvironmentalHazard tantrum =
                UnityEngine.Object.Instantiate(prefab);

            tantrum.SynchronizedPosition = new RelativePosition(
                player.Position + (Vector3.up * 0.25f));

            NetworkServer.Spawn(tantrum.gameObject);
        }

        // TantrumHazard.Spawn de LabAPI impose une rotation et n'appelle pas NetworkServer.Spawn :
        // la flaque n'existerait que cote serveur.
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
