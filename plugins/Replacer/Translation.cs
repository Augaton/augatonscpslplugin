using System.ComponentModel;

namespace Replacer
{
    public sealed class Translation
    {
        [Description("Hint affiche au remplacant. {nickname} est remplace par le pseudo du joueur parti, {role} par son role.")]
        public string ReplacedHint { get; set; } = "<color=yellow>Vous remplacez <b>{nickname}</b> en tant que {role}.</color>";

        [Description("Broadcast diffuse a tous lors d'un remplacement, si announce_to_server est actif.")]
        public string ReplacedBroadcast { get; set; } = "<color=yellow>Un joueur a quitte la partie et a ete remplace.</color>";

        [Description("Pseudo utilise quand celui du joueur parti est vide ou refuse par le filtre de texte.")]
        public string UnknownNickname { get; set; } = "un joueur deconnecte";

        [Description("Reponse a .noreplace quand le joueur refuse d'etre remplacant.")]
        public string OptedOutMessage { get; set; } = "Vous ne serez pas choisi comme remplacant pendant ce round.";

        [Description("Reponse a .noreplace quand le joueur accepte de nouveau.")]
        public string OptedInMessage { get; set; } = "Vous pouvez de nouveau etre choisi comme remplacant.";

        [Description("Reponse quand la commande est reutilisee trop vite. {seconds} est le temps restant.")]
        public string CooldownMessage { get; set; } = "Attendez encore {seconds} s.";

        [Description("Reponse quand la commande ne vient pas d'un joueur.")]
        public string PlayerOnlyMessage { get; set; } = "Commande reservee aux joueurs.";
    }
}
