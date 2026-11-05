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

        [Description("Annonce C.A.S.S.I.E de la fuite de gaz. Vide = aucune annonce.")]
        public string GasLeakCassie { get; set; } = "attention . danger detected";

        [Description("Sous-titres de la fuite de gaz. %ZONE% est remplace par le nom de la zone.")]
        public string GasLeakSubtitles { get; set; } = "Attention. Fuite de gaz en %ZONE%. Quittez les salles eclairees en vert.";

        [Description("Annonce C.A.S.S.I.E de la surcharge. Vide = aucune annonce.")]
        public string TeslaSurgeCassie { get; set; } = "attention . security system failure detected";

        [Description("Sous-titres de la surcharge. %ZONE% est remplace par le nom de la zone.")]
        public string TeslaSurgeSubtitles { get; set; } = "Attention. Surcharge des portes Tesla en %ZONE%.";

        [Description("Annonce C.A.S.S.I.E de la coupure de cameras. Vide = aucune annonce.")]
        public string CameraOutageCassie { get; set; } = "attention . system failure detected";

        [Description("Sous-titres de la coupure de cameras. %ZONE% est remplace par le nom de la zone.")]
        public string CameraOutageSubtitles { get; set; } = "Attention. Reseau de cameras hors service.";

        [Description("Nom affiche de la zone de confinement legere.")]
        public string LightContainmentName { get; set; } = "zone de confinement legere";

        [Description("Nom affiche de la zone de confinement lourde.")]
        public string HeavyContainmentName { get; set; } = "zone de confinement lourde";

        [Description("Nom affiche de la zone d'entree.")]
        public string EntranceName { get; set; } = "zone d'entree";
    }
}
