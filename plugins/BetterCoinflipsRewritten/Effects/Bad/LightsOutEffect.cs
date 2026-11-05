using AugatonLib.Arbitration;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;
using MapGeneration;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class LightsOutEffect : ICoinEffect
    {
        private const string CooldownKey = "lightsout";

        private readonly float duration;

        public LightsOutEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "LightsOut"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .LightsOutEffectMessage
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
                   duration > 0f &&
                   !IsAnyZoneBlackedOut() &&
                   GlobalCooldown.IsReady(
                       CooldownKey,
                       Plugin.Instance.Config.FacilityEffectCooldown);
        }

        public void Execute(Player player)
        {
            float cooldown =
                Plugin.Instance.Config.FacilityEffectCooldown;

            if (!GlobalCooldown.TryConsume(CooldownKey, cooldown))
                return;

            LightArbiter.Blackout(Interop.Owner, duration);
        }

        private static bool IsAnyZoneBlackedOut()
        {
            foreach (FacilityZone zone in LightArbiter.AllZones)
            {
                if (LightArbiter.IsBlackedOut(zone))
                    return true;
            }

            return false;
        }
    }
}
