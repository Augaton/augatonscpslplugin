using System.ComponentModel;

namespace Contracts
{
    public sealed class Translation
    {
        [Description("Hint a l'attribution. %OBJECTIVE% est l'objectif, %REWARD% la recompense.")]
        public string Assigned { get; set; } = "<color=yellow>Contrat secret :</color> %OBJECTIVE% Recompense : %REWARD%.";

        [Description("Hint a la fin du contrat. %REWARD% est la recompense.")]
        public string Completed { get; set; } = "<color=green>Contrat rempli :</color> vous recevez %REWARD%.";

        [Description("Reponse de .contrat. %OBJECTIVE%, %PROGRESS%, %TARGET% et %REWARD% decrivent le contrat.")]
        public string Status { get; set; } = "%OBJECTIVE% Avancement : %PROGRESS%/%TARGET%. Recompense : %REWARD%.";

        [Description("Reponse de .contrat une fois le contrat rempli.")]
        public string AlreadyDone { get; set; } = "Votre contrat de ce round est rempli.";

        [Description("Reponse de .contrat sans contrat.")]
        public string NoContract { get; set; } = "Vous n'avez aucun contrat pour l'instant.";

        [Description("Reponse de .contrat trop rapprochee.")]
        public string TooFast { get; set; } = "Attendez un instant.";

        [Description("Reponse de .contrat hors du jeu.")]
        public string PlayerOnly { get; set; } = "Commande reservee aux joueurs.";

        [Description("Objectif de tuer des ennemis. %TARGET% est le nombre.")]
        public string KillEnemies { get; set; } = "Eliminez %TARGET% ennemi(s).";

        [Description("Objectif d'activer des generateurs. %TARGET% est le nombre.")]
        public string ActivateGenerators { get; set; } = "Activez %TARGET% generateur(s).";

        [Description("Objectif de s'evader.")]
        public string Escape { get; set; } = "Evadez-vous du site.";

        [Description("Objectif de se soigner. %TARGET% est le nombre.")]
        public string UseMedical { get; set; } = "Utilisez %TARGET% objet(s) de soin.";

        [Description("Objectif de survivre. %TARGET% est le nombre de minutes.")]
        public string Survive { get; set; } = "Restez en vie %TARGET% minute(s).";

        [Description("Objectif de lancer des pieces. %TARGET% est le nombre.")]
        public string FlipCoins { get; set; } = "Lancez %TARGET% piece(s).";

        [Description("Objectif d'avaler des pilules. %TARGET% est le nombre.")]
        public string SwallowPills { get; set; } = "Avalez %TARGET% pilule(s).";

        [Description("Objectif d'utiliser SCP-261. %TARGET% est le nombre.")]
        public string UseVending { get; set; } = "Utilisez SCP-261 %TARGET% fois.";

        [Description("Objectif d'echanger a SCP-1162. %TARGET% est le nombre.")]
        public string ExchangeItem { get; set; } = "Echangez %TARGET% objet(s) a SCP-1162.";
    }
}
