using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RealisticSizes.API;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace RealisticSizes.Handlers
{
    public sealed class PlayerHandlers
    {
        private readonly Plugin plugin;
        private readonly Dictionary<string, CoroutineHandle> pending = new Dictionary<string, CoroutineHandle>();

        private int spawnCounter;

        public PlayerHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || string.IsNullOrEmpty(ev.Player.UserId))
                    return;

                Player player = ev.Player;
                RoleTypeId role = player.Role;

                if (!IsEligible(player, role))
                {
                    ResetScale(player);
                    return;
                }

                SizeRange range = ResolveRange(role);

                if (range is null)
                {
                    ResetScale(player);
                    return;
                }

                Vector3 scale = BuildScale(range);
                string userId = player.UserId;

                Cancel(userId);
                ScaleArbiter.Reset(player, ScaleArbiter.BaselineOf(player));

                pending[userId] = Timing.CallDelayed(NextDelay(), () =>
                {
                    try
                    {
                        pending.Remove(userId);

                        Player target = Player.Get(userId);

                        if (target is null || target.IsDestroyed || target.Role != role)
                            return;

                        ScaleArbiter.SetBaseline(target, scale);

                        Logger.Debug($"Taille appliquee a {target.Nickname} ({role}) : {scale}.", plugin.Config.Debug);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"OnChangedRole (application differee): {e}");
                    }
                });
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
                if (ev?.Player is null || string.IsNullOrEmpty(ev.Player.UserId))
                    return;

                Cancel(ev.Player.UserId);
                ScaleArbiter.Forget(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        public void OnRoundStarted() => spawnCounter = 0;

        public void OnRoundEnded(RoundEndedEventArgs ev) => Reset();

        public void OnRoundRestarted() => Reset();

        public void Reset()
        {
            try
            {
                Stop();
                ScaleArbiter.Clear();
            }
            catch (Exception e)
            {
                Logger.Error($"Reset: {e}");
            }
        }

        public void Stop()
        {
            foreach (KeyValuePair<string, CoroutineHandle> entry in pending)
                Timing.KillCoroutines(entry.Value);

            pending.Clear();
            spawnCounter = 0;
        }

        private bool IsEligible(Player player, RoleTypeId role)
        {
            if (role == RoleTypeId.None || role == RoleTypeId.Spectator)
                return false;

            if (plugin.Config.IgnoredRoles.Contains(role))
                return false;

            return plugin.Config.AffectScps || !player.IsSCP;
        }

        private SizeRange ResolveRange(RoleTypeId role)
        {
            switch (plugin.Config.ActiveMode)
            {
                case SizeMode.Roleplay:
                    return plugin.Config.RoleplayRange;

                case SizeMode.Fun:
                    return plugin.Config.FunRange;

                case SizeMode.Manual:
                    return plugin.Config.ManualRanges.TryGetValue(role, out SizeRange range) ? range : null;

                default:
                    return null;
            }
        }

        private Vector3 BuildScale(SizeRange range)
        {
            float height = UnityEngine.Random.Range(range.MinHeight, range.MaxHeight);

            if (!plugin.Config.AllowUnproportionalValues)
                return new Vector3(height, height, height);

            float width = UnityEngine.Random.Range(range.MinWidth, range.MaxWidth);

            return new Vector3(width, height, width);
        }

        private float NextDelay()
        {
            Config config = plugin.Config;
            float delay = Mathf.Max(0f, config.ApplyDelay);

            if (!config.SpreadWorkload || Player.Count < config.SpreadMinPlayers)
                return delay;

            spawnCounter++;

            return delay + (spawnCounter % 12 * Mathf.Max(0f, config.SpreadStep));
        }

        private void Cancel(string userId)
        {
            if (!pending.TryGetValue(userId, out CoroutineHandle handle))
                return;

            Timing.KillCoroutines(handle);
            pending.Remove(userId);
        }

        private static void ResetScale(Player player)
        {
            if (player is null)
                return;

            ScaleArbiter.Reset(player, Vector3.one);
        }
    }
}
