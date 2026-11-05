using System.Collections.Generic;
using AugatonLib.Random;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public sealed class ChaosPill : PillBase
    {
        private readonly List<ChaosOutcome> good = new List<ChaosOutcome>(16);
        private readonly List<ChaosOutcome> bad = new List<ChaosOutcome>(24);
        private readonly List<ChaosOutcome> neutral = new List<ChaosOutcome>(24);

        public ChaosPill(PillDefinition definition, Color glowColor, IEnumerable<ChaosOutcome> outcomes)
            : base(definition, glowColor)
        {
            if (outcomes is null)
                return;

            foreach (ChaosOutcome outcome in outcomes)
            {
                if (outcome is null || outcome.Weight <= 0)
                    continue;

                Bucket(outcome.Category).Add(outcome);
            }
        }

        public int OutcomeCount => good.Count + bad.Count + neutral.Count;

        public override void Consume(Player player)
        {
            ChaosOutcome outcome = Draw();

            if (outcome is null)
                return;

            PayloadExecutor.Apply(player, outcome, $"{Key}:{outcome.Key}");

            Logger.Debug($"{player.Nickname} a tire l'issue {outcome.Category}/{outcome.Key}.", Config.Debug);
        }

        private ChaosOutcome Draw()
        {
            List<ChaosOutcome> bucket = RollBucket();

            return bucket is null ? null : Chance.PickWeighted(bucket, o => o.Weight);
        }

        private List<ChaosOutcome> RollBucket()
        {
            float goodShare = good.Count == 0 ? 0f : Mathf.Max(0f, Config.ChaosGoodWeight);
            float badShare = bad.Count == 0 ? 0f : Mathf.Max(0f, Config.ChaosBadWeight);
            float neutralShare = neutral.Count == 0 ? 0f : Mathf.Max(0f, Config.ChaosNeutralWeight);
            float total = goodShare + badShare + neutralShare;

            if (total <= 0f)
                return FirstNonEmpty();

            float roll = UnityEngine.Random.Range(0f, total);

            if (roll < goodShare)
                return good;

            return roll < goodShare + badShare ? bad : neutral;
        }

        private List<ChaosOutcome> Bucket(ChaosCategory category)
        {
            switch (category)
            {
                case ChaosCategory.Good: return good;
                case ChaosCategory.Bad: return bad;
                default: return neutral;
            }
        }

        private List<ChaosOutcome> FirstNonEmpty()
        {
            if (good.Count > 0)
                return good;

            if (bad.Count > 0)
                return bad;

            return neutral.Count > 0 ? neutral : null;
        }
    }
}
