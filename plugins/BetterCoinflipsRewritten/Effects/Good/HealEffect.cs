using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class HealEffect : ICoinEffect
    {
        private readonly float amount;

        public HealEffect(float amount)
        {
            this.amount = amount;
        }

        public string Name
        {
            get { return "Heal"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .HealEffectMessage
                    .Replace(
                        "{amount}",
                        amount.ToString("0.#"));
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   player.Health < player.MaxHealth;
        }

        public void Execute(Player player)
        {
            float newHealth = player.Health + amount;

            if (newHealth > player.MaxHealth)
                newHealth = player.MaxHealth;

            player.Health = newHealth;
        }
    }
}
