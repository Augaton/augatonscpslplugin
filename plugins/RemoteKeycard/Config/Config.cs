using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;

namespace RemoteKeycard.Config
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("L'effet Amnesie empeche l'utilisation a distance des cartes.")]
        public bool AmnesiaMatters { get; set; } = true;

        [Description("Le plugin agit sur les generateurs.")]
        public bool AffectGenerators { get; set; } = true;

        [Description("Le plugin agit sur le panneau de l'ogive.")]
        public bool AffectWarheadPanel { get; set; } = true;

        [Description("Le plugin agit sur les casiers SCP.")]
        public bool AffectScpLockers { get; set; } = true;

        [Description("Le plugin agit sur les portes.")]
        public bool AffectDoors { get; set; } = true;

        [Description("Roles ne beneficiant jamais de l'ouverture a distance.")]
        public List<RoleTypeId> IgnoredRoles { get; set; } = new List<RoleTypeId>();

        [Description("La carte doit etre tenue en main plutot que simplement presente dans l'inventaire.")]
        public bool RequireHeldKeycard { get; set; } = false;

        [Description("Reglages destines aux developpeurs.")]
        public ExtraSettings Extras { get; set; } = new ExtraSettings();
    }
}
