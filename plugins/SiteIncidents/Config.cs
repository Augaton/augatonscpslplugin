using System.ComponentModel;

namespace SiteIncidents
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Delai en secondes avant le premier incident du round.")]
        public float FirstIncidentSeconds { get; set; } = 300f;

        [Description("Delai minimal en secondes entre deux incidents. Minimum 30.")]
        public float MinIntervalSeconds { get; set; } = 180f;

        [Description("Delai maximal en secondes entre deux incidents.")]
        public float MaxIntervalSeconds { get; set; } = 360f;

        [Description("Nombre de joueurs requis pour qu'un incident se produise.")]
        public int MinPlayers { get; set; } = 4;

        [Description("Suspend les incidents pendant un mode de jeu de type arene, comme le Gun Game.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Secondes retirees a l'intervalle par niveau du Directeur du chaos, si ce plugin est installe.")]
        public float IntervalPerChaosLevel { get; set; } = 20f;

        [Description("Poids de la panne de zone. 0 = jamais.")]
        public int BlackoutWeight { get; set; } = 10;

        [Description("Duree de la panne, en secondes.")]
        public float BlackoutDuration { get; set; } = 20f;

        [Description("Poids de l'alerte de zone. 0 = jamais.")]
        public int AlertWeight { get; set; } = 10;

        [Description("Duree de l'alerte, en secondes.")]
        public float AlertDuration { get; set; } = 30f;

        [Description("Couleur de l'alerte, au format hexadecimal.")]
        public string AlertColor { get; set; } = "#FF0000";

        [Description("Poids du verrouillage de portes. 0 = jamais.")]
        public int LockdownWeight { get; set; } = 10;

        [Description("Duree du verrouillage, en secondes.")]
        public float LockdownDuration { get; set; } = 15f;

        [Description("Nombre de portes verrouillees dans la zone.")]
        public int LockdownDoors { get; set; } = 10;

        [Description("Poids de la fuite de gaz dans une salle. 0 = jamais.")]
        public int GasLeakWeight { get; set; } = 6;

        [Description("Duree de la fuite de gaz, en secondes.")]
        public float GasLeakDuration { get; set; } = 20f;

        [Description("Couleur de la salle touchee par la fuite.")]
        public string GasLeakColor { get; set; } = "#33FF66";

        [Description("Poids de la surcharge des portes Tesla. 0 = jamais.")]
        public int TeslaSurgeWeight { get; set; } = 6;

        [Description("Duree de la surcharge, en secondes.")]
        public float TeslaSurgeDuration { get; set; } = 15f;

        [Description("Secondes entre deux decharges pendant la surcharge, 2 au plus bas.")]
        public float TeslaSurgeInterval { get; set; } = 3f;

        [Description("Poids de la coupure de cameras. 0 = jamais. Sans SCP-079 en jeu, elle n'est pas tiree.")]
        public int CameraOutageWeight { get; set; } = 6;

        [Description("Duree de la coupure de cameras, en secondes.")]
        public float CameraOutageDuration { get; set; } = 15f;
    }
}
