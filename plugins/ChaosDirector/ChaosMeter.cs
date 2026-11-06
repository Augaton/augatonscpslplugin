using System;
using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Chaos;
using AugatonLib.Random;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace ChaosDirector
{
    public sealed class ChaosMeter
    {
        private readonly Plugin plugin;

        public ChaosMeter(Plugin plugin) => this.plugin = plugin;

        public int Points { get; private set; }

        private Config Config => plugin.Config;

        public void Subscribe(string owner)
        {
            PluginBus.Subscribe(owner, BusTopics.PillConsumed, _ => Add(Config.PointsPerPill));
            PluginBus.Subscribe(owner, BusTopics.CoinFlipped, _ => Add(Config.PointsPerCoinFlip));
            PluginBus.Subscribe(owner, BusTopics.SupplyDropped, _ => Add(Config.PointsPerDrop));
            PluginBus.Subscribe(owner, BusTopics.SiteIncident, _ => Add(Config.PointsPerIncident));
            PluginBus.Subscribe(owner, BusTopics.TeamWiped, _ => Add(Config.PointsPerTeamWipe));
            PluginBus.Subscribe(owner, BusTopics.VendingUsed, _ => Add(Config.PointsPerVending));
            PluginBus.Subscribe(owner, BusTopics.ItemExchanged, _ => Add(Config.PointsPerExchange));
            PluginBus.Subscribe(owner, BusTopics.ContractCompleted, _ => Add(Config.PointsPerContract));
            PluginBus.Subscribe(owner, BusTopics.ScpLevelReached, _ => Add(Config.PointsPerScpLevel));
            PluginBus.Subscribe(owner, BusTopics.StreakReached, OnStreakReached);
        }

        public void OnDeath(PlayerDeathEventArgs ev)
        {
            try
            {
                Add(Config.PointsPerDeath);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeath: {e}");
            }
        }

        private void OnStreakReached(BusMessage message)
        {
            if (message.TryGet(out PlayerStreak streak) && streak.IsMilestone)
                Add(Config.PointsPerStreak);
        }

        public void Reset()
        {
            Points = 0;
            ChaosState.Reset();
        }

        private void Add(int amount)
        {
            if (amount <= 0 || !Round.IsRoundInProgress)
                return;

            if (Config.PauseDuringGamemodes && WorldEvents.IsPaused)
                return;

            Points += amount;

            int level = Math.Min(Points / Math.Max(1, Config.PointsPerLevel), Math.Max(0, Config.MaxBonusLevel));

            if (level == ChaosState.BonusLevel)
                return;

            ChaosState.SetBonusLevel(level);
            PluginBus.Publish(BusTopics.ChaosLevelChanged, plugin.Name, level);

            if (Config.AnnounceLevels)
                Server.SendBroadcast(plugin.Translation.LevelUp.Replace("%LEVEL%", level.ToString()), Config.BroadcastDuration);

            if (level >= Config.MaxBonusLevel)
                StartApogee();
        }

        // Le niveau ne redescend pas pendant un round : ce point n'est atteint qu'une fois.
        private void StartApogee()
        {
            bool incident = Request(BusTopics.IncidentRequested, Config.ApogeeIncidents);
            bool drop = Request(BusTopics.DropRequested, Config.ApogeeDrops);

            if (incident || drop)
                Server.SendBroadcast(plugin.Translation.Apogee, Config.BroadcastDuration);
        }

        private bool Request(string topic, List<string> choices)
        {
            return choices?.Count > 0 && PluginBus.Request(topic, plugin.Name, Chance.Pick(choices));
        }
    }
}
