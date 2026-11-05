using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Good
{
    public sealed class MedicalKitEffect : ICoinEffect
    {
        private bool droppedOnGround;

        public string Name
        {
            get { return "MedicalKit"; }
        }

        public string Message
        {
            get
            {
                if (droppedOnGround)
                {
                    return Plugin.Instance.Translation
                        .MedicalKitDroppedMessage;
                }

                return Plugin.Instance.Translation
                    .MedicalKitEffectMessage;
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
            droppedOnGround = false;

            bool alreadyHasMedkit = false;

            foreach (Item item in player.Items)
            {
                if (item is null || item.Type != ItemType.Medkit)
                    continue;

                alreadyHasMedkit = true;
                break;
            }

            if (!alreadyHasMedkit)
            {
                API.PickupDrop.Give(player, ItemType.Medkit);
                return;
            }

            API.PickupDrop.AtFeet(player, ItemType.Medkit);

            droppedOnGround = true;
        }
    }
}
