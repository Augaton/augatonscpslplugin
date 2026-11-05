using System;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using BetterCoinflipsRewritten.API;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using MEC;

namespace BetterCoinflipsRewritten
{
    public sealed class EventHandlers
    {
        private readonly Plugin plugin;
        private readonly CoinService coinService;
        private readonly CoinSpawnService coinSpawnService;
        private readonly CoinConsumeService coinConsumeService;
        private readonly CoinRenewalService coinRenewalService;

        private CoroutineHandle spawnHandle;
        private bool spawnScheduled;

        public EventHandlers(
            Plugin plugin,
            CoinService coinService,
            CoinSpawnService coinSpawnService,
            CoinConsumeService coinConsumeService,
            CoinRenewalService coinRenewalService)
        {
            this.plugin = plugin;
            this.coinService = coinService;
            this.coinSpawnService = coinSpawnService;
            this.coinConsumeService = coinConsumeService;
            this.coinRenewalService = coinRenewalService;
        }

        public void OnFlippingCoin(PlayerFlippingCoinEventArgs ev)
        {
            try
            {
                if (ev is null ||
                    ev.Player is null ||
                    ev.Player.IsDestroyed ||
                    !ev.Player.IsAlive)
                {
                    return;
                }

                double secondsRemaining;

                if (coinService.IsOnCooldown(
                    ev.Player,
                    out secondsRemaining))
                {
                    ev.IsAllowed = false;

                    string message =
                        plugin.Translation.CooldownMessage.Replace(
                            "{seconds}",
                            secondsRemaining.ToString("0.0"));

                    plugin.Hints.Show(
                        ev.Player,
                        message,
                        plugin.Config.CooldownMessageDuration);

                    Logger.Debug(
                        "[BetterCoinflipsRewritten] " +
                        ev.Player.Nickname +
                        " attempted to flip a coin during cooldown. " +
                        "Remaining: " +
                        secondsRemaining.ToString("0.0") +
                        " seconds.",
                        plugin.Config.Debug);

                    return;
                }

                coinService.SetCooldown(ev.Player);

                Logger.Debug(
                    "[BetterCoinflipsRewritten] Coin flip detected for " +
                    ev.Player.Nickname +
                    ". IsTails: " +
                    ev.IsTails +
                    ".",
                    plugin.Config.Debug);

                coinConsumeService.Schedule(ev.Player, ev.CoinItem);

                coinService.ExecuteFlip(
                    ev.Player,
                    ev.IsTails);

                PluginBus.Publish(
                    BusTopics.CoinFlipped,
                    Interop.Owner,
                    ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnFlippingCoin: {e}");
            }
        }

        public void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                if (ev is null || ev.Player is null)
                    return;

                coinService.RemovePlayer(ev.Player);
                plugin.Hints.Remove(ev.Player);
                ScaleArbiter.Forget(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        public void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                ScaleArbiter.Release(ev?.Player, Interop.Owner);
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangedRole: {e}");
            }
        }

        public void OnRoundStarted()
        {
            try
            {
                coinService.Clear();
                plugin.Hints.Clear();
                API.RoomCache.Rebuild();
                ReleaseArbiters();
                API.GlobalCooldown.Clear();
                API.Scheduler.Clear();

                Logger.Debug(
                    "[BetterCoinflipsRewritten] Round started. " +
                    "Player data has been cleared.",
                    plugin.Config.Debug);

                KillPendingSpawn();

                spawnScheduled = true;
                spawnHandle = Timing.CallDelayed(
                    plugin.Config.CoinSpawnDelay,
                    delegate
                    {
                        spawnScheduled = false;

                        try
                        {
                            coinSpawnService.SpawnMapCoins();
                            coinRenewalService.Start();
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"SpawnMapCoins: {e}");
                        }
                    });
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        public void OnRoundRestarted()
        {
            try
            {
                KillPendingSpawn();
                coinService.Clear();
                plugin.Hints.Clear();
                API.RoomCache.Clear();
                ReleaseArbiters();
                API.GlobalCooldown.Clear();
                API.Scheduler.Clear();
                coinRenewalService.Stop();

                Logger.Debug(
                    "[BetterCoinflipsRewritten] Round is restarting.",
                    plugin.Config.Debug);
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundRestarted: {e}");
            }
        }

        public static void ReleaseArbiters()
        {
            ScaleArbiter.ReleaseOwner(Interop.Owner);
            LightArbiter.ReleaseOwner(Interop.Owner);
        }

        public void KillPendingSpawn()
        {
            if (!spawnScheduled)
                return;

            Timing.KillCoroutines(spawnHandle);
            spawnScheduled = false;
        }
    }
}
