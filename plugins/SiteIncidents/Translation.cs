using System.ComponentModel;

namespace SiteIncidents
{
    public sealed class Translation
    {
        [Description("Annonce C.A.S.S.I.E de la panne. Vide = aucune annonce.")]
        public string BlackoutCassie { get; set; } = "attention . light system failure detected";

        [Description("Sous-titres de la panne. %ZONE% est remplace par le nom de la zone.")]
        public string BlackoutSubtitles { get; set; } = "Attention. Panne d'eclairage detectee en %ZONE%.";

        [Description("Annonce C.A.S.S.I.E de l'alerte. Vide = aucune annonce.")]
        public string AlertCassie { get; set; } = "attention . security alert";

        [Description("Sous-titres de l'alerte. %ZONE% est remplace par le nom de la zone.")]
        public string AlertSubtitles { get; set; } = "Attention. Alerte de securite en %ZONE%.";

        [Description("Annonce C.A.S.S.I.E du verrouillage. Vide = aucune annonce.")]
        public string LockdownCassie { get; set; } = "attention . door system failure detected";

        [Description("Sous-titres du verrouillage. %ZONE% est remplace par le nom de la zone.")]
        public string LockdownSubtitles { get; set; } = "Attention. Portes bloquees en %ZONE%.";

        [Description("Nom affiche de la zone de confinement legere.")]
        public string LightContainmentName { get; set; } = "zone de confinement legere";

        [Description("Nom affiche de la zone de confinement lourde.")]
        public string HeavyContainmentName { get; set; } = "zone de confinement lourde";

        [Description("Nom affiche de la zone d'entree.")]
        public string EntranceName { get; set; } = "zone d'entree";
    }
}
