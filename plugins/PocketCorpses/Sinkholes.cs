using System.Collections.Generic;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PocketCorpses
{
    internal sealed class Sinkholes
    {
        private const float FloorSearchRange = 30f;

        private readonly List<OpenSinkhole> open = new List<OpenSinkhole>();

        public void OpenBelow(Vector3 roof, double closeTime)
        {
            if (!Physics.Raycast(roof, Vector3.down, out RaycastHit floor, FloorSearchRange, FpcStateProcessor.Mask))
                return;

            SinkholeHazard hazard = SinkholeHazard.Spawn(floor.point, Quaternion.identity, Vector3.one);

            // SinkholeHazard.Spawn de LabAPI n'appelle pas NetworkServer.Spawn : sans cet appel, le gouffre
            // n'existerait que cote serveur et aucun joueur ne le verrait.
            NetworkServer.Spawn(hazard.Base.gameObject);
            open.Add(new OpenSinkhole(hazard, closeTime));
        }

        public void CloseExpired(double now)
        {
            for (int i = open.Count - 1; i >= 0; i--)
            {
                if (open[i].CloseTime > now)
                    continue;

                Close(open[i].Hazard);
                open.RemoveAt(i);
            }
        }

        public void CloseAll()
        {
            foreach (OpenSinkhole sinkhole in open)
                Close(sinkhole.Hazard);

            open.Clear();
        }

        // Hazard.Destroy leve une exception sur un gouffre deja detruit.
        private static void Close(SinkholeHazard hazard)
        {
            if (!hazard.IsDestroyed)
                hazard.Destroy();
        }

        private readonly struct OpenSinkhole
        {
            public OpenSinkhole(SinkholeHazard hazard, double closeTime)
            {
                Hazard = hazard;
                CloseTime = closeTime;
            }

            public SinkholeHazard Hazard { get; }

            public double CloseTime { get; }
        }
    }
}
