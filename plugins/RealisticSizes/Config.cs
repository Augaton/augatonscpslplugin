using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using RealisticSizes.API;

namespace RealisticSizes
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Mode actif : Roleplay (variations discretes), Fun (variations larges) ou Manual (par role).")]
        public SizeMode ActiveMode { get; set; } = SizeMode.Fun;

        [Description("Autorise des valeurs differentes en hauteur et en largeur. Si false, la silhouette reste proportionnee.")]
        public bool AllowUnproportionalValues { get; set; } = false;

        [Description("Applique aussi une taille aux SCP. Deconseille : modifie les hitbox et casse l'equilibrage.")]
        public bool AffectScps { get; set; } = false;

        [Description("Roles ne recevant jamais de taille personnalisee.")]
        public List<RoleTypeId> IgnoredRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Tutorial,
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker,
            RoleTypeId.Scp079,
        };

        [Description("Delai en secondes avant d'appliquer la taille, le temps que le spawn se termine.")]
        public float ApplyDelay { get; set; } = 0.4f;

        [Description("Etale l'application des tailles sur plusieurs frames quand beaucoup de joueurs apparaissent en meme temps.")]
        public bool SpreadWorkload { get; set; } = true;

        [Description("Nombre de joueurs connectes a partir duquel l'etalement s'active.")]
        public int SpreadMinPlayers { get; set; } = 5;

        [Description("Ecart en secondes ajoute entre chaque joueur lors de l'etalement.")]
        public float SpreadStep { get; set; } = 0.15f;

        [Description("Un remplacant de Replacer garde la taille du joueur parti. Sans Replacer, ce reglage n'a aucun effet.")]
        public bool KeepSizeOnReplacement { get; set; } = true;

        [Description("La taille influe sur les humains : petit = plus rapide, grand = plus de vie. Desactive par defaut, a equilibrer.")]
        public bool EnableSizeGameplay { get; set; } = false;

        [Description("Vitesse gagnee ou perdue, en pourcent, par ecart de 0.1 de taille.")]
        public float SpeedPercentPerTenth { get; set; } = 3f;

        [Description("Vie gagnee ou perdue, en pourcent, par ecart de 0.1 de taille.")]
        public float HealthPercentPerTenth { get; set; } = 5f;

        [Description("Ecart maximal de vitesse, en pourcent.")]
        public float MaxSpeedPercent { get; set; } = 15f;

        [Description("Ecart maximal de vie, en pourcent.")]
        public float MaxHealthPercent { get; set; } = 20f;

        [Description("Plage utilisee en mode Roleplay.")]
        public SizeRange RoleplayRange { get; set; } = new SizeRange
        {
            MinHeight = 0.92f,
            MaxHeight = 1.08f,
            MinWidth = 0.94f,
            MaxWidth = 1.06f,
        };

        [Description("Plage utilisee en mode Fun.")]
        public SizeRange FunRange { get; set; } = new SizeRange
        {
            MinHeight = 0.55f,
            MaxHeight = 1.35f,
            MinWidth = 0.6f,
            MaxWidth = 1.3f,
        };

        [Description("Plages par role, utilisees en mode Manual. Un role absent garde sa taille normale.")]
        public Dictionary<RoleTypeId, SizeRange> ManualRanges { get; set; } = new Dictionary<RoleTypeId, SizeRange>
        {
            [RoleTypeId.ClassD] = new SizeRange { MinHeight = 0.9f, MaxHeight = 1.12f, MinWidth = 0.9f, MaxWidth = 1.15f },
            [RoleTypeId.Scientist] = new SizeRange { MinHeight = 0.9f, MaxHeight = 1.08f, MinWidth = 0.88f, MaxWidth = 1.05f },
            [RoleTypeId.FacilityGuard] = new SizeRange { MinHeight = 0.98f, MaxHeight = 1.12f, MinWidth = 1f, MaxWidth = 1.12f },
        };
    }
}
