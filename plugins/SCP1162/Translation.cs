using System.ComponentModel;

namespace SCP1162
{
    public sealed class Translation
    {
        [Description("Verbe insere dans {action} quand l'objet est depose.")]
        public string DroppedAction { get; set; } = "depose";

        [Description("Verbe insere dans {action} quand l'objet est lance.")]
        public string ThrownAction { get; set; } = "lance";

        [Description("Hint d'echange reussi. {action} est remplace par le verbe, {item} par l'objet donne, {reward} par l'objet recu.")]
        public string ItemExchanged { get; set; } = "<b>Vous avez {action} <color=#7FFF7F>{item}</color> dans <color=yellow>SCP-1162</color> et recupere <color=#FF7F7F>{reward}</color>.</b>";

        [Description("Hint quand l'objet est perdu sans contrepartie. {action} est remplace par le verbe, {item} par l'objet donne.")]
        public string ItemLost { get; set; } = "<b>Vous avez {action} <color=#7FFF7F>{item}</color> dans <color=yellow>SCP-1162</color> et n'avez <color=#FF7F7F>rien</color> recupere.</b>";

        [Description("Hint quand SCP-1162 arrache les mains du joueur.")]
        public string HandsSevered { get; set; } = "<b><color=#FF7F7F>SCP-1162</color> vous a arrache les mains.</b>";

        [Description("Reponse de commande apres placement. {count} est remplace par le nombre d'emplacements.")]
        public string Spawned { get; set; } = "SCP-1162 a ete place ({count} emplacement(s)).";

        [Description("Reponse de commande quand aucun emplacement valide n'existe.")]
        public string NotSpawned { get; set; } = "Aucun emplacement valide, SCP-1162 n'a pas pu etre place.";

        [Description("Reponse de commande quand SCP-1162 est deja place.")]
        public string AlreadySpawned { get; set; } = "SCP-1162 est deja present.";

        [Description("Reponse de commande apres retrait.")]
        public string Despawned { get; set; } = "SCP-1162 a ete retire.";

        [Description("Reponse de commande quand SCP-1162 est deja absent.")]
        public string AlreadyDespawned { get; set; } = "SCP-1162 est deja absent.";
    }
}
