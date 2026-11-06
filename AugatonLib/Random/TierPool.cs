using System;
using System.Collections.Generic;

namespace AugatonLib.Random
{
    public static class TierPool
    {
        // Les entrees du rang demande, ou celles du rang inferieur le plus proche si ce rang n'en a aucune.
        public static List<T> Closest<T>(IEnumerable<T> entries, Func<T, int> tierOf, int tier)
        {
            List<T> pool = new List<T>();
            int best = int.MinValue;

            foreach (T entry in entries)
            {
                int candidate = tierOf(entry);

                if (candidate > tier || candidate < best)
                    continue;

                if (candidate > best)
                {
                    pool.Clear();
                    best = candidate;
                }

                pool.Add(entry);
            }

            return pool;
        }
    }
}
