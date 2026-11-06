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
                   FindEmptied(player) is not null;
        }

        public void Execute(Player player)
        {
            FirearmItem firearm = FindEmptied(player);

            if (firearm is not null)
                firearm.StoredAmmo = firearm.MaxAmmo;
        }

        // Le joueur tient la piece au moment du lancer : l'arme est forcement dans l'inventaire.
        private static FirearmItem FindEmptied(Player player)
        {
            foreach (Item item in player.Items)
            {
                if (item is FirearmItem firearm && firearm.StoredAmmo < firearm.MaxAmmo)
                    return firearm;
            }

            return null;
        }
    }
}
