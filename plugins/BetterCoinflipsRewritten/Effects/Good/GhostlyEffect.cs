using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class GhostlyEffect : ICoinEffect
    {
        private readonly float duration;

        public GhostlyEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "Ghostly"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .GhostlyEffectMessage
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
            player.EnableEffect<Ghostly>(1, duration);
            player.EnableEffect<Invisible>(1, duration);
            player.EnableEffect<SilentWalk>(255, duration);
        }
    }
}
