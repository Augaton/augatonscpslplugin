using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Hints;
using PlayerRoles;

namespace ScpLevels
{
    public sealed class Config : IHintSettings
    {
        [Description("Niveaux des SCP. Un meme seuil ne compte qu'une fois.")]
        public List<ScpLevel> Levels { get; set; } = new List<ScpLevel>
        {
            new ScpLevel { Kills = 2, MovementBoost = 3, BonusHealth = 100f },
            new ScpLevel { Kills = 4, MovementBoost = 6, BonusHealth = 150f, RegenPerSecond = 1f },
            new ScpLevel { Kills = 7, MovementBoost = 9, BonusHealth = 200f, RegenPerSecond = 2f },
            new ScpLevel { Kills = 11, MovementBoost = 12, BonusHealth = 250f, RegenPerSecond = 3f },
        };

        [Description("Un kill de SCP-049-2 compte pour SCP-049.")]
        public bool ZombieKillsFeedScp049 { get; set; } = true;

        [Description("Roles SCP qui ne montent pas de niveau.")]
        public List<RoleTypeId> ExcludedRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Scp079,
            RoleTypeId.Scp0492,
        };

        [Description("Annonce chaque niveau a tous les joueurs. Sinon, seul le SCP le voit.")]
        public bool AnnounceLevels { get; set; } = true;

        [Description("Ne compte rien pendant un mode de jeu.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 4f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 350f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
