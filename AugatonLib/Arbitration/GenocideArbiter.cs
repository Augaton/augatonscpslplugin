using System;
using System.Collections.Generic;
using PlayerRoles;
using UnityEngine;

namespace AugatonLib.Arbitration
{
    public static class GenocideArbiter
    {
        private sealed class TeamHold
        {
            public TeamHold(string owner, Team team, float expiry)
            {
                Owner = owner;
                Team = team;
                Expiry = expiry;
            }

            public string Owner { get; }

            public Team Team { get; }

            public float Expiry { get; }
        }

        private const float Permanent = float.MaxValue;

        private static readonly Dictionary<string, float> Suppressors =
            new Dictionary<string, float>(StringComparer.Ordinal);

        private static readonly List<TeamHold> Holds = new List<TeamHold>(4);

        public static bool IsSuppressed
        {
            get
            {
                Purge();
                return Suppressors.Count > 0;
            }
        }

        public static int Count
        {
            get
            {
                Purge();
                return Suppressors.Count;
            }
        }

        public static void Suppress(string owner) => Suppress(owner, 0f);

        public static void Suppress(string owner, float seconds)
        {
            if (string.IsNullOrEmpty(owner))
                return;

            Suppressors[owner] = seconds <= 0f
                ? Permanent
                : Time.realtimeSinceStartup + seconds;
        }

        public static void Allow(string owner)
        {
            if (!string.IsNullOrEmpty(owner))
                Suppressors.Remove(owner);
        }

        /// <summary>
        /// Retient l'annonce d'une seule equipe le temps qu'un remplacant arrive. Celui qui retient
        /// publie ensuite BusTopics.TeamCheckRequested, pour que l'extinction soit annoncee si
        /// personne n'est venu.
        /// </summary>
        public static void Hold(string owner, Team team, float seconds)
        {
            if (string.IsNullOrEmpty(owner) || seconds <= 0f)
                return;

            Release(owner, team);
            Holds.Add(new TeamHold(owner, team, Time.realtimeSinceStartup + seconds));
        }

        public static void Release(string owner, Team team)
        {
            Holds.RemoveAll(hold => hold.Team == team && string.Equals(hold.Owner, owner, StringComparison.Ordinal));
        }

        public static bool IsHeld(Team team)
        {
            float now = Time.realtimeSinceStartup;

            Holds.RemoveAll(hold => hold.Expiry <= now);

            foreach (TeamHold hold in Holds)
            {
                if (hold.Team == team)
                    return true;
            }

            return false;
        }

        public static void Clear()
        {
            Suppressors.Clear();
            Holds.Clear();
        }

        public static string Describe()
        {
            Purge();

            if (Suppressors.Count == 0)
                return Holds.Count == 0 ? "libre" : $"{Holds.Count} equipe(s) retenue(s)";

            string[] owners = new string[Suppressors.Count];
            Suppressors.Keys.CopyTo(owners, 0);

            return $"supprime par {string.Join(", ", owners)}, {Holds.Count} equipe(s) retenue(s)";
        }

        private static void Purge()
        {
            if (Suppressors.Count == 0)
                return;

            float now = Time.realtimeSinceStartup;
            List<string> expired = null;

            foreach (KeyValuePair<string, float> entry in Suppressors)
            {
                if (entry.Value > now)
                    continue;

                expired ??= new List<string>(2);
                expired.Add(entry.Key);
            }

            if (expired is null)
                return;

            foreach (string owner in expired)
                Suppressors.Remove(owner);
        }
    }
}
