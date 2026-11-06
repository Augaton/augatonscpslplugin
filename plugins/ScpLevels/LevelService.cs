using System;
using System.Collections.Generic;
using AugatonLib.Arcade;
using AugatonLib.Runtime;
using AugatonLib.Text;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;

namespace ScpLevels
{
    public sealed class LevelService
    {
        private const float RegenTickSeconds = 1f;
        private const int MaxMovementBoost = 100;

        private readonly Plugin plugin;
        private readonly List<Player> leveled = new List<Player>(8);

        private CoroutineHandle regen;
        private bool running;

        public LevelService(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        private KillLadder<ScpLevel> Ladder => plugin.Ladder;

        public int LeveledCount => leveled.Count;

        public void Start()
        {
            Stop();

            running = true;
            regen = Timing.RunCoroutine(Regenerate());
        }

        public void Stop()
        {
            if (running)
                Timing.KillCoroutines(regen);

            running = false;
            leveled.Clear();
            Ladder.Clear();
        }

        public void OnDeath(PlayerDeathEventArgs ev)
        {
            try
            {
                Player scp = ev?.Attacker;

                if (scp is null || ev.Player is null || scp == ev.Player || !scp.IsSCP || !Round.IsRoundInProgress)
                    return;

                if (!ev.OldRole.IsHuman() || ev.OldRole == RoleTypeId.Tutorial)
                    return;

                if (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses)
                    return;

                if (Config.ExcludedRoles is not null && Config.ExcludedRoles.Contains(scp.Role))
                    return;

                AddKill(scp);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeath: {e}");
            }
        }

        // Le jeu retire les effets et remet les PV du role : les niveaux d'un role ne survivent pas a un autre.
        public void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                Drop(ev?.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangedRole: {e}");
            }
        }

        public void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                Drop(ev?.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        public void AddKill(Player scp)
        {
            ScpLevel level = Ladder.Add(scp.UserId, out _);

            if (level is null)
                return;

            Apply(scp, level);

            if (!leveled.Contains(scp))
                leveled.Add(scp);

            string text = plugin.Translation.LevelUp
                .Replace("%ROLE%", RoleNames.Full(scp.Role))
                .Replace("%LEVEL%", Ladder.LevelOf(scp.UserId).ToString());

            if (Config.AnnounceLevels)
                plugin.Hints.ShowAll(text, Config.HintDuration);
            else
                plugin.Hints.Show(scp, text, Config.HintDuration);
        }

        private static void Apply(Player scp, ScpLevel level)
        {
            if (level.BonusHealth > 0f)
            {
                scp.MaxHealth += level.BonusHealth;
                scp.Heal(level.BonusHealth);
            }

            if (level.MovementBoost > 0)
                scp.EnableEffect<MovementBoost>((byte)Math.Min(level.MovementBoost, MaxMovementBoost));
        }

        private void Drop(Player player)
        {
            if (player is null)
                return;

            leveled.Remove(player);
            Ladder.Forget(player.UserId);
        }

        private IEnumerator<float> Regenerate()
        {
            while (running)
            {
                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"LevelService.Tick: {e}");
                }

                yield return Timing.WaitForSeconds(RegenTickSeconds);
            }
        }

        private void Tick()
        {
            foreach (Player scp in leveled)
            {
                ScpLevel level = Ladder.CurrentStep(scp.UserId);

                if (level is not null && level.RegenPerSecond > 0f && !scp.IsDestroyed && scp.Health < scp.MaxHealth)
                    scp.Heal(level.RegenPerSecond * RegenTickSeconds);
            }
        }
    }
}
