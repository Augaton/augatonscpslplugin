using System.ComponentModel;
using AugatonLib.Hints;

namespace RoundMutators
{
    public sealed class Config : IHintSettings
    {
        [Description("Chance qu'un round recoive un mutateur, en pourcentage.")]
        public float ChancePercent { get; set; } = 25f;

        [Description("Rounds normaux garantis apres un mutateur.")]
        public int RoundsBetween { get; set; } = 1;

        [Description("Joueurs requis pour un mutateur.")]
        public int MinPlayers { get; set; } = 6;

        [Description("Secondes apres le debut du round avant le tirage, 3 au plus bas. Laisse un mode de jeu demarrer d'abord.")]
        public float StartDelaySeconds { get; set; } = 10f;

        [Description("Niveau du Directeur du chaos qui declenche un mutateur en cours de round. 0 = jamais.")]
        public int ChaosLevelForMutator { get; set; } = 3;

        [Description("Poids du mutateur de gravite. 0 = jamais.")]
        public int LowGravityWeight { get; set; } = 10;

        [Description("Poids du mutateur de vitesse. 0 = jamais.")]
        public int SwiftWeight { get; set; } = 10;

        [Description("Poids du mutateur de taille. 0 = jamais.")]
        public int TinyWeight { get; set; } = 10;

        [Description("Poids du mutateur d'endurance. 0 = jamais.")]
        public int TirelessWeight { get; set; } = 10;

        [Description("Poids du mutateur d'obscurite. 0 = jamais.")]
        public int NightFallWeight { get; set; } = 10;

        [Description("Poids du mutateur de munitions. 0 = jamais.")]
        public int ScarceAmmoWeight { get; set; } = 10;

        [Description("Poids du mutateur de soins. 0 = jamais.")]
        public int ScarceMedicalWeight { get; set; } = 10;

        [Description("Poids du mutateur de vitesse des SCP. 0 = jamais.")]
        public int SwiftScpsWeight { get; set; } = 10;

        [Description("Poids du mutateur de vie. 0 = jamais.")]
        public int FragileHumansWeight { get; set; } = 10;

        [Description("Poids du mutateur de vampirisme. 0 = jamais.")]
        public int VampireWeight { get; set; } = 10;

        [Description("Poids du mutateur de degats. 0 = jamais.")]
        public int GlassCannonWeight { get; set; } = 10;

        [Description("Poids du mutateur de munitions sans fin. 0 = jamais.")]
        public int EndlessAmmoWeight { get; set; } = 10;

        [Description("Poids du mutateur de regeneration. 0 = jamais.")]
        public int RegenerationWeight { get; set; } = 10;

        [Description("Vie rendue par un kill avec Vampire.")]
        public float VampireHeal { get; set; } = 25f;

        [Description("Multiplicateur des degats entre joueurs avec GlassCannon, 1 au plus bas.")]
        public float GlassCannonFactor { get; set; } = 1.5f;

        [Description("Vie rendue par seconde aux humains avec Regeneration.")]
        public float RegenerationPerSecond { get; set; } = 1f;

        [Description("Part retiree de la carte avec ScarceAmmo et ScarceMedical, en pourcentage.")]
        public float ScarcePercent { get; set; } = 60f;

        [Description("Vitesse ajoutee aux SCP avec SwiftScps, en pourcentage, 100 au plus.")]
        public int SwiftScpPercent { get; set; } = 15;

        [Description("Vie maximale des humains avec FragileHumans, en pourcentage de la normale.")]
        public float FragileHealthPercent { get; set; } = 80f;

        [Description("Gravite des humains avec LowGravity. 0.8 = 80 % de la normale, 0.2 au plus bas.")]
        public float GravityFactor { get; set; } = 0.8f;

        [Description("Vitesse ajoutee aux humains avec Swift, en pourcentage, 100 au plus.")]
        public int SpeedPercent { get; set; } = 8;

        [Description("Taille des humains avec Tiny. 0.9 = 90 % de leur taille, 0.5 au plus bas.")]
        public float SizeFactor { get; set; } = 0.9f;

        [Description("Aucun mutateur pendant un mode de jeu, et un mode qui demarre retire le mutateur en cours.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Duree du hint d'annonce, en secondes.")]
        public float HintDuration { get; set; } = 6f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 200f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
