using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class HpReductionEffect : ICoinEffect
    {
        private readonly float percentage;
        private float removedHealth;

        public HpReductionEffect(float percentage)
        {
            this.percentage = percentage;
        }

        public string Name
        {
            get { return "HpReduction"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .HpReductionEffectMessage
                    .Replace(
                        "{amount}",
                        removedHealth.ToString("0.#"));
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   player.Health > 1f &&
                   percentage > 0f;
        }

        public void Execute(Player player)
        {
            removedHealth =
                player.Health * (percentage / 100f);

            float newHealth =
                player.Health - removedHealth;

            if (newHealth < 1f)
                newHealth = 1f;

            removedHealth =
                player.Health - newHealth;

            player.Health = newHealth;
        }
    }
}
