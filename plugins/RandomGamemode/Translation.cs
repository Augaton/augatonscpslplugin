using System.ComponentModel;

namespace RandomGamemode
{
    public sealed class Translation
    {
        [Description("Reponse quand la cle de mode demandee n'existe pas. %KEY% est remplace par la cle saisie.")]
        public string UnknownGamemode { get; set; } = "Mode \"%KEY%\" inconnu. Utilisez gamemode list pour voir les cles disponibles.";

        [Description("Reponse quand un mode est force pour le prochain round. %KEY% est remplace par la cle du mode.")]
        public string GamemodeForced { get; set; } = "Le mode \"%KEY%\" sera active au prochain round.";

        [Description("Reponse quand le forcage de mode est annule.")]
        public string GamemodeCleared { get; set; } = "Le forcage de mode a ete annule.";

        [Description("Reponse quand un mode est demarre immediatement. %KEY% est remplace par la cle du mode.")]
        public string GamemodeStarted { get; set; } = "Le mode \"%KEY%\" a ete demarre immediatement.";

        [Description("Reponse quand le mode en cours est arrete.")]
        public string GamemodeStopped { get; set; } = "Le mode en cours a ete arrete.";

        [Description("Reponse quand aucun mode n'est actif.")]
        public string NoActiveGamemode { get; set; } = "Aucun mode n'est actif.";

        [Description("Reponse quand un mode tourne deja.")]
        public string AlreadyRunning { get; set; } = "Un mode est deja en cours. Utilisez gamemode stop d'abord.";

        [Description("Reponse quand la commande exige un round en cours.")]
        public string RoundNotStarted { get; set; } = "Le round n'a pas commence.";

        [Description("Broadcast d'annonce du Juggernaut. %PLAYER% est remplace par son pseudo.")]
        public string JuggernautAnnouncement { get; set; } = "<size=30><color=red>%PLAYER% est le Juggernaut. Abattez-le.</color></size>";

        [Description("Broadcast a la mort du Juggernaut. %PLAYER% est remplace par son pseudo.")]
        public string JuggernautDefeated { get; set; } = "<size=30><color=green>Le Juggernaut %PLAYER% est tombe.</color></size>";

        [Description("Broadcast du vainqueur du Gun Game. %PLAYER% est remplace par son pseudo.")]
        public string GunGameWinner { get; set; } = "<size=35><color=yellow>%PLAYER% a termine le Gun Game.</color></size>";

        [Description("Hint de passage de palier au Gun Game. %TIER% est remplace par le numero du palier.")]
        public string GunGameTier { get; set; } = "<color=yellow>Palier %TIER%</color>";

        [Description("Broadcast du vote. %OPTIONS% est remplace par la liste numerotee des modes.")]
        public string VoteBroadcast { get; set; } = "<size=28><color=yellow>Mode special au prochain round ! Console (~) : .votemode numero</color>\n%OPTIONS%</size>";

        [Description("Reponse quand aucun vote n'est ouvert.")]
        public string VoteClosed { get; set; } = "Aucun vote n'est ouvert.";

        [Description("Reponse a un numero invalide.")]
        public string VoteInvalid { get; set; } = "Numero invalide. Tapez .votemode pour voir les choix.";

        [Description("Reponse quand le joueur vote trop vite.")]
        public string VoteTooFast { get; set; } = "Attendez un instant avant de revoter.";

        [Description("Reponse quand la commande ne vient pas d'un joueur.")]
        public string VotePlayerOnly { get; set; } = "Commande reservee aux joueurs.";

        [Description("Cache-cache : le chasseur entre en jeu.")]
        public string HideAndSeekReleased { get; set; } = "<size=30><color=red>Le chasseur est lache.</color></size>";

        [Description("Cache-cache : les survivants ont tenu.")]
        public string HideAndSeekSurvived { get; set; } = "<size=30><color=green>Les survivants ont tenu jusqu'au bout.</color></size>";

        [Description("Confirmation du vote. %MODE% est remplace par le mode choisi.")]
        public string VoteRegistered { get; set; } = "Vote enregistre pour %MODE%.";
    }
}
