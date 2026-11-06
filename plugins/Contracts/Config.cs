using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Hints;
using PlayerRoles;

namespace Contracts
{
    public sealed class Config : IHintSettings
    {
        [Description("Ne distribue aucun contrat et ne compte rien pendant un mode de jeu.")]
        public bool PauseDuringGamemodes { get; set; } = true;

        [Description("Secondes apres le debut du round avant les premiers contrats.")]
        public float FirstOfferSeconds { get; set; } = 15f;

        [Description("Cooldown de la commande .contrat par joueur, en secondes.")]
        public float CommandCooldownSeconds { get; set; } = 3f;

        [Description("Roles de SCP sans contrat.")]
        public List<RoleTypeId> ExcludedScps { get; set; } = new List<RoleTypeId> { RoleTypeId.Scp079, RoleTypeId.Scp0492 };

        [Description("Contrats remplis au plus par joueur et par round, 1 au plus bas. 1 = pas de contrat suivant.")]
        public int MaxContractsPerRound { get; set; } = 2;

        [Description("Secondes entre un contrat rempli et le suivant, 5 au plus bas.")]
        public float FollowUpDelaySeconds { get; set; } = 20f;

        [Description("Contrats suivants, plus durs et mieux payes.")]
        public List<ContractOffer> FollowUpOffers { get; set; } = new List<ContractOffer>
        {
            new ContractOffer { Kind = ContractKind.KillEnemies, Target = 4, Reward = ItemType.GrenadeHE, RewardQuantity = 2, RewardLabel = "deux grenades" },
            new ContractOffer { Kind = ContractKind.ActivateGenerators, Target = 2, Weight = 6, Reward = ItemType.ArmorCombat, RewardLabel = "une armure de combat" },
            new ContractOffer { Kind = ContractKind.Survive, Target = 12, Weight = 8, Reward = ItemType.SCP500, RewardLabel = "une pilule" },
            new ContractOffer { Kind = ContractKind.FlipCoins, Target = 5, Weight = 6, Reward = ItemType.SCP207, RewardLabel = "un SCP-207" },
            new ContractOffer { Kind = ContractKind.KillEnemies, ForScps = true, Target = 5, Reward = ItemType.None, RewardHealth = 500f, RewardLabel = "500 PV" },
            new ContractOffer { Kind = ContractKind.Survive, ForScps = true, Target = 14, Weight = 6, Reward = ItemType.None, RewardHealth = 400f, RewardLabel = "400 PV" },
        };

        [Description("Contrats possibles.")]
        public List<ContractOffer> Offers { get; set; } = new List<ContractOffer>
        {
            new ContractOffer { Kind = ContractKind.KillEnemies, Target = 2, Reward = ItemType.Medkit, RewardLabel = "une trousse de soins" },
            new ContractOffer { Kind = ContractKind.ActivateGenerators, Target = 1, Weight = 6, Reward = ItemType.GrenadeFlash, RewardLabel = "une grenade aveuglante" },
            new ContractOffer { Kind = ContractKind.Escape, Target = 1, Weight = 8, Reward = ItemType.SCP500, RewardLabel = "une pilule" },
            new ContractOffer { Kind = ContractKind.UseMedical, Target = 2, Weight = 6, Reward = ItemType.Painkillers, RewardQuantity = 2, RewardLabel = "des antidouleurs" },
            new ContractOffer { Kind = ContractKind.Survive, Target = 8, Weight = 8, Reward = ItemType.ArmorCombat, RewardLabel = "une armure de combat" },
            new ContractOffer { Kind = ContractKind.FlipCoins, Target = 3, Weight = 6, Reward = ItemType.Coin, RewardQuantity = 2, RewardLabel = "deux pieces" },
            new ContractOffer { Kind = ContractKind.SwallowPills, Target = 1, Weight = 6, Reward = ItemType.Adrenaline, RewardLabel = "une adrenaline" },
            new ContractOffer { Kind = ContractKind.UseVending, Target = 1, Weight = 5, Reward = ItemType.Medkit, RewardLabel = "une trousse de soins" },
            new ContractOffer { Kind = ContractKind.ExchangeItem, Target = 2, Weight = 5, Reward = ItemType.GrenadeFlash, RewardLabel = "une grenade aveuglante" },
            new ContractOffer { Kind = ContractKind.KillEnemies, ForScps = true, Target = 3, Reward = ItemType.None, RewardHealth = 300f, RewardLabel = "300 PV" },
            new ContractOffer { Kind = ContractKind.Survive, ForScps = true, Target = 8, Weight = 6, Reward = ItemType.None, RewardHealth = 200f, RewardLabel = "200 PV" },
        };

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 8f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 950f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
