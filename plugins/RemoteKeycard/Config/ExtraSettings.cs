using System.ComponentModel;

namespace RemoteKeycard.Config
{
    public sealed class ExtraSettings
    {
        [Description("Active le systeme d'evenements expose aux autres plugins. Laisser a false si aucun plugin n'utilise l'API de RemoteKeycard.")]
        public bool EnableEvents { get; set; } = false;
    }
}
