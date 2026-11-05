using System;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using SupplyDrop.API;

namespace SupplyDrop.Handlers
{
    public sealed class ServerHandlers
    {
        private readonly DropService service;

        public ServerHandlers(DropService service) => this.service = service;

        public void OnRoundStarted()
        {
            try
            {
                service.Start();
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev) => Cleanup();

        public void OnRoundRestarted() => Cleanup();

        public void OnWaitingForPlayers() => Cleanup();

        private void Cleanup()
        {
            try
            {
                service.Stop();
            }
            catch (Exception e)
            {
                Logger.Error($"Cleanup: {e}");
            }
        }
    }
}
