using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class FullMagazineEffect : ICoinEffect
    {
        public string Name
        {
            get { return "FullMagazine"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.FullMagazineEffectMessage; }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   player.CurrentItem is FirearmItem firearm &&
                   firearm.StoredAmmo < firearm.MaxAmmo;
        }

        public void Execute(Player player)
        {
            if (player.CurrentItem is FirearmItem firearm)
                firearm.StoredAmmo = firearm.MaxAmmo;
        }
    }
}
