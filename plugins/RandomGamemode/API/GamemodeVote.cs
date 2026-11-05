using System.Collections.Generic;
using System.Text;
using AugatonLib.Random;
using AugatonLib.Voting;

namespace RandomGamemode.API
{
    public sealed class GamemodeVote
    {
        private readonly BallotBox box = new BallotBox();
        private readonly List<GamemodeBase> options = new List<GamemodeBase>(4);

        public bool IsOpen => box.IsOpen;

        public IReadOnlyList<GamemodeBase> Options => options;

        public void Open(List<GamemodeBase> candidates)
        {
            options.Clear();
            options.AddRange(candidates);
            box.Open(options.Count);
        }

        public void Cancel()
        {
            options.Clear();
            box.Close();
        }

        public bool Cast(string userId, int option) => box.Cast(userId, option);

        public void Retract(string userId) => box.Retract(userId);

        public GamemodeBase Winner(System.Func<GamemodeBase, bool> canStart)
        {
            List<int> leaders = box.Leaders(option => canStart(options[option]));

            if (leaders.Count > 0)
                return options[Chance.Pick(leaders)];

            List<GamemodeBase> eligible = options.FindAll(option => canStart(option));

            return Chance.PickWeighted(eligible, option => option.Settings.Weight);
        }

        public string Describe()
        {
            StringBuilder builder = new StringBuilder(128);

            for (int i = 0; i < options.Count; i++)
            {
                builder.Append(i + 1).Append(". ").Append(options[i].DisplayName)
                    .Append(" (").Append(box.CountFor(i)).Append(")\n");
            }

            return builder.ToString().TrimEnd('\n');
        }
    }
}
