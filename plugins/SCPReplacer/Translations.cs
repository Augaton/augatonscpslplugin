using System.ComponentModel;

namespace SCPReplacer
{
    public sealed class Translations
    {
        [Description("Prefixe ajoute devant chaque broadcast du plugin.")]
        public string BroadcastHeader { get; set; } = "<color=yellow>[Zone-Shilari]</color>\n";

        [Description("Reponse de .volunteer quand les arguments sont invalides.")]
        public string WrongUsageMessage { get; set; } = "Usage : .volunteer <numero SCP>. Exemple : .volunteer 079 ou .v 079";

        [Description("Reponse quand le delai de remplacement est depasse.")]
        public string TooLateMessage { get; set; } = "Il est trop tard dans la partie pour remplacer un SCP.";

        [Description("Reponse quand aucun SCP n'est remplacable.")]
        public string NoEligibleScpsError { get; set; } = "Aucun SCP n'est actuellement remplacable.";

        [Description("Reponse quand le numero demande n'est pas remplacable. La liste des SCP disponibles est ajoutee a la suite.")]
        public string InvalidScpError { get; set; } = "Ce numero de SCP n'est pas disponible. SCP disponibles : ";

        [Description("Reponse de .volunteer sans argument. La liste des SCP disponibles est ajoutee a la suite.")]
        public string AvailableScpsMessage { get; set; } = "SCP actuellement remplacables : ";

        [Description("Reponse quand le joueur est deja inscrit a la loterie.")]
        public string AlreadyVolunteeredMessage { get; set; } = "Vous etes deja inscrit a la loterie pour ce SCP.";

        [Description("Reponse quand un SCP tente de se porter volontaire.")]
        public string ScpCannotVolunteerMessage { get; set; } = "Les SCP ne peuvent pas utiliser cette commande.";

        [Description("Reponse quand un joueur vivant tente de se porter volontaire alors que seuls les spectateurs le peuvent.")]
        public string SpectatorOnlyMessage { get; set; } = "Seuls les spectateurs peuvent se porter volontaires.";

        [Description("Reponse de console apres inscription. %NUMBER% est remplace par le numero du SCP.")]
        public string EnteredLotteryMessage { get; set; } = "Vous participez a la loterie pour devenir SCP-%NUMBER%.";

        [Description("Broadcast apres inscription. %NUMBER% est remplace par le numero du SCP.")]
        public string EnteredLotteryBroadcast { get; set; } = "Vous participez a la loterie pour remplacer <color=red>SCP-%NUMBER%</color>";

        [Description("Broadcast aux humains quand un SCP devient remplacable. %NUMBER% est remplace par le numero du SCP.")]
        public string ReplaceBroadcast { get; set; } = "Tapez <color=green>.volunteer %NUMBER%</color> dans la console <color=orange>~</color> pour devenir <color=red>SCP-%NUMBER%</color>";

        [Description("Broadcast au gagnant de la loterie. %NUMBER% est remplace par le numero du SCP.")]
        public string ChangedSuccessfullySelfBroadcast { get; set; } = "Vous avez remplace <color=red>SCP-%NUMBER%</color>";

        [Description("Broadcast aux autres joueurs apres remplacement. %NUMBER% est remplace par le numero du SCP.")]
        public string ChangedSuccessfullyEveryoneBroadcast { get; set; } = "<color=red>SCP-%NUMBER%</color> a ete remplace";

        [Description("Reponse pendant le cooldown des commandes. %SECONDS% est remplace par le temps restant.")]
        public string CooldownMessage { get; set; } = "Patientez %SECONDS%s avant de reutiliser cette commande.";

        [Description("Reponse de .human quand la commande est desactivee.")]
        public string HumanCommandDisabled { get; set; } = "Cette commande est desactivee sur ce serveur.";

        [Description("Reponse de .human quand le joueur n'est pas SCP.")]
        public string HumanCommandNotScp { get; set; } = "Vous devez etre un SCP pour utiliser cette commande.";

        [Description("Reponse de .human quand le role du SCP est exclu.")]
        public string HumanCommandIgnoredRole { get; set; } = "Votre role ne peut pas utiliser cette commande.";

        [Description("Reponse de .human quand personne ne pourrait reprendre le role.")]
        public string HumanCommandUnavailable { get; set; } = "Votre role ne peut pas etre repris pour le moment.";

        [Description("Reponse de .human apres le delai autorise.")]
        public string HumanCommandTooLate { get; set; } = "Cette commande doit etre utilisee en debut de round.";

        [Description("Reponse de .human quand le SCP a perdu trop de vie.")]
        public string HumanCommandTooLowHealth { get; set; } = "Vous avez trop peu de vie pour utiliser cette commande.";

        [Description("Reponse de .human en cas de succes. %ROLE% est remplace par le nouveau role.")]
        public string HumanCommandSuccess { get; set; } = "Vous etes devenu %ROLE%.";

        [Description("Broadcast envoye aux SCP en debut de round pour signaler la commande .human.")]
        public string HumanHint { get; set; } = "Tapez <color=#ff4eac>.human</color> dans la console <color=#b8bd00>~</color> pour jouer un role humain a la place.";

        [Description("Reponse quand la commande est lancee depuis la console serveur.")]
        public string PlayerOnlyMessage { get; set; } = "Cette commande doit etre utilisee par un joueur.";

        [Description("Message prevu quand la loterie se termine sans volontaire valide.")]
        public string LotteryNoWinner { get; set; } = "Aucun volontaire valide, le remplacement est annule.";
    }
}
