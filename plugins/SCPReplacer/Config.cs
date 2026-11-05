using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;

namespace SCPReplacer
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Temps maximum apres le debut du round, en secondes, pendant lequel le depart d'un SCP declenche une annonce de remplacement.")]
        public float QuitCutoff { get; set; } = 60f;

        [Description("Temps maximum apres le debut du round, en secondes, pendant lequel la commande .volunteer reste utilisable.")]
        public float ReplaceCutoff { get; set; } = 90f;

        [Description("Pourcentage de vie minimum (0-100) que le SCP devait avoir pour etre remplacable.")]
        public float RequiredHealthPercentage { get; set; } = 95f;

        [Description("Duree de la loterie en secondes apres le premier volontaire.")]
        public float LotteryPeriodSeconds { get; set; } = 10f;

        [Description("Cooldown en secondes entre deux utilisations d'une commande du plugin par un meme joueur.")]
        public float CommandCooldownSeconds { get; set; } = 3f;

        [Description("Autorise les joueurs vivants a se porter volontaires. Si false, seuls les spectateurs peuvent.")]
        public bool AllowAliveVolunteers { get; set; } = false;

        [Description("Autorise la commande .human permettant a un SCP de renoncer a son role en debut de round.")]
        public bool AllowHumanCommand { get; set; } = true;

        [Description("Nombre maximum de SCP remplaces par round. 0 = sans limite.")]
        public int MaxReplacementsPerRound { get; set; } = 0;

        [Description("Roles SCP ne pouvant jamais etre remplaces.")]
        public List<RoleTypeId> IgnoredRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Scp0492,
        };

        [Description("Roles humains attribues par la commande .human, avec leur poids relatif.")]
        public Dictionary<RoleTypeId, int> HumanCommandRoles { get; set; } = new Dictionary<RoleTypeId, int>
        {
            { RoleTypeId.ClassD, 45 },
            { RoleTypeId.Scientist, 45 },
            { RoleTypeId.FacilityGuard, 10 },
        };

        [Description("Objets donnes en plus au joueur qui devient Classe-D via .human.")]
        public List<ItemType> ClassDBonusItems { get; set; } = new List<ItemType>
        {
            ItemType.Flashlight,
            ItemType.Coin,
        };

        [Description("Envoie aussi les annonces dans la console joueur en plus du broadcast.")]
        public bool AnnounceInConsole { get; set; } = true;

        [Description("Duree du broadcast d'annonce de remplacement disponible, en secondes.")]
        public ushort AnnounceBroadcastDuration { get; set; } = 16;

        [Description("Duree des broadcasts de confirmation, en secondes.")]
        public ushort ResultBroadcastDuration { get; set; } = 5;

        [Description("Duree du broadcast rappelant l'existence de .human aux SCP en debut de round, en secondes. 0 = desactive.")]
        public ushort HumanHintDuration { get; set; } = 8;
    }
}
