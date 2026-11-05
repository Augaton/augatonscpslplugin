using System;
using AugatonLib.Bus;
using AugatonLib.Chaos;
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

        public void Reset()
        {
            Points = 0;
            ChaosState.Reset();
        }

        private void Add(int amount)
        {
            if (amount <= 0 || !Round.IsRoundInProgress)
                return;

            Points += amount;

            int level = Math.Min(Points / Math.Max(1, Config.PointsPerLevel), Math.Max(0, Config.MaxBonusLevel));

            if (level == ChaosState.BonusLevel)
                return;

            ChaosState.SetBonusLevel(level);

            if (Config.AnnounceLevels)
                Server.SendBroadcast(plugin.Translation.LevelUp.Replace("%LEVEL%", level.ToString()), Config.BroadcastDuration);
        }
    }
}
