using System.ComponentModel;

namespace Contracts
{
    public sealed class ContractOffer
    {
        [Description("Objectif : KillEnemies, ActivateGenerators, Escape, UseMedical ou Survive.")]
        public ContractKind Kind { get; set; } = ContractKind.KillEnemies;

        [Description("Quantite a atteindre. Pour Survive, en minutes.")]
        public int Target { get; set; } = 1;

        [Description("Poids dans le tirage. 0 = jamais.")]
        public int Weight { get; set; } = 10;

        [Description("Contrat reserve aux SCP. Les autres vont aux humains.")]
        public bool ForScps { get; set; }

        [Description("Vie rendue en recompense, utile pour un SCP qui n'a pas d'inventaire. 0 = aucune.")]
        public float RewardHealth { get; set; }

        [Description("Objet donne en recompense. None = aucun. SCP500 donne une vraie pilule si SCP500s est installe.")]
        public ItemType Reward { get; set; } = ItemType.Medkit;

        [Description("Nombre d'objets de la recompense.")]
        public int RewardQuantity { get; set; } = 1;

        [Description("Nom de la recompense, affiche au joueur.")]
        public string RewardLabel { get; set; } = "une trousse de soins";
    }
}
