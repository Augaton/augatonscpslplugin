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
                   FindFirearm(player) is not null;
        }

        public void Execute(Player player)
        {
            FirearmItem firearm = FindFirearm(player);

            if (firearm is not null)
                player.DropItem(firearm);
        }

        // Le joueur tient la piece au moment du lancer : l'arme est forcement dans l'inventaire.
        private static FirearmItem FindFirearm(Player player)
        {
            foreach (Item item in player.Items)
            {
                if (item is FirearmItem firearm)
                    return firearm;
            }

            return null;
        }
    }
}
