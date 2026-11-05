using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class LockedIn : GamemodeBase
    {
        private static readonly RoleTypeId[] ScpAnchors =
        {
            RoleTypeId.Scp939, RoleTypeId.Scp049, RoleTypeId.Scp096,
            RoleTypeId.Scp173, RoleTypeId.Scp106,
        };

        public override string Key => "lockedin";

        public override string DisplayName => "Confinement total";

        public override IEnumerator<float> Run()
        {
            GateLock.Engage();

            yield return Timing.WaitForSeconds(4f);
            Announce(Settings.Description);
        }

        public override void Stop() => GateLock.Disengage();

        public override void OnDecontaminating(LczDecontaminationStartingEventArgs ev)
        {
            if (ev is not null)
                ev.IsAllowed = false;
        }

        public override void OnInteractingDoor(PlayerInteractingDoorEventArgs ev) => GateLock.Apply(ev);

        public override void OnRespawningTeam(WaveRespawningEventArgs ev)
        {
            if (ev?.Roles is null || ev.Wave is null)
                return;

            Faction faction = ev.Wave.Faction;
            List<string> userIds = new List<string>(ev.Roles.Count);

            foreach (Player player in ev.SpawningPlayers)
            {
                if (player is not null && !string.IsNullOrEmpty(player.UserId))
                    userIds.Add(player.UserId);
            }

            Delay(3f, () =>
            {
                foreach (string userId in userIds)
                {
                    Player target = Player.Get(userId);

                    if (target is null || target.IsDestroyed || !target.IsAlive)
                        continue;

                    RoleTypeId anchor = faction == Faction.FoundationStaff
                        ? RoleTypeId.FacilityGuard
                        : ScpAnchors[UnityEngine.Random.Range(0, ScpAnchors.Length)];

                    MoveToSpawn(target, anchor);
                }
            });
        }
    }
}
