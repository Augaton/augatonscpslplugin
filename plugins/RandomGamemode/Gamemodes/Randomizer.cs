using System.Collections.Generic;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class Randomizer : GamemodeBase
    {
        private static readonly RoleTypeId[] HumanRoles =
        {
            RoleTypeId.NtfCaptain, RoleTypeId.ChaosConscript, RoleTypeId.ClassD,
            RoleTypeId.FacilityGuard, RoleTypeId.Scientist,
        };

        private static readonly RoleTypeId[] ScpRoles =
        {
            RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.Scp096,
            RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939,
        };

        public override string Key => "randomizer";

        public override string DisplayName => "Randomiseur";

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            List<Player> candidates = new List<Player>(32);

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && !player.IsDestroyed)
                    candidates.Add(player);
            }

            if (candidates.Count == 0)
                yield break;

            Player scp = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            scp.SetRole(ScpRoles[UnityEngine.Random.Range(0, ScpRoles.Length)]);
            candidates.Remove(scp);

            int changed = 0;

            foreach (Player player in candidates)
            {
                if (player.IsDestroyed)
                    continue;

                player.SetRole(HumanRoles[UnityEngine.Random.Range(0, HumanRoles.Length)]);

                if (++changed % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            yield return Timing.WaitForSeconds(1.5f);

            IReadOnlyList<ItemType> pool = Config.RandomizerItemPool;

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive)
                    continue;

                if (player.Role == RoleTypeId.Scp0492)
                    MoveToSpawn(player, RoleTypeId.ClassD);
                else if (!player.IsSCP)
                    MoveToSpawn(player, HumanRoles[UnityEngine.Random.Range(0, HumanRoles.Length)]);

                if (player.IsSCP || pool.Count == 0)
                    continue;

                player.ClearInventory();
                player.AddItem(ItemType.KeycardO5);

                for (int i = 0; i < Config.RandomizerItemCount; i++)
                    player.AddItem(pool[UnityEngine.Random.Range(0, pool.Count)]);
            }

            yield return Timing.WaitForSeconds(2f);
            Announce(Settings.Description);
        }

        // CanEnd et non RoundEnding.IsAllowed : annuler RoundEnding laisse RoundSummary.IsRoundEnded a vrai,
        // ce qui ferait passer le round pour termine aupres de tous les plugins.
        public override void OnEndingRound(RoundEndingConditionsCheckEventArgs ev)
        {
            if (ev is null)
                return;

            int alive = 0;

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && player.IsAlive)
                    alive++;
            }

            if (alive > 1)
                ev.CanEnd = false;
        }
    }
}
