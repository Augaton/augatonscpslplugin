using System.ComponentModel;

namespace SpectatorVote
{
    public sealed class Translation
    {
        [Description("Hint d'ouverture, montre aux spectateurs. %SECONDS% = duree, %OPTIONS% = lignes de choix.")]
        public string VoteOpened { get; set; } = "Les morts votent pour la suite du round (%SECONDS% s) :\n%OPTIONS%";

        [Description("Ligne de choix. %NUMBER% = numero, %LABEL% = incident.")]
        public string OptionLine { get; set; } = ".vi %NUMBER% : %LABEL%";

        [Description("Hint du resultat, montre a tous. %LABEL% = choix gagnant.")]
        public string VoteResult { get; set; } = "Les morts ont choisi : %LABEL%";

        [Description("Reponse a un vote pris en compte. %LABEL% = incident choisi.")]
        public string VoteRegistered { get; set; } = "Vote enregistre : %LABEL%.";

        [Description("Reponse quand aucun vote n'est ouvert.")]
        public string VoteClosed { get; set; } = "Aucun vote en cours.";

        [Description("Reponse a un joueur qui n'est pas spectateur.")]
        public string SpectatorsOnly { get; set; } = "Seuls les spectateurs votent.";

        [Description("Reponse a un numero hors de la liste.")]
        public string VoteInvalid { get; set; } = "Choix invalide. Tapez .vi pour voir la liste.";

        [Description("Reponse pendant le delai entre deux votes.")]
        public string VoteTooFast { get; set; } = "Attendez un instant.";

        [Description("Reponse a une commande venue de la console du serveur.")]
        public string PlayerOnly { get; set; } = "Commande reservee aux joueurs.";
    }
}
