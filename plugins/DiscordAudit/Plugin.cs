using AugatonLib.Bus;
using AugatonLib.Runtime;
using AugatonLib.Text;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace DiscordAudit
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public WebhookSender Sender { get; private set; }

        public override string Name => "DiscordAudit";

        public override string Description => "Envoie les actions du staff vers un webhook Discord.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Sender = new WebhookSender(this);

            // Pas de demarrage ici : aucune tache reseau ne doit partir pendant Enable.
            ServerEvents.WaitingForPlayers += Sender.Start;
            PluginBus.Subscribe(Name, BusTopics.StaffAction, Sender.OnStaffAction);
            PluginBus.Subscribe(Name, BusTopics.StaffAlert, Sender.OnStaffAlert);
            PluginBus.Subscribe(Name, BusTopics.RoundSummaryReady, Sender.OnRoundSummary);

            SelfCheck.Register(Name, "adresse du webhook", CheckAddress);

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            ServerEvents.WaitingForPlayers -= Sender.Start;
            PluginBus.UnsubscribeOwner(Name);

            Sender.Stop();
            PluginDirectory.Unregister(this);

            Sender = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.SendIntervalSeconds = AtLeast("send_interval_seconds", Config.SendIntervalSeconds, 2f);
            Config.MaxQueuedLines = (int)AtLeast("max_queued_lines", Config.MaxQueuedLines, 1f);

            if (HasInvalidAddress())
                Logger.Warn("webhook_url n'est pas une adresse de webhook Discord en https : aucun envoi.");
        }

        private bool HasInvalidAddress()
        {
            return !string.IsNullOrEmpty(Config.WebhookUrl) && !DiscordMessage.IsWebhookUrl(Config.WebhookUrl);
        }

        private string CheckAddress() => HasInvalidAddress() ? "adresse invalide" : null;
    }
}
