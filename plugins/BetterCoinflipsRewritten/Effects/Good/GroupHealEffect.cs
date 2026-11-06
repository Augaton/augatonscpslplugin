using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class GroupHealEffect : ICoinEffect
    {
        private readonly float amount;
        private readonly float range;

        private int healed;

        public GroupHealEffect(float amount, float range)
        {
            this.amount = amount;
            this.range = range;
        }

        public string Name
        {
            get { return "GroupHeal"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .GroupHealEffectMessage
                    .Replace("{count}", healed.ToString());
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive;
        }

        public void Execute(Player player)
        {
            float rangeSquared = range * range;

            healed = 0;

            foreach (Player ally in Player.ReadyList)
            {
                if (!ally.IsAlive || ally.Faction != player.Faction)
                    continue;

                if ((ally.Position - player.Position).sqrMagnitude > rangeSquared)
                    continue;

                ally.Heal(amount);
                healed++;
            }
        }
    }
}
