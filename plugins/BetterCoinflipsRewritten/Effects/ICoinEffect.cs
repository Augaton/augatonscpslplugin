using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects
{
    public interface ICoinEffect
    {
        string Name { get; }

        string Message { get; }

        bool CanApply(Player player);

        void Execute(Player player);
    }
}
