using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class DrunkEffect : ICoinEffect
    {
        private readonly float duration;

        public DrunkEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "Drunk"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .DrunkEffectMessage
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
            player.EnableEffect<Concussed>(1, duration);
            player.EnableEffect<Blurred>(8, duration);
            player.EnableEffect<Sinkhole>(1, duration);
            player.EnableEffect<Exhausted>(1, duration);
        }
    }
}
