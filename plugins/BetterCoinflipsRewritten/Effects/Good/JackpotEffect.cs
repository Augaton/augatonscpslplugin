using System.Collections.Generic;
using AugatonLib.Random;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class JackpotEffect : ICoinEffect
    {
        private readonly IReadOnlyList<ItemType> items;
        private readonly int amount;

        public JackpotEffect(IReadOnlyList<ItemType> items, int amount)
        {
            this.items = items;
            this.amount = amount;
        }

        public string Name
        {
            get { return "Jackpot"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.JackpotEffectMessage; }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   items is not null &&
                   items.Count > 0;
        }

        public void Execute(Player player)
        {
            for (int i = 0; i < amount; i++)
                PickupDrop.AtFeet(player, Chance.Pick(items));
        }
    }
}
