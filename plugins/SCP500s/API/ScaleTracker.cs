using AugatonLib.Arbitration;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SCP500s.API
{
    public sealed class ScaleTracker
    {
        public void Apply(Player player, Vector3 scale) => ScaleArbiter.Request(player, Interop.Owner, scale);

        public void Restore(Player player) => ScaleArbiter.Release(player, Interop.Owner);

        public void Remove(Player player) => ScaleArbiter.Forget(player);

        public void Clear() => ScaleArbiter.ReleaseOwner(Interop.Owner);
    }
}
