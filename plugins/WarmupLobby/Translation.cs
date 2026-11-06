using System.ComponentModel;

namespace WarmupLobby
{
    public sealed class Translation
    {
        [Description("Hint affiche a chaque apparition pendant l'echauffement.")]
        public string Spawned { get; set; } = "Echauffement : battez-vous en attendant le round !";
    }
}
