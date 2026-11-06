using System;
using System.Collections.Generic;
using System.Text;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// Verifications faites sur le vrai serveur : prefabs, salles, dependances, tout ce que la
    /// compilation et les tests ne voient pas. Une verification renvoie null si tout va bien,
    /// sinon le probleme en quelques mots.
    /// </summary>
    public static class SelfCheck
    {
        private sealed class Entry
        {
            public Entry(string owner, string label, Func<string> check)
            {
                Owner = owner;
                Label = label;
                Check = check;
            }

            public string Owner { get; }

            public string Label { get; }

            public Func<string> Check { get; }
        }

        private static readonly List<Entry> Entries = new List<Entry>(16);

        public static void Register(string owner, string label, Func<string> check)
        {
            if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(label) && check is not null)
                Entries.Add(new Entry(owner, label, check));
        }

        public static void UnregisterOwner(string owner)
        {
            Entries.RemoveAll(entry => string.Equals(entry.Owner, owner, StringComparison.Ordinal));
        }

        public static string Report()
        {
            if (Entries.Count == 0)
                return "Aucune verification enregistree.";

            StringBuilder builder = new StringBuilder(512);
            int failures = 0;

            foreach (Entry entry in Entries)
            {
                string problem = Run(entry);

                if (problem is null)
                {
                    builder.AppendLine($"[OK] {entry.Owner} : {entry.Label}");
                    continue;
                }

                failures++;
                builder.AppendLine($"[ECHEC] {entry.Owner} : {entry.Label} ({problem})");
            }

            builder.Append($"{Entries.Count} verification(s), {failures} echec(s).");
            return builder.ToString();
        }

        public static List<string> Failures()
        {
            List<string> failures = new List<string>(4);

            foreach (Entry entry in Entries)
            {
                string problem = Run(entry);

                if (problem is not null)
                    failures.Add($"{entry.Owner} : {entry.Label} ({problem})");
            }

            return failures;
        }

        private static string Run(Entry entry)
        {
            try
            {
                return entry.Check();
            }
            catch (Exception e)
            {
                return $"{e.GetType().Name} : {e.Message}";
            }
        }
    }
}
