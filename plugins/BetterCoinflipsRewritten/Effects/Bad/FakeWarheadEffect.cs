using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class FakeWarheadEffect : ICoinEffect
    {
        private readonly float dimDuration;

        public FakeWarheadEffect(float dimDuration)
        {
            this.dimDuration = dimDuration;
        }

        public string Name
        {
            get { return "FakeWarhead"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .FakeWarheadEffectMessage;
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
            API.TargetedRpc.WarheadShake(player, false);

            if (dimDuration <= 0f)
                return;

            API.TargetedRpc.DimScreen(player);

            API.Scheduler.Delay(
                dimDuration,
                delegate
                {
                    API.TargetedRpc.UndimScreen(player);
                });
        }
    }
}
