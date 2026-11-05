using System.ComponentModel;

namespace SupplyDrop
{
    public sealed class Translation
    {
        [Description("Reponse quand la commande exige un round en cours.")]
        public string RoundNotStarted { get; set; } = "Le round n'a pas commence.";

        [Description("Reponse quand la cle de profil n'existe pas. %KEY% est remplace par la cle saisie.")]
        public string UnknownProfile { get; set; } = "Profil \"%KEY%\" inconnu. Utilisez supplydrop list pour voir les cles disponibles.";

        [Description("Reponse apres declenchement manuel d'un largage. %KEY% est remplace par la cle du profil.")]
        public string DropCalled { get; set; } = "Largage \"%KEY%\" declenche.";

        [Description("Reponse quand aucun profil de largage n'est configure.")]
        public string NoProfile { get; set; } = "Aucun profil de largage configure.";
    }
}
