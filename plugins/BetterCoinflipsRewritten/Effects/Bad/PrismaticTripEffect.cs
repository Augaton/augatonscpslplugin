using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class PrismaticTripEffect : ICoinEffect
    {
        private readonly float duration;

        public PrismaticTripEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "PrismaticTrip"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .PrismaticTripEffectMessage
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
            player.EnableEffect<RainbowTaste>(5, duration);
            player.EnableEffect<Traumatized>(1, duration);
            player.EnableEffect<Blurred>(5, duration);
        }
    }
}
