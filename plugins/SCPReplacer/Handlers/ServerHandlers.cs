using System;
using AugatonLib.Arbitration;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;

namespace SCPReplacer.Handlers
{
    public sealed class ServerHandlers
    {
        private const float ReminderDelaySeconds = 1f;

        private readonly Plugin plugin;

        private CoroutineHandle reminder;
        private bool reminderScheduled;

        public ServerHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnRoundStarted()
        {
            try
            {
                plugin.Service.Reset();
                plugin.Cooldowns.Clear();

                if (plugin.Config.HumanHintDuration == 0 || !plugin.Config.AllowHumanCommand)
                    return;

                // Un mode de jeu part sur le meme evenement : un instant plus tard, on sait s'il coupe .human.
                CancelReminder();
                reminderScheduled = true;
                reminder = Timing.CallDelayed(ReminderDelaySeconds, SendReminder);
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        public void CancelReminder()
        {
            if (reminderScheduled)
                Timing.KillCoroutines(reminder);

            reminderScheduled = false;
        }

        private void SendReminder()
        {
            reminderScheduled = false;

            try
            {
                if (plugin.Service.IsSuspended)
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
                Logger.Error($"SendReminder: {e}");
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
                CancelReminder();
                plugin.Service.Reset();
                plugin.Service.GamemodeActive = false;
                plugin.Cooldowns.Clear();
            }
            catch (Exception e)
            {
                Logger.Error($"Cleanup: {e}");
            }
        }
    }
}
