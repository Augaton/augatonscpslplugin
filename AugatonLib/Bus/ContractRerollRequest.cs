using LabApi.Features.Wrappers;

namespace AugatonLib.Bus
{
    public sealed class ContractRerollRequest
    {
        public ContractRerollRequest(Player player) => Player = player;

        public Player Player { get; }

        public bool Accepted { get; set; }
    }
}
