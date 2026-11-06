using System.ComponentModel;

namespace Scp261
{
    public sealed class Translation
    {
        [Description("Hint quand la machine donne un objet.")]
        public string ItemDispensed { get; set; } = "<b>La machine <color=#42f57b>a sorti quelque chose.</color></b>";

        [Description("Hint quand la machine applique un effet.")]
        public string EffectApplied { get; set; } = "<b>La machine <color=#42f57b>tremble violemment.</color></b>";

        [Description("Hint du retrecissement.")]
        public string Shrunk { get; set; } = "<b>La machine <color=#42f57b>vous a rapetisse.</color></b>";

        [Description("Hint de l'explosion.")]
        public string Exploded { get; set; } = "<b>La machine <color=#42f57b>a explose.</color></b>";

        [Description("Hint de la grenade flash.")]
        public string Flashbang { get; set; } = "<b>La machine <color=#42f57b>a sorti une canette etrange.</color></b>";

        [Description("Hint de la flaque de tantrum.")]
        public string Tantrum { get; set; } = "<b>Quelque chose <color=#42f57b>a coule de la machine.</color></b>";

        [Description("Hint de la teleportation.")]
        public string Teleported { get; set; } = "<b>Vous avez ete teleporte quelque part.</b>";

        [Description("Hint quand le joueur ne tient pas un objet accepte.")]
        public string MissingItem { get; set; } = "<b>Il vous faut <color=red>une piece en main.</color></b>";

        [Description("Hint quand le joueur n'a pas le droit d'utiliser la machine.")]
        public string Denied { get; set; } = "<b>Vous <color=red>ne pouvez pas utiliser SCP-261.</color></b>";

        [Description("Annonce du jackpot. %PLAYER% est le pseudo, %COUNT% le nombre de lots.")]
        public string Jackpot { get; set; } = "<b><color=yellow>JACKPOT !</color> %PLAYER% remporte %COUNT% lot(s) a SCP-261.</b>";

        [Description("Hint quand la machine est suspendue pendant un mode de jeu.")]
        public string Paused { get; set; } = "<b>La machine <color=red>est hors service.</color></b>";
    }
}
