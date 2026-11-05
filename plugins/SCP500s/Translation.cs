using System.ComponentModel;

namespace SCP500s
{
    public sealed class Translation
    {
        [Description("Hint affiche quand le joueur tente de prendre une pilule pendant son cooldown. La prise est bloquee et la pilule reste dans l'inventaire.")]
        public string PillCooldown { get; set; } = "<color=orange>Attendez avant de consommer une autre pilule.</color>";

        [Description("Hint affiche au joueur deplace par une pilule d'echange de position.")]
        public string SwapVictim { get; set; } = "<color=#00CED1>Quelqu'un vient d'echanger sa place avec la votre.</color>";

        [Description("Hint affiche au joueur attire par une pilule d'invocation.")]
        public string SummonVictim { get; set; } = "<color=#E9967A>Une force vous a attire ailleurs.</color>";

        [Description("Hint affiche au joueur touche par la contagion d'une pilule voisine.")]
        public string ContagionVictim { get; set; } = "<color=#9ACD32>Quelqu'un a pris une pilule a cote de vous. Vous aussi, en fait.</color>";

        [Description("Broadcast diffuse a tous quand une pilule force une vague de renfort.")]
        public string ForceWaveBroadcast { get; set; } = "<color=#FFA07A>Une pilule SCP-500 vient d'appeler du renfort.</color>";

        [Description("Cause de deces affichee quand une pilule tue le joueur.")]
        public string DamageReason { get; set; } = "Effets d'une pilule SCP-500 experimentale.";

        [Description("Texte affiche quand on ramasse ou selectionne une pilule. {name} est remplace par son nom, {description} par sa description, {color} par la couleur de son halo.")]
        public string ItemInfo { get; set; } = "<b><color={color}>{name}</color></b>\n{description}";

        [Description("Resultat de la pilule de recensement. {scp}, {foundation}, {chaos}, {scientists} et {classd} sont remplaces par le nombre de survivants de chaque camp.")]
        public string Census { get; set; } = "<color=#87CEFA><b>Recensement</b>\nSCP : {scp}  |  Fondation : {foundation}  |  Chaos : {chaos}\nScientifiques : {scientists}  |  Classe-D : {classd}</color>";

        [Description("En-tete du resultat de la pilule radar.")]
        public string RadarHeader { get; set; } = "<color=#FF6347><b>Signatures SCP detectees</b></color>";

        [Description("Ligne du radar pour un SCP. {role} est remplace par le SCP, {distance} par sa distance en metres.")]
        public string RadarLine { get; set; } = "{role} a {distance} m";

        [Description("Message de la pilule radar quand aucun SCP n'est en vie.")]
        public string RadarEmpty { get; set; } = "<color=#FF6347>Aucune signature SCP detectee.</color>";
    }
}
