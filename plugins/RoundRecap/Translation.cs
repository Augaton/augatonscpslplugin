using System.ComponentModel;

namespace RoundRecap
{
    public sealed class Translation
    {
        [Description("Titre du bilan de fin de round.")]
        public string Header { get; set; } = "<size=30><color=yellow>Bilan du round</color></size>";

        [Description("Ligne des morts. Dans toutes ces lignes, %COUNT% est le total. Une ligne vide est masquee.")]
        public string Deaths { get; set; } = "%COUNT% mort(s)";

        [Description("Ligne des evasions.")]
        public string Escapes { get; set; } = "%COUNT% evasion(s)";

        [Description("Ligne des pilules consommees.")]
        public string Pills { get; set; } = "%COUNT% pilule(s) avalee(s)";

        [Description("Ligne des pieces lancees.")]
        public string Coins { get; set; } = "%COUNT% piece(s) lancee(s)";

        [Description("Ligne des largages.")]
        public string Drops { get; set; } = "%COUNT% largage(s)";

        [Description("Ligne des remplacements.")]
        public string Replacements { get; set; } = "%COUNT% joueur(s) remplace(s)";

        [Description("Ligne des incidents de site.")]
        public string Incidents { get; set; } = "%COUNT% incident(s) de site";

        [Description("Ligne des equipes eliminees.")]
        public string Wipes { get; set; } = "%COUNT% equipe(s) eliminee(s)";
    }
}
