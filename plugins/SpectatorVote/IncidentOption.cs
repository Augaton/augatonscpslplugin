using System.ComponentModel;

namespace SpectatorVote
{
    public sealed class IncidentOption
    {
        [Description("Type de choix : Incident, SupplyDrop ou Mutator.")]
        public VoteOptionKind Kind { get; set; } = VoteOptionKind.Incident;

        [Description("Cible du choix. Incident : Blackout, Alert, Lockdown, GasLeak, TeslaSurge, CameraOutage ou ElevatorOutage. SupplyDrop : cle d'un profil de largage. Mutator : nom d'un mutateur.")]
        public string Incident { get; set; } = string.Empty;

        [Description("Nom affiche aux joueurs.")]
        public string Label { get; set; } = string.Empty;

        [Description("Poids dans le tirage des choix. 0 = jamais propose.")]
        public int Weight { get; set; } = 10;
    }
}
