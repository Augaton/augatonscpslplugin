using LabApi.Features.Wrappers;
using UnityEngine;

namespace PocketCorpses
{
    internal sealed class FallingCorpse
    {
        public FallingCorpse(Ragdoll ragdoll, double dropTime)
        {
            Ragdoll = ragdoll;
            DropTime = dropTime;
        }

        public Ragdoll Ragdoll { get; }

        public double DropTime { get; set; }

        public Vector3? Roof { get; set; }
    }
}
