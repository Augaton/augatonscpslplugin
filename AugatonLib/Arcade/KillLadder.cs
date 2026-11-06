using System;
using System.Collections.Generic;

namespace AugatonLib.Arcade
{
    /// <summary>
    /// Compte les kills de chaque joueur et dit quand il atteint un palier. Les paliers sont tries, et un
    /// seuil nul, negatif ou deja pris est ecarte : deux paliers ne peuvent pas tomber sur le meme kill.
    /// </summary>
    public sealed class KillLadder<TStep>
        where TStep : class
    {
        private readonly Dictionary<string, int> kills = new Dictionary<string, int>(StringComparer.Ordinal);
        private readonly List<TStep> steps = new List<TStep>(8);
        private readonly Func<TStep, int> thresholdOf;

        public KillLadder(Func<TStep, int> thresholdOf) => this.thresholdOf = thresholdOf;

        public IReadOnlyList<TStep> Steps => steps;

        public void SetSteps(IEnumerable<TStep> values)
        {
            steps.Clear();

            if (values is null)
                return;

            HashSet<int> taken = new HashSet<int>();

            foreach (TStep value in values)
            {
                if (value is null)
                    continue;

                int threshold = thresholdOf(value);

                if (threshold > 0 && taken.Add(threshold))
                    steps.Add(value);
            }

            steps.Sort((left, right) => thresholdOf(left).CompareTo(thresholdOf(right)));
        }

        public int KillsOf(string player)
        {
            return !string.IsNullOrEmpty(player) && kills.TryGetValue(player, out int count) ? count : 0;
        }

        public int LevelOf(string player)
        {
            int count = KillsOf(player);
            int level = 0;

            foreach (TStep step in steps)
            {
                if (thresholdOf(step) > count)
                    break;

                level++;
            }

            return level;
        }

        public TStep CurrentStep(string player)
        {
            int level = LevelOf(player);

            return level == 0 ? null : steps[level - 1];
        }

        public TStep Add(string player, out int count)
        {
            count = 0;

            if (string.IsNullOrEmpty(player))
                return null;

            count = KillsOf(player) + 1;
            kills[player] = count;

            foreach (TStep step in steps)
            {
                if (thresholdOf(step) == count)
                    return step;
            }

            return null;
        }

        public int Forget(string player)
        {
            int count = KillsOf(player);

            if (count > 0)
                kills.Remove(player);

            return count;
        }

        public void Clear() => kills.Clear();
    }
}
