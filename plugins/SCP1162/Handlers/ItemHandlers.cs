using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using SCP1162.API;

namespace SCP1162.Handlers
{
    public sealed class ItemHandlers
    {
        private readonly Scp1162Service service;

        public ItemHandlers(Scp1162Service service) => this.service = service;

        public void OnDroppingItem(PlayerDroppingItemEventArgs ev)
        {
            try
            {
                if (ev is null || ev.Player is null || ev.Item is null || !ev.IsAllowed)
                    return;

                ev.IsAllowed = service.AllowDrop(ev.Player, ev.Item, ev.Throw);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDroppingItem: {e}");
            }
        }

        public void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                if (ev?.Player is null)
                    return;

                service.Forget(ev.Player);
                Plugin.Instance.Hints.Remove(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }
    }
}
