using System;
using System.Collections.Generic;
using AugatonLib.Arcade;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using AugatonLib.Text;
using AugatonLib.World;
using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace Streaks
{
    public sealed class StreakService
    {
        private readonly Plugin plugin;
        private readonly Dictionary<string, int> retained = new Dictionary<string, int>();

        private string headStart;

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
                bool counted = IsCounted(killer, ev);

                if (counted && Config.BountyKills > 0 && ended >= Config.BountyKills)
                    ClaimBounty(killer, ev.Player, ended);
                else if (Config.AnnounceEndKills > 0 && ended >= Config.AnnounceEndKills && !retained.ContainsKey(ev.Player.UserId))
                    AnnounceEnd(ev.Player, ended);

                if (counted)
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

        // Le joueur qui part est tue juste apres : sa serie se lit maintenant ou plus jamais.
        public void OnDeparting(Player leaver)
        {
            try
            {
                int kills = Ladder.KillsOf(leaver?.UserId);

                if (kills > 0)
                    retained[leaver.UserId] = kills;
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeparting: {e}");
            }
        }

        public void OnPlayerReplaced(BusMessage message)
        {
            if (!message.TryGet(out PlayerReplacement replacement) || replacement.ReplacedUserId is null)
                return;

            if (retained.TryGetValue(replacement.ReplacedUserId, out int kills))
                Ladder.SetKills(replacement.Replacement?.UserId, kills);

            retained.Remove(replacement.ReplacedUserId);
        }

        public void OnGamemodeWon(BusMessage message)
        {
            if (message.TryGet(out Player player))
                headStart = player.UserId;
        }

        // Au debut du round et pas a la fin du precedent : la deconnexion du redemarrage efface la serie.
        public void GrantHeadStart()
        {
            Ladder.SetKills(headStart, Config.KillsForGamemodeWin);
            headStart = null;
        }

        public void Reset()
        {
            Ladder.Clear();
            retained.Clear();
        }

        public string CheckItems()
        {
            if (!InventoryItemLoader.AvailableItems.ContainsKey(Config.BountyItem))
                return $"objet inconnu : {Config.BountyItem}";

            foreach (StreakReward reward in Ladder.Steps)
            {
                if (!InventoryItemLoader.AvailableItems.ContainsKey(reward.Item))
                    return $"objet inconnu : {reward.Item}";
            }

            return null;
        }

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

        private void AnnounceEnd(Player player, int kills)
        {
            plugin.Hints.ShowAll(
                Text.StreakEnded.Replace("%PLAYER%", SafeText.Nickname(player)).Replace("%COUNT%", kills.ToString()),
                Config.HintDuration);
        }

        private int BountyQuantityFor(int kills)
        {
            int bonus = Config.BountyStepKills > 0 ? (kills - Config.BountyKills) / Config.BountyStepKills : 0;

            return Config.BountyQuantity + Math.Min(bonus, Config.BountyMaxBonus);
        }

        private void ClaimBounty(Player killer, Player target, int kills)
        {
            ItemGrant.Give(killer, Config.BountyItem, BountyQuantityFor(kills));

            plugin.Hints.ShowAll(
                Text.BountyClaimed.Replace("%KILLER%", SafeText.Nickname(killer)).Replace("%PLAYER%", SafeText.Nickname(target)),
                Config.HintDuration);
        }
    }
}
