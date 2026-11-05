using InventorySystem.Items.Usables.Scp330;
using LabApi.Features.Wrappers;
using UnityEngine;
using Scp330Pickup = LabApi.Features.Wrappers.Scp330Pickup;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class PinkCandyEffect : ICoinEffect
    {
        public string Name
        {
            get { return "PinkCandy"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .PinkCandyEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive;
        }

        public void Execute(Player player)
        {
            Scp330Pickup bag = Pickup.Create(
                ItemType.SCP330,
                player.Position,
                Quaternion.identity,
                Vector3.one,
                false) as Scp330Pickup;

            if (bag is null)
                return;

            // Le sac cree par Item.Create contenait deja un bonbon tire au hasard : le joueur en recoit deux.
            bag.Candies.Add(Scp330Candies.GetRandom());
            bag.Candies.Add(CandyKindID.Pink);
            bag.Spawn();
        }
    }
}
