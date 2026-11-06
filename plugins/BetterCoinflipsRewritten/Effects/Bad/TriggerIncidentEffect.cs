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

        private bool accepted;

        public string Name
        {
            get { return "TriggerIncident"; }
        }

        public string Message
        {
            get
            {
                Translation text = Plugin.Instance.Translation;

                return accepted ? text.TriggerIncidentEffectMessage : text.RequestRefusedEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            Config config = Plugin.Instance.Config;

            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   config.CoinIncidents?.Count > 0 &&
                   !Warhead.IsDetonated &&
                   !(config.PauseDuringGamemodes && GamemodeState.BlocksBonuses) &&
                   PluginDirectory.IsRegistered(TargetPlugin) &&
                   GlobalCooldown.IsReady(CooldownKey, config.FacilityEffectCooldown);
        }

        // Le cooldown ne part qu'une fois la demande acceptee : un refus ne bloque pas le lancer suivant.
        public void Execute(Player player)
        {
            Config config = Plugin.Instance.Config;

            accepted = PluginBus.Request(BusTopics.IncidentRequested, Interop.Owner, Chance.Pick(config.CoinIncidents));

            if (accepted)
                GlobalCooldown.TryConsume(CooldownKey, config.FacilityEffectCooldown);
        }
    }
}
