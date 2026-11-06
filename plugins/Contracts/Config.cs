using System.Collections.Generic;
using System.ComponentModel;
using AugatonLib.Hints;

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

        [Description("Contrats possibles.")]
        public List<ContractOffer> Offers { get; set; } = new List<ContractOffer>
        {
            new ContractOffer { Kind = ContractKind.KillEnemies, Target = 2, Reward = ItemType.Medkit, RewardLabel = "une trousse de soins" },
            new ContractOffer { Kind = ContractKind.ActivateGenerators, Target = 1, Weight = 6, Reward = ItemType.GrenadeFlash, RewardLabel = "une grenade aveuglante" },
            new ContractOffer { Kind = ContractKind.Escape, Target = 1, Weight = 8, Reward = ItemType.SCP500, RewardLabel = "une pilule" },
            new ContractOffer { Kind = ContractKind.UseMedical, Target = 2, Weight = 6, Reward = ItemType.Painkillers, RewardQuantity = 2, RewardLabel = "des antidouleurs" },
            new ContractOffer { Kind = ContractKind.Survive, Target = 8, Weight = 8, Reward = ItemType.ArmorCombat, RewardLabel = "une armure de combat" },
        };

        [Description("Duree des hints, en secondes.")]
        public float HintDuration { get; set; } = 8f;

        [Description("Position verticale du hint de ce plugin dans HintServiceMeow. Doit differer de celle des autres plugins.")]
        public float HintYCoordinate { get; set; } = 950f;

        [Description("Taille de police du hint de ce plugin.")]
        public int HintFontSize { get; set; } = 20;
    }
}
