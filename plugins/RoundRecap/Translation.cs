using System.ComponentModel;

namespace RoundRecap
{
    public sealed class Translation
    {
        [Description("Titre du bilan de fin de round.")]
        public string Header { get; set; } = "<size=30><color=yellow>Bilan du round</color></size>";

        [Description("Ligne du mode de jeu du round. %MODE% est son nom.")]
        public string Gamemode { get; set; } = "Mode : %MODE%";

        [Description("Ligne du mutateur de round. %MODE% est son nom.")]
        public string Mutator { get; set; } = "Mutateur : %MODE%";

        [Description("Vainqueur du mode de jeu. %PLAYER% est son pseudo. Vide = masque.")]
        public string GamemodeWinner { get; set; } = "Vainqueur du mode : %PLAYER%";

        [Description("SCP le plus dangereux. %PLAYER% est son pseudo, %LEVEL% son niveau. Vide = masque.")]
        public string TopScp { get; set; } = "SCP le plus dangereux : %PLAYER% (niveau %LEVEL%)";

        [Description("Premier kill du round. %PLAYER% est le tueur. Vide = masque.")]
        public string FirstBlood { get; set; } = "Premier sang : %PLAYER%";

        [Description("Joueur mort le plus souvent, au moins deux fois. %PLAYER% est son pseudo, %COUNT% son total. Vide = masque.")]
        public string MostDeaths { get; set; } = "Mort le plus souvent : %PLAYER% (%COUNT%)";

        [Description("Meilleur tueur. %PLAYER% est son pseudo, %COUNT% son total. Vide = masque.")]
        public string BestKiller { get; set; } = "Meilleur tueur : %PLAYER% (%COUNT%)";

        [Description("Meilleure serie de kills, avec Streaks. %PLAYER% est le pseudo, %COUNT% la serie. Vide = masque.")]
        public string BestStreak { get; set; } = "Meilleure serie : %PLAYER% (%COUNT%)";

        [Description("Premiere evasion. %PLAYER% est le pseudo, %TIME% le moment du round. Vide = masque.")]
        public string FirstEscape { get; set; } = "Premiere evasion : %PLAYER% (%TIME%)";

        [Description("Humain en vie depuis le plus longtemps a la fin du round. %PLAYER% est son pseudo. Vide = masque.")]
        public string Survivor { get; set; } = "Survivant le plus tenace : %PLAYER%";

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

        [Description("Ligne des utilisations de SCP-261.")]
        public string Vendings { get; set; } = "%COUNT% passage(s) a SCP-261";

        [Description("Ligne des echanges a SCP-1162.")]
        public string Exchanges { get; set; } = "%COUNT% echange(s) a SCP-1162";

        [Description("Ligne des contrats remplis.")]
        public string Contracts { get; set; } = "%COUNT% contrat(s) rempli(s)";

        [Description("Ligne des equipes eliminees.")]
        public string Wipes { get; set; } = "%COUNT% equipe(s) eliminee(s)";
    }
}
