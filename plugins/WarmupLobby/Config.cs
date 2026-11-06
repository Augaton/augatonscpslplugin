using System.ComponentModel;
using AugatonLib.Hints;
using PlayerRoles;

namespace WarmupLobby
{
    public sealed class Config : IHintSettings
    {
        [Description("Premier camp de l'echauffement.")]
        public RoleTypeId FirstRole { get; set; } = RoleTypeId.NtfPrivate;

        [Description("Second camp de l'echauffement, ennemi du premier.")]
        public RoleTypeId SecondRole { get; set; } = RoleTypeId.ChaosConscript;

        [Description("Role dont le point d'apparition sert d'arene : tous les joueurs y apparaissent.")]
        public RoleTypeId ArenaSpawnRole { get; set; } = RoleTypeId.ChaosConscript;

        [Description("Secondes entre deux reapparitions des morts, 1 au plus bas.")]
        public float RespawnSeconds { get; set; } = 3f;

        [Description("Joueurs requis pour lancer l'echauffement.")]
        public int MinPlayers { get; set; } = 2;

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 4f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 300f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
