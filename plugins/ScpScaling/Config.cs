using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;

namespace ScpScaling
{
    public sealed class Config
    {
        [Description("Nombre de joueurs pour lequel un SCP garde ses PV normaux.")]
        public int ReferencePlayers { get; set; } = 16;

        [Description("PV gagnes ou perdus par joueur d'ecart avec la reference, en pourcentage.")]
        public float PercentPerPlayer { get; set; } = 2.5f;

        [Description("Facteur minimal applique aux PV. 0.6 = 60 %.")]
        public float MinFactor { get; set; } = 0.6f;

        [Description("Facteur maximal applique aux PV. 1.5 = 150 %.")]
        public float MaxFactor { get; set; } = 1.5f;

        [Description("Roles SCP laisses a leurs PV normaux.")]
        public List<RoleTypeId> ExcludedRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Scp079,
            RoleTypeId.Scp0492,
        };

        [Description("Ne change rien pendant un mode de jeu, qui regle lui-meme les PV.")]
        public bool PauseDuringGamemodes { get; set; } = true;
    }
}
