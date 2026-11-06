using System;
using AugatonLib.FriendlyFire;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;

namespace AutoFFToggle.Handlers
{
    public sealed class ServerHandlers
    {
        internal const string Owner = "AutoFFToggle";

        internal const string ManualOwner = "AutoFFToggle.Manual";

        private readonly Plugin plugin;

        private CoroutineHandle enableHandle;
        private bool enableScheduled;

        public ServerHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnRoundStarted()
        {
            try
            {
                KillPendingEnable();

                if (!plugin.Config.DisableDuringRound)
                    return;

                FriendlyFireArbiter.RequestOff(Owner);

                Logger.Debug($"Tir allie coupe pour la duree du round. {FriendlyFireArbiter.Describe()}", plugin.Config.Debug);
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            try
            {
                if (!plugin.Config.EnableOnRoundEnd)
                    return;

                if (plugin.Config.EnableDelay <= 0f)
                {
                    EnableFriendlyFire();
                    return;
                }

                KillPendingEnable();
                enableScheduled = true;
                enableHandle = Timing.CallDelayed(plugin.Config.EnableDelay, EnableFriendlyFire);
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundEnded: {e}");
            }
        }

        public void OnRoundRestarted()
        {
            try
            {
                KillPendingEnable();

                // AugatonLib survit au round : un forcage staff oublie resterait pose sur tous les suivants.
                FriendlyFireArbiter.Release(ManualOwner);

                if (plugin.Config.RestoreOnRoundRestart)
                    FriendlyFireArbiter.Release(Owner);
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundRestarted: {e}");
            }
        }

        public void Reset() => KillPendingEnable();

        internal static void ReleaseArbiter()
        {
            FriendlyFireArbiter.Release(Owner);
            FriendlyFireArbiter.Release(ManualOwner);
        }

        private void EnableFriendlyFire()
        {
            try
            {
                enableScheduled = false;
                FriendlyFireArbiter.RequestOn(Owner);

                Logger.Debug($"Tir allie reactive. {FriendlyFireArbiter.Describe()}", plugin.Config.Debug);

                if (!plugin.Config.AnnounceOnEnable)
                    return;

                Server.SendBroadcast(plugin.Translation.FriendlyFireEnabled, plugin.Config.AnnounceDuration);
            }
            catch (Exception e)
            {
                Logger.Error($"EnableFriendlyFire: {e}");
            }
        }

        private void KillPendingEnable()
        {
            if (!enableScheduled)
                return;

            Timing.KillCoroutines(enableHandle);
            enableScheduled = false;
        }
    }
}
