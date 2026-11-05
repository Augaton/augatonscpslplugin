using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class MoreHealthEffect : ICoinEffect
    {
        private readonly float percentage;

        public MoreHealthEffect(float percentage)
        {
            this.percentage = percentage;
        }

        public string Name
        {
            get { return "MoreHealth"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .MoreHealthEffectMessage
                    .Replace(
                        "{percentage}",
                        percentage.ToString("0.#"));
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   player.MaxHealth > 0f;
        }

        public void Execute(Player player)
        {
            float increase =
                player.MaxHealth * (percentage / 100f);

            player.MaxHealth += increase;
            player.Health += increase;

            if (player.Health > player.MaxHealth)
                player.Health = player.MaxHealth;
        }
    }
}
