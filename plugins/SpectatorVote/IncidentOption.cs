using System.ComponentModel;

namespace SpectatorVote
{
    public sealed class IncidentOption
    {
        [Description("Incident de SiteIncidents : Blackout, Alert, Lockdown, GasLeak, TeslaSurge ou CameraOutage.")]
        public string Incident { get; set; } = string.Empty;

        [Description("Nom affiche aux joueurs.")]
        public string Label { get; set; } = string.Empty;

        [Description("Poids dans le tirage des choix. 0 = jamais propose.")]
        public int Weight { get; set; } = 10;
    }
}
