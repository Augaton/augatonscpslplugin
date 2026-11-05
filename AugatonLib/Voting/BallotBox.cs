using System;
using System.Collections.Generic;

namespace AugatonLib.Voting
{
    public sealed class BallotBox
    {
        private readonly Dictionary<string, int> ballots = new Dictionary<string, int>(StringComparer.Ordinal);

        public int OptionCount { get; private set; }

        public bool IsOpen => OptionCount > 0;

        public void Open(int optionCount)
        {
            OptionCount = Math.Max(0, optionCount);
            ballots.Clear();
        }

        public void Close() => Open(0);

        public bool Cast(string voter, int option)
        {
            if (!IsOpen || string.IsNullOrEmpty(voter) || option < 0 || option >= OptionCount)
                return false;

            ballots[voter] = option;
            return true;
        }

        public int CountFor(int option)
        {
            int count = 0;

            foreach (int ballot in ballots.Values)
            {
                if (ballot == option)
                    count++;
            }

            return count;
        }

        public List<int> Leaders(Func<int, bool> isEligible = null)
        {
            List<int> leaders = new List<int>(OptionCount);
            int best = 0;

            for (int option = 0; option < OptionCount; option++)
            {
                if (isEligible is not null && !isEligible(option))
                    continue;

                int count = CountFor(option);

                if (count == 0 || count < best)
                    continue;

                if (count > best)
                {
                    leaders.Clear();
                    best = count;
                }

                leaders.Add(option);
            }

            return leaders;
        }
    }
}
