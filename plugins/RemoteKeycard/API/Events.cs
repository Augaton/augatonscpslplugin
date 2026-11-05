using LabApi.Events;
using RemoteKeycard.API.EventArgs;

namespace RemoteKeycard.API
{
    public static class Events
    {
        public static event LabEventHandler<UsingKeycardEventArgs> UsingKeycard;

        public static void OnUsingKeycard(UsingKeycardEventArgs ev) => UsingKeycard.InvokeEvent(ev);
    }
}
