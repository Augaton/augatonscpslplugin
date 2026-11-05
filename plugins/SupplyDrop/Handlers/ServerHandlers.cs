using System;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
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

        public void OnGeneratorActivated(GeneratorActivatedEventArgs ev) => Guard(service.OnGeneratorActivated);

        public void OnDecontaminationStarted() => Guard(service.OnDecontaminationStarted);

        public void OnWarheadDetonated(WarheadDetonatedEventArgs ev) => Guard(service.OnWarheadDetonated);

        public void OnRoundEnded(RoundEndedEventArgs ev) => Cleanup();

        public void OnRoundRestarted() => Cleanup();

        public void OnWaitingForPlayers() => Cleanup();

        private static void Guard(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error($"Declencheur de largage : {e}");
            }
        }

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
