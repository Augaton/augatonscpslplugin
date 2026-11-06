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

        private bool accepted;

        public string Name
        {
            get { return "SurpriseDrop"; }
        }

        public string Message
        {
            get
            {
                Translation text = Plugin.Instance.Translation;

                return accepted ? text.SurpriseDropEffectMessage : text.RequestRefusedEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            Config config = Plugin.Instance.Config;

            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   config.SurpriseDropProfiles?.Count > 0 &&
                   !(config.PauseDuringGamemodes && GamemodeState.BlocksBonuses) &&
                   PluginDirectory.IsRegistered(TargetPlugin) &&
                   GlobalCooldown.IsReady(CooldownKey, config.FacilityEffectCooldown);
        }

        // Le cooldown ne part qu'une fois la demande acceptee : un refus ne bloque pas le lancer suivant.
        public void Execute(Player player)
        {
            Config config = Plugin.Instance.Config;

            accepted = PluginBus.Request(BusTopics.DropRequested, Interop.Owner, Chance.Pick(config.SurpriseDropProfiles));

            if (accepted)
                GlobalCooldown.TryConsume(CooldownKey, config.FacilityEffectCooldown);
        }
    }
}
