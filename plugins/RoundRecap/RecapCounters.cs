using System.Collections.Generic;
using System.Text;
using AugatonLib.Bus;
using LabApi.Features.Wrappers;

namespace RoundRecap
{
    public sealed class RecapCounters
    {
        private int pills;
        private int coins;
        private int drops;
        private int replacements;
        private int incidents;
        private int wipes;
        private int vendings;
        private int exchanges;
        private string gamemode;

        public void Subscribe(string owner)
        {
            PluginBus.Subscribe(owner, BusTopics.PillConsumed, _ => pills++);
            PluginBus.Subscribe(owner, BusTopics.CoinFlipped, _ => coins++);
            PluginBus.Subscribe(owner, BusTopics.SupplyDropped, _ => drops++);
            PluginBus.Subscribe(owner, BusTopics.PlayerReplaced, _ => replacements++);
            PluginBus.Subscribe(owner, BusTopics.SiteIncident, _ => incidents++);
            PluginBus.Subscribe(owner, BusTopics.TeamWiped, _ => wipes++);
            PluginBus.Subscribe(owner, BusTopics.VendingUsed, _ => vendings++);
            PluginBus.Subscribe(owner, BusTopics.ItemExchanged, _ => exchanges++);
            PluginBus.Subscribe(owner, BusTopics.GamemodeStarted, OnGamemodeStarted);
        }

        public void Reset()
        {
            pills = 0;
            coins = 0;
            drops = 0;
            replacements = 0;
            incidents = 0;
            wipes = 0;
            vendings = 0;
            exchanges = 0;
            gamemode = null;
        }

        public string Describe(Translation text, List<string> heroes)
        {
            List<string> lines = new List<string>(8);

            if (!string.IsNullOrEmpty(gamemode) && !string.IsNullOrEmpty(text.Gamemode))
                lines.Add(text.Gamemode.Replace("%MODE%", gamemode));

            Add(lines, text.Deaths, Round.TotalDeaths);
            Add(lines, text.Escapes, Round.EscapedClassD + Round.EscapedScientists);
            Add(lines, text.Pills, pills);
            Add(lines, text.Coins, coins);
            Add(lines, text.Drops, drops);
            Add(lines, text.Replacements, replacements);
            Add(lines, text.Incidents, incidents);
            Add(lines, text.Vendings, vendings);
            Add(lines, text.Exchanges, exchanges);
            Add(lines, text.Wipes, wipes);

            if (lines.Count == 0 && heroes.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder(text.Header);

            if (lines.Count > 0)
                builder.Append('\n').Append(string.Join("  |  ", lines));

            if (heroes.Count > 0)
                builder.Append('\n').Append(string.Join("  |  ", heroes));

            return builder.ToString();
        }

        private void OnGamemodeStarted(BusMessage message)
        {
            if (message.TryGet(out string name))
                gamemode = name;
        }

        private static void Add(List<string> lines, string template, int count)
        {
            if (count > 0 && !string.IsNullOrEmpty(template))
                lines.Add(template.Replace("%COUNT%", count.ToString()));
        }
    }
}
