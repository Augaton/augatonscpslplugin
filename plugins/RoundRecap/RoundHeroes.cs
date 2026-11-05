using System;
using System.Collections.Generic;
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

            public int Kills { get; set; }
        }

        private readonly Dictionary<string, Tally> kills = new Dictionary<string, Tally>();
        private readonly Dictionary<string, TimeSpan> aliveSince = new Dictionary<string, TimeSpan>();

        private string firstEscapee;
        private TimeSpan firstEscapeTime;

        public void Reset()
        {
            kills.Clear();
            aliveSince.Clear();
            firstEscapee = null;
        }

        public void OnDeath(PlayerDeathEventArgs ev)
        {
            try
            {
                Player killer = ev?.Attacker;

                if (killer is null || killer == ev.Player || string.IsNullOrEmpty(killer.UserId))
                    return;

                if (!kills.TryGetValue(killer.UserId, out Tally tally))
                {
                    tally = new Tally();
                    kills[killer.UserId] = tally;
                }

                tally.Name = SafeText.Nickname(killer);
                tally.Kills++;
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
            List<string> lines = new List<string>(3);

            Tally best = BestKiller();

            if (best is not null && !string.IsNullOrEmpty(text.BestKiller))
                lines.Add(text.BestKiller.Replace("%PLAYER%", best.Name).Replace("%COUNT%", best.Kills.ToString()));

            if (firstEscapee is not null && !string.IsNullOrEmpty(text.FirstEscape))
                lines.Add(text.FirstEscape.Replace("%PLAYER%", firstEscapee).Replace("%TIME%", firstEscapeTime.ToString(@"m\:ss")));

            string survivor = Survivor();

            if (survivor is not null && !string.IsNullOrEmpty(text.Survivor))
                lines.Add(text.Survivor.Replace("%PLAYER%", survivor));

            return lines;
        }

        private Tally BestKiller()
        {
            Tally best = null;

            foreach (Tally tally in kills.Values)
            {
                if (best is null || tally.Kills > best.Kills)
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
