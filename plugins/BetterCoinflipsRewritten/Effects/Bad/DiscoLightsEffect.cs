using AugatonLib.Arbitration;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class DiscoLightsEffect : ICoinEffect
    {
        private const string CooldownKey = "disco";

        private readonly float duration;
        private readonly float interval;

        public DiscoLightsEffect(float duration, float interval)
        {
            this.duration = duration;
            this.interval = interval;
        }

        public string Name
        {
            get { return "DiscoLights"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .DiscoLightsEffectMessage
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
                   interval > 0f &&
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

            int steps = Mathf.Max(1, Mathf.FloorToInt(duration / interval));

            for (int i = 0; i < steps; i++)
            {
                Scheduler.Delay(
                    interval * i,
                    delegate { LightArbiter.Tint(Interop.Owner, NextColor()); });
            }

            Scheduler.Delay(
                duration,
                delegate { LightArbiter.ReleaseTint(Interop.Owner); });
        }

        private static Color NextColor()
        {
            return new Color(
                Rng.NextUnit(),
                Rng.NextUnit(),
                Rng.NextUnit());
        }
    }
}
