using System;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using InventorySystem.Items.MicroHID.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MicroHIDPedestal = InventorySystem.Items.MicroHID.MicroHIDPedestal;

namespace MicroHidDock
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        private MicroDock dock;

        public static Plugin Instance { get; private set; }

        internal MicroDock Dock => dock;

        public override string Name => "MicroHidDock";

        public override string Description => "Permet de reposer le Micro-HID sur son socle, ou il se recharge.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            dock = new MicroDock(this);

            PlayerEvents.InteractingLocker += OnInteractingLocker;
            PlayerEvents.DroppedItem += OnDroppedItem;
            ServerEvents.RoundStarted += dock.Start;
            ServerEvents.RoundRestarted += OnRoundRestarted;

            PluginBus.Subscribe(Name, BusTopics.SiteIncident, dock.OnSiteIncident);

            SelfCheck.Register(Name, "socle du Micro-HID", MicroDock.CheckPedestal);

            PluginDirectory.Register(this, Capability.Hints, Capability.Bus);
        }

        public override void Disable()
        {
            PlayerEvents.InteractingLocker -= OnInteractingLocker;
            PlayerEvents.DroppedItem -= OnDroppedItem;
            ServerEvents.RoundStarted -= dock.Start;
            ServerEvents.RoundRestarted -= OnRoundRestarted;

            OnRoundRestarted();
            PluginBus.UnsubscribeOwner(Name);
            PluginDirectory.Unregister(this);

            dock = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.DockCooldownSeconds = AtLeast("dock_cooldown_seconds", Config.DockCooldownSeconds, 1f);
            Config.DockRange = AtLeast("dock_range", Config.DockRange, 0f);
            Config.RechargePerSecond = AtLeast("recharge_per_second", Config.RechargePerSecond, 0f);
            Config.RechargeLimit = Math.Min(1f, AtLeast("recharge_limit", Config.RechargeLimit, 0f));
            Config.TeslaBoostFactor = AtLeast("tesla_boost_factor", Config.TeslaBoostFactor, 1f);
            Config.TeslaBoostSeconds = AtLeast("tesla_boost_seconds", Config.TeslaBoostSeconds, 0f);
            Config.BlackoutPauseSeconds = AtLeast("blackout_pause_seconds", Config.BlackoutPauseSeconds, 0f);
        }

        private void OnRoundRestarted()
        {
            dock.Stop();
            Hints.Clear();
        }

        private void OnInteractingLocker(PlayerInteractingLockerEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || ev.Locker?.Base is not MicroHIDPedestal || ev.Chamber is null)
                    return;

                if (ev.Player.CurrentItem is not MicroHIDItem micro || micro.Phase != MicroHidPhase.Standby)
                    return;

                if (!ev.Chamber.IsEmpty || dock.IsOnCooldown(ev.Player))
                    return;

                ev.IsAllowed = false;
                dock.Seat(ev.Player, ev.Player.DropItem(micro) as MicroHIDPickup, ev.Chamber);
            }
            catch (Exception e)
            {
                Logger.Error($"OnInteractingLocker: {e}");
            }
        }

        private void OnDroppedItem(PlayerDroppedItemEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || ev.Pickup is not MicroHIDPickup pickup || pickup.Phase != MicroHidPhase.Standby)
                    return;

                if (!dock.TryFindFreeChamber(pickup.Position, out LockerChamber chamber) || dock.IsOnCooldown(ev.Player))
                    return;

                ev.Throw = false;
                dock.Seat(ev.Player, pickup, chamber);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDroppedItem: {e}");
            }
        }
    }
}
