using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Random;
using AugatonLib.Runtime;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class SurpriseDropEffect : ICoinEffect
    {
        private const string CooldownKey = "surprise_drop";
        private const string TargetPlugin = "SupplyDrop";

        public string Name
        {
            get { return "SurpriseDrop"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.SurpriseDropEffectMessage; }
        }

        public bool CanApply(Player player)
        {
            Config config = Plugin.Instance.Config;

            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   config.SurpriseDropProfiles?.Count > 0 &&
                   !(config.PauseDuringGamemodes && WorldEvents.IsPaused) &&
                   PluginDirectory.IsRegistered(TargetPlugin) &&
                   GlobalCooldown.IsReady(CooldownKey, config.FacilityEffectCooldown);
        }

        public void Execute(Player player)
        {
            Config config = Plugin.Instance.Config;

            if (!GlobalCooldown.TryConsume(CooldownKey, config.FacilityEffectCooldown))
                return;

            List<string> choices = config.SurpriseDropProfiles;

            PluginBus.Publish(BusTopics.DropRequested, Interop.Owner, Chance.Pick(choices));
        }
    }
}
