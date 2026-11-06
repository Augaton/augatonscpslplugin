using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;
using MapGeneration;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class FalseAlarmEffect : ICoinEffect
    {
        private const string CooldownKey = "false_alarm";

        public string Name
        {
            get { return "FalseAlarm"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.FalseAlarmEffectMessage; }
        }

        public static string CassieLine(FacilityZone zone)
        {
            switch (zone)
            {
                case FacilityZone.LightContainment: return "attention . scp detected in light containment zone";
                case FacilityZone.HeavyContainment: return "attention . scp detected in heavy containment zone";
                case FacilityZone.Entrance: return "attention . scp detected in entrance zone";
                case FacilityZone.Surface: return "attention . scp detected on surface";
                default: return null;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   CassieLine(player.Zone) is not null &&
                   GlobalCooldown.IsReady(
                       CooldownKey,
                       Plugin.Instance.Config.FacilityEffectCooldown);
        }

        public void Execute(Player player)
        {
            if (!GlobalCooldown.TryConsume(CooldownKey, Plugin.Instance.Config.FacilityEffectCooldown))
                return;

            Announcer.Message(CassieLine(player.Zone), Plugin.Instance.Translation.FalseAlarmSubtitles, false, 0f, 0f);
        }
    }
}
