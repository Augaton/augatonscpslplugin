using AugatonLib.Arbitration;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Enums;

namespace RandomGamemode.API
{
    public static class GateLock
    {
        public static void Engage() => DoorLockRegistry.Lock(Interop.GateOwner, DoorName.EzGateA, DoorName.EzGateB);

        public static void Disengage() => DoorLockRegistry.Unlock(Interop.GateOwner);

        public static void Apply(PlayerInteractingDoorEventArgs ev)
        {
            if (ev?.Door is null)
                return;

            // CanOpen et non IsAllowed : refuser l'event ferait sortir le jeu sans le retour "acces refuse".
            if (DoorLockRegistry.IsLocked(ev.Door.DoorName))
                ev.CanOpen = false;
        }
    }
}
