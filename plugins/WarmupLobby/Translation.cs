using System.ComponentModel;

namespace WarmupLobby
{
    public sealed class Translation
    {
        [Description("Hint affiche a chaque apparition pendant l'echauffement.")]
        public string Spawned { get; set; } = "Echauffement : battez-vous en attendant le round !";

        [Description("Annonce du champion du lobby. %PLAYER% est son pseudo, %COUNT% ses kills.")]
        public string Champion { get; set; } = "<size=30><color=yellow>Champion du lobby : %PLAYER% (%COUNT% kills)</color></size>";
    }
}
