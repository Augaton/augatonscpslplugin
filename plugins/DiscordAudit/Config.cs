using System.ComponentModel;

namespace DiscordAudit
{
    public sealed class Config
    {
        [Description("Adresse du webhook Discord. Vide = aucun envoi. C'est un secret : elle ne doit jamais etre committee.")]
        public string WebhookUrl { get; set; } = string.Empty;

        [Description("Secondes entre deux envois. Les actions sont regroupees, 2 au plus bas.")]
        public float SendIntervalSeconds { get; set; } = 5f;

        [Description("Actions gardees en attente. Au-dela, les plus anciennes sont perdues.")]
        public int MaxQueuedLines { get; set; } = 200;
    }
}
