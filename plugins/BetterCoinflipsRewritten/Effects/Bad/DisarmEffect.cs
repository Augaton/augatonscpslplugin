using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class DisarmEffect : ICoinEffect
    {
        public string Name
        {
            get { return "Disarm"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.DisarmEffectMessage; }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   player.CurrentItem is FirearmItem;
        }

        public void Execute(Player player)
        {
            if (player.CurrentItem is FirearmItem firearm)
                player.DropItem(firearm);
        }
    }
}
