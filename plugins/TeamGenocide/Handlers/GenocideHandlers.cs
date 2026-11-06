using System;
using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Text;
using AugatonLib.World;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using TeamGenocide.API;

namespace TeamGenocide.Handlers
{
    public sealed class GenocideHandlers
    {
        private readonly Plugin plugin;
        private readonly HashSet<Team> announced = new HashSet<Team>();
        private readonly HashSet<Team> lastSurvivors = new HashSet<Team>();
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

        public void OnLastSurvivor(BusMessage message)
        {
            if (!activated || !message.TryGet(out Player player))
                return;

            Config config = plugin.Config;

            if (config.LastSurvivorMinPlayers <= 0 || Player.Count < config.LastSurvivorMinPlayers)
                return;

            if (!config.LastSurvivorTeams.Contains(player.Team) || !lastSurvivors.Add(player.Team))
                return;

            if (config.LastSurvivorShield > 0f)
                player.CreateAhpProcess(config.LastSurvivorShield, config.LastSurvivorShield, AhpStat.DefaultDecay, AhpStat.DefaultEfficacy, 0f, false);

            if (config.LastSurvivorItem != ItemType.None)
                ItemGrant.Give(player, config.LastSurvivorItem);

            Server.SendBroadcast(config.LastSurvivorBroadcast.Replace("%PLAYER%", SafeText.Nickname(player)), config.LastSurvivorBroadcastDuration);
        }

        public void OnTeamWiped(BusMessage message)
        {
            if (!activated || message is null || !message.TryGet(out Team team))
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

            announced.Add(team);

            Announcement announcement = announcements[UnityEngine.Random.Range(0, announcements.Count)];

            if (announcement is null)
                return;

            CoroutineHandle handle = announcement.Announce();

            if (handle.IsRunning)
                pending.Add(handle);

            Logger.Info($"[TeamGenocide] Equipe {team} entierement eliminee.");
        }

        public void Reset()
        {
            lastSurvivors.Clear();

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
