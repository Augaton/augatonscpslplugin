using LabApi.Features.Wrappers;
using RemoteKeycard.API.Enums;

namespace RemoteKeycard.API.EventArgs
{
    public sealed class UsingKeycardEventArgs : System.EventArgs
    {
        public UsingKeycardEventArgs(Player player, RemoteTarget target, object originalEvent, bool isAllowed = true)
        {
            Player = player;
            Target = target;
            OriginalEvent = originalEvent;
            IsAllowed = isAllowed;
        }

        public Player Player { get; }

        public RemoteTarget Target { get; }

        public object OriginalEvent { get; }

        public bool IsAllowed { get; set; }
    }
}
