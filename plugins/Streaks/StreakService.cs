using System;
using AugatonLib.Arcade;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using AugatonLib.Text;
using AugatonLib.World;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace Streaks
{
    public sealed class StreakService
    {
        private readonly Plugin plugin;

        public StreakService(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        private Translation Text => plugin.Translation;

        private KillLadder<StreakReward> Ladder => plugin.Ladder;

        public void OnDeath(PlayerDeathEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || !Round.IsRoundInProgress)
                    return;

                int ended = Ladder.Forget(ev.Player.UserId);
                Player killer = ev.Attacker;

                if (!IsCounted(killer, ev))
                    return;

                if (Config.BountyKills > 0 && ended >= Config.BountyKills)
                    ClaimBounty(killer, ev.Player);

                RegisterKill(killer);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeath: {e}");
            }
        }

        public void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                Ladder.Forget(ev?.Player?.UserId);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        public void Reset() => Ladder.Clear();

        public void RegisterKill(Player killer)
        {
            StreakReward reward = Ladder.Add(killer.UserId, out int kills);

            if (Config.BountyKills > 0 && kills == Config.BountyKills)
            {
                plugin.Hints.ShowAll(
                    Text.BountyPlaced.Replace("%PLAYER%", SafeText.Nickname(killer)).Replace("%COUNT%", kills.ToString()),
                    Config.HintDuration);
            }

            PluginBus.Publish(BusTopics.StreakReached, Plugin.Owner, new PlayerStreak(killer, kills, reward is not null));

            if (reward is null)
                return;

            ItemGrant.Give(killer, reward.Item, reward.Quantity);

            plugin.Hints.Show(
                killer,
                Text.RewardEarned.Replace("%COUNT%", kills.ToString()).Replace("%REWARD%", reward.Label),
                Config.HintDuration);
        }

        private bool IsCounted(Player killer, PlayerDeathEventArgs ev)
        {
            if (killer is null || killer == ev.Player || !killer.IsHuman || killer.IsTutorial)
                return false;

            if (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses)
                return false;

            return ev.OldRole != RoleTypeId.Tutorial && ev.OldRole.GetFaction() != killer.Faction;
        }

        private void ClaimBounty(Player killer, Player target)
        {
            ItemGrant.Give(killer, Config.BountyItem, Config.BountyQuantity);

            plugin.Hints.ShowAll(
                Text.BountyClaimed.Replace("%KILLER%", SafeText.Nickname(killer)).Replace("%PLAYER%", SafeText.Nickname(target)),
                Config.HintDuration);
        }
    }
}
