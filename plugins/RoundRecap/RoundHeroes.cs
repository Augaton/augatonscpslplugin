using System;
using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Text;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace RoundRecap
{
    public sealed class RoundHeroes
    {
        private sealed class Tally
        {
            public string Name { get; set; }

            public int Total { get; set; }
        }

        private const int MinShownStreak = 2;
        private const int MinShownDeaths = 2;

        private readonly Dictionary<string, Tally> kills = new Dictionary<string, Tally>();
        private readonly Dictionary<string, Tally> deaths = new Dictionary<string, Tally>();
        private readonly Dictionary<string, TimeSpan> aliveSince = new Dictionary<string, TimeSpan>();

        private string firstEscapee;
        private string firstBlood;
        private string gamemodeWinner;
        private string topScp;
        private int topScpLevel;
        private TimeSpan firstEscapeTime;
        private string bestStreaker;
        private int bestStreak;

        public void Reset()
        {
            kills.Clear();
            deaths.Clear();
            firstBlood = null;
            aliveSince.Clear();
            firstEscapee = null;
            gamemodeWinner = null;
            topScp = null;
            topScpLevel = 0;
            bestStreaker = null;
            bestStreak = 0;
        }

        public void OnStreakReached(BusMessage message)
        {
            if (!message.TryGet(out PlayerStreak streak) || streak.Kills < MinShownStreak || streak.Kills <= bestStreak)
                return;

            bestStreak = streak.Kills;
            bestStreaker = SafeText.Nickname(streak.Player);
        }

        public void OnGamemodeWon(BusMessage message)
        {
            if (message.TryGet(out Player player))
                gamemodeWinner = SafeText.Nickname(player);
        }

        public void OnScpLevelReached(BusMessage message)
        {
            if (!message.TryGet(out ScpLevelUp levelUp) || levelUp.Level <= topScpLevel)
                return;

            topScpLevel = levelUp.Level;
            topScp = SafeText.Nickname(levelUp.Player);
        }

        public void OnDeath(PlayerDeathEventArgs ev)
        {
            try
            {
                Count(deaths, ev?.Player);

                Player killer = ev?.Attacker;

                if (killer is null || killer == ev.Player || string.IsNullOrEmpty(killer.UserId))
                    return;

                firstBlood ??= SafeText.Nickname(killer);
                Count(kills, killer);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeath: {e}");
            }
        }

        public void OnEscaped(PlayerEscapedEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || firstEscapee is not null)
                    return;

                firstEscapee = SafeText.Nickname(ev.Player);
                firstEscapeTime = Round.Duration;
            }
            catch (Exception e)
            {
                Logger.Error($"OnEscaped: {e}");
            }
        }

        public void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                string userId = ev?.Player?.UserId;

                if (string.IsNullOrEmpty(userId))
                    return;

                if (!ev.Player.IsHuman || ev.Player.IsTutorial)
                    aliveSince.Remove(userId);
                else if (!aliveSince.ContainsKey(userId))
                    aliveSince[userId] = Round.Duration;
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
                if (!string.IsNullOrEmpty(ev?.Player?.UserId))
                    aliveSince.Remove(ev.Player.UserId);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        public List<string> Describe(Translation text)
        {
            List<string> lines = new List<string>(4);

            if (firstBlood is not null && !string.IsNullOrEmpty(text.FirstBlood))
                lines.Add(text.FirstBlood.Replace("%PLAYER%", firstBlood));

            Tally best = Best(kills);

            if (best is not null && !string.IsNullOrEmpty(text.BestKiller))
                lines.Add(text.BestKiller.Replace("%PLAYER%", best.Name).Replace("%COUNT%", best.Total.ToString()));

            Tally unluckiest = Best(deaths);

            if (unluckiest?.Total >= MinShownDeaths && !string.IsNullOrEmpty(text.MostDeaths))
                lines.Add(text.MostDeaths.Replace("%PLAYER%", unluckiest.Name).Replace("%COUNT%", unluckiest.Total.ToString()));

            if (bestStreaker is not null && !string.IsNullOrEmpty(text.BestStreak))
                lines.Add(text.BestStreak.Replace("%PLAYER%", bestStreaker).Replace("%COUNT%", bestStreak.ToString()));

            if (firstEscapee is not null && !string.IsNullOrEmpty(text.FirstEscape))
                lines.Add(text.FirstEscape.Replace("%PLAYER%", firstEscapee).Replace("%TIME%", firstEscapeTime.ToString(@"m\:ss")));

            if (gamemodeWinner is not null && !string.IsNullOrEmpty(text.GamemodeWinner))
                lines.Add(text.GamemodeWinner.Replace("%PLAYER%", gamemodeWinner));

            if (topScp is not null && !string.IsNullOrEmpty(text.TopScp))
                lines.Add(text.TopScp.Replace("%PLAYER%", topScp).Replace("%LEVEL%", topScpLevel.ToString()));

            string survivor = Survivor();

            if (survivor is not null && !string.IsNullOrEmpty(text.Survivor))
                lines.Add(text.Survivor.Replace("%PLAYER%", survivor));

            return lines;
        }

        private static void Count(Dictionary<string, Tally> tallies, Player player)
        {
            if (string.IsNullOrEmpty(player?.UserId))
                return;

            if (!tallies.TryGetValue(player.UserId, out Tally tally))
            {
                tally = new Tally();
                tallies[player.UserId] = tally;
            }

            tally.Name = SafeText.Nickname(player);
            tally.Total++;
        }

        private static Tally Best(Dictionary<string, Tally> tallies)
        {
            Tally best = null;

            foreach (Tally tally in tallies.Values)
            {
                if (best is null || tally.Total > best.Total)
                    best = tally;
            }

            return best;
        }

        // Le plus ancien en vie au moment du bilan, donc celui qui a tenu le plus longtemps sans mourir.
        private string Survivor()
        {
            Player oldest = null;
            TimeSpan since = TimeSpan.MaxValue;

            foreach (KeyValuePair<string, TimeSpan> entry in aliveSince)
            {
                Player player = Player.Get(entry.Key);

                if (player is not null && !player.IsDestroyed && player.IsHuman && entry.Value < since)
                {
                    oldest = player;
                    since = entry.Value;
                }
            }

            return oldest is null ? null : SafeText.Nickname(oldest);
        }
    }
}
