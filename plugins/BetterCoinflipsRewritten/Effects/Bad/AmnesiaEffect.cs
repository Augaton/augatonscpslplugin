using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class AmnesiaEffect : ICoinEffect
    {
        private readonly float duration;

        public AmnesiaEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "Amnesia"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .AmnesiaEffectMessage
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
            player.EnableEffect<AmnesiaVision>(1, duration);
            player.EnableEffect<AmnesiaItems>(1, duration);
            player.EnableEffect<Concussed>(1, duration);
        }
    }
}
