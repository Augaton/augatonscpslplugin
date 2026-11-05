using System;
using AugatonLib.Arbitration;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace SCPReplacer.Handlers
{
    public sealed class ServerHandlers
    {
        private readonly Plugin plugin;

        public ServerHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnRoundStarted()
        {
            try
            {
                plugin.Service.Reset();
                plugin.Cooldowns.Clear();

                if (plugin.Config.HumanHintDuration == 0 || !plugin.Config.AllowHumanCommand)
                    return;

                foreach (Player player in Player.ReadyList)
                {
                    if (!player.IsSCP)
                        continue;

                    player.SendBroadcast(plugin.Translation.HumanHint, plugin.Config.HumanHintDuration);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev) => Cleanup();

        public void OnRoundRestarted() => Cleanup();

        public void OnDeparting(Player leaver)
        {
            try
            {
                plugin.Cooldowns.Remove(leaver.UserId);
                plugin.Service.RemoveVolunteer(leaver.UserId);

                if (!DepartureArbiter.IsOwner(leaver, Plugin.DepartureOwner))
                    return;

                plugin.Service.TryRegisterDeparture(leaver, out _);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeparting: {e}");
            }
        }

        private void Cleanup()
        {
            try
            {
                plugin.Service.Reset();
                plugin.Cooldowns.Clear();
            }
            catch (Exception e)
            {
                Logger.Error($"Cleanup: {e}");
            }
        }
    }
}
