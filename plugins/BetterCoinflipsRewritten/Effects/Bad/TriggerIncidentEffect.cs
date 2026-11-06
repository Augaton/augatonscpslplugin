using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Random;
using AugatonLib.Runtime;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class TriggerIncidentEffect : ICoinEffect
    {
        private const string CooldownKey = "trigger_incident";
        private const string TargetPlugin = "SiteIncidents";

        public string Name
        {
            get { return "TriggerIncident"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.TriggerIncidentEffectMessage; }
        }

        public bool CanApply(Player player)
        {
            Config config = Plugin.Instance.Config;

            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   config.CoinIncidents?.Count > 0 &&
                   !(config.PauseDuringGamemodes && WorldEvents.IsPaused) &&
                   PluginDirectory.IsRegistered(TargetPlugin) &&
                   GlobalCooldown.IsReady(CooldownKey, config.FacilityEffectCooldown);
        }

        public void Execute(Player player)
        {
            Config config = Plugin.Instance.Config;

            if (!GlobalCooldown.TryConsume(CooldownKey, config.FacilityEffectCooldown))
                return;

            List<string> choices = config.CoinIncidents;

            PluginBus.Publish(BusTopics.IncidentRequested, Interop.Owner, Chance.Pick(choices));
        }
    }
}
