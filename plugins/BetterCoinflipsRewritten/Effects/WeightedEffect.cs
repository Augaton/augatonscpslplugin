namespace BetterCoinflipsRewritten.Effects
{
    public sealed class WeightedEffect
    {
        public WeightedEffect(
            ICoinEffect effect,
            int weight)
        {
            Effect = effect;
            Weight = weight;
        }

        public ICoinEffect Effect { get; private set; }

        public int Weight { get; private set; }
    }
}
