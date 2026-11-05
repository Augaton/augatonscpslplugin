using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class SugarRushEffect : ICoinEffect
    {
        private readonly float duration;

        public SugarRushEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "SugarRush"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .SugarRushEffectMessage
                    .Replace(
                        "{duration}",
                        duration.ToString("0.#"));
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   duration > 0f;
        }

        public void Execute(Player player)
        {
            player.EnableEffect<MovementBoost>(50, duration);
            player.EnableEffect<Invigorated>(1, duration);
            player.EnableEffect<Vitality>(1, duration);
            player.StaminaRemaining = 1f;
        }
    }
}
