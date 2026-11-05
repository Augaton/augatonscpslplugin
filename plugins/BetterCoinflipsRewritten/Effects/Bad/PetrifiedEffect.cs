using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class PetrifiedEffect : ICoinEffect
    {
        private readonly float duration;

        public PetrifiedEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "Petrified"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .PetrifiedEffectMessage
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
                   !player.IsSCP &&
                   duration > 0f;
        }

        public void Execute(Player player)
        {
            player.EnableEffect<Ensnared>(1, duration);
            player.EnableEffect<Deafened>(1, duration);
        }
    }
}
