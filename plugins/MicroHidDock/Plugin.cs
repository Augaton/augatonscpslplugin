using System;
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

            SelfCheck.Register(Name, "socle du Micro-HID", MicroDock.CheckPedestal);

            PluginDirectory.Register(this, Capability.Hints);
        }

        public override void Disable()
        {
            PlayerEvents.InteractingLocker -= OnInteractingLocker;
            PlayerEvents.DroppedItem -= OnDroppedItem;
            ServerEvents.RoundStarted -= dock.Start;
            ServerEvents.RoundRestarted -= OnRoundRestarted;

            OnRoundRestarted();
            PluginDirectory.Unregister(this);

            dock = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.DockCooldownSeconds = AtLeast("dock_cooldown_seconds", Config.DockCooldownSeconds, 1f);

            if (Config.DockRange < 0f)
            {
                Logger.Warn("dock_range negatif : valeur remise a 0.");
                Config.DockRange = 0f;
            }

            if (Config.RechargePerSecond < 0f)
            {
                Logger.Warn("recharge_per_second negatif : recharge coupee.");
                Config.RechargePerSecond = 0f;
            }

            if (Config.RechargeLimit < 0f || Config.RechargeLimit > 1f)
            {
                Logger.Warn($"recharge_limit {Config.RechargeLimit} hors de 0-1 : valeur bornee.");
                Config.RechargeLimit = Math.Max(0f, Math.Min(1f, Config.RechargeLimit));
            }
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
