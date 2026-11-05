using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using TeamGenocide.API;

namespace TeamGenocide.Handlers
{
    public sealed class GenocideHandlers
    {
        private readonly Plugin plugin;
        private readonly HashSet<Team> announced = new HashSet<Team>();
        private readonly List<CoroutineHandle> pending = new List<CoroutineHandle>(8);

        private CoroutineHandle activationHandle;
        private bool activationScheduled;
        private bool activated;

        public GenocideHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnRoundStarted()
        {
            try
            {
                Reset();

                activationScheduled = true;
                activationHandle = Timing.CallDelayed(plugin.Config.ActivationDelay, () =>
                {
                    activationScheduled = false;
                    activated = true;
                });
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev) => Reset();

        public void OnRoundRestarted() => Reset();

        public void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            try
            {
                if (!activated || ev?.Player is null || ev.ChangeReason != RoleChangeReason.Died)
                    return;

                Team leaving = ev.OldRole.Team;

                if (leaving == Team.Dead || ev.NewRole.GetTeam() == leaving)
                    return;

                AnnounceIfWiped(leaving, ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangingRole: {e}");
            }
        }

        public void OnTeamCheckRequested(BusMessage message)
        {
            if (activated && message is not null && message.TryGet(out Team team))
                AnnounceIfWiped(team, null);
        }

        private void AnnounceIfWiped(Team team, Player excluded)
        {
            if (GenocideArbiter.IsSuppressed || GenocideArbiter.IsHeld(team))
                return;

            Config config = plugin.Config;

            if (config.AnnounceOncePerTeam && announced.Contains(team))
                return;

            if (!config.Announcements.TryGetValue(team, out List<Announcement> announcements)
                || announcements is null
                || announcements.Count == 0)
            {
                return;
            }

            if (CountTeam(team, excluded) > 0)
                return;

            announced.Add(team);

            Announcement announcement = announcements[UnityEngine.Random.Range(0, announcements.Count)];

            if (announcement is null)
                return;

            CoroutineHandle handle = announcement.Announce();

            if (handle.IsRunning)
                pending.Add(handle);

            PluginBus.Publish(BusTopics.TeamWiped, "TeamGenocide", team);

            Logger.Info($"[TeamGenocide] Equipe {team} entierement eliminee.");
        }

        private static int CountTeam(Team team, Player excluded)
        {
            int count = 0;

            foreach (Player player in Player.ReadyList)
            {
                if (player == excluded)
                    continue;

                if (player.Team == team)
                    count++;
            }

            return count;
        }

        public void Reset()
        {
            if (activationScheduled)
            {
                Timing.KillCoroutines(activationHandle);
                activationScheduled = false;
            }

            foreach (CoroutineHandle handle in pending)
                Timing.KillCoroutines(handle);

            pending.Clear();
            announced.Clear();
            activated = false;

            try
            {
                plugin.Hints.Clear();
                LightEffect.Release();
            }
            catch (Exception e)
            {
                Logger.Error($"Reset: {e}");
            }
        }
    }
}
