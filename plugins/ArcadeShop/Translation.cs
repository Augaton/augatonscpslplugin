using System.ComponentModel;

namespace ArcadeShop
{
    public sealed class Translation
    {
        [Description("Hint d'un gain. %AMOUNT% = gain, %BALANCE% = solde.")]
        public string Earned { get; set; } = "+%AMOUNT% points (%BALANCE%), tapez .shop dans la console";

        [Description("Ligne de tete du catalogue. %BALANCE% = solde.")]
        public string CatalogHeader { get; set; } = "Vous avez %BALANCE% points. Acheter : .shop <nom>";

        [Description("Ligne du catalogue. %KEY% = nom a taper, %LABEL% = objet, %PRICE% = prix.")]
        public string CatalogLine { get; set; } = "%KEY% : %LABEL%, %PRICE% points";

        [Description("Ajoute a une ligne limitee. %LEFT% = achats restants.")]
        public string CatalogLimit { get; set; } = " (%LEFT% restant(s))";

        [Description("Reponse a un achat. %LABEL% = objet, %BALANCE% = solde restant.")]
        public string Bought { get; set; } = "Achete : %LABEL%. Il vous reste %BALANCE% points.";

        [Description("Reponse a une commande venue de la console du serveur.")]
        public string PlayerOnly { get; set; } = "Commande reservee aux joueurs.";

        [Description("Reponse quand aucun round ne tourne, ou pendant un mode de jeu.")]
        public string Closed { get; set; } = "La boutique est fermee pour le moment.";

        [Description("Reponse a un spectateur, un SCP ou un joueur menotte.")]
        public string NotEligible { get; set; } = "Seul un humain vivant et libre peut acheter.";

        [Description("Reponse pendant le delai entre deux achats.")]
        public string TooFast { get; set; } = "Attendez un instant avant un nouvel achat.";

        [Description("Reponse a un nom d'objet inconnu.")]
        public string UnknownItem { get; set; } = "Objet inconnu. Tapez .shop pour voir le catalogue.";

        [Description("Reponse a un achat trop cher. %BALANCE% = solde, %PRICE% = prix.")]
        public string TooExpensive { get; set; } = "Il vous manque des points : %BALANCE% sur %PRICE%.";

        [Description("Reponse quand la limite d'achat de l'objet est atteinte.")]
        public string LimitReached { get; set; } = "Limite d'achat atteinte pour ce round.";

        [Description("Hint du vainqueur d'un mode de jeu. %AMOUNT% est le bonus.")]
        public string WinnerBonus { get; set; } = "Victoire : +%AMOUNT% points au prochain round.";

        [Description("Reponse a un soin achete en pleine forme.")]
        public string AlreadyHealthy { get; set; } = "Vous etes deja en pleine forme.";

        [Description("Nom du changement de contrat dans le catalogue.")]
        public string ContractRerollLabel { get; set; } = "Changer de contrat";

        [Description("Confirmation du changement de contrat. %BALANCE% est le solde restant.")]
        public string ContractRerolled { get; set; } = "Contrat change. Il vous reste %BALANCE% points.";

        [Description("Reponse sans contrat en cours a changer.")]
        public string ContractRerollRefused { get; set; } = "Vous n'avez aucun contrat en cours a changer.";
    }
}
