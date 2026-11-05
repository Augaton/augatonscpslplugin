using System;
using AugatonLib.Arbitration;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using RemoteKeycard.API;
using RemoteKeycard.API.Enums;
using RemoteKeycard.API.EventArgs;
using RemoteKeycard.API.Extensions;

namespace RemoteKeycard.Handlers
{
    public sealed class KeycardHandlers
    {
        private readonly Plugin plugin;

        public KeycardHandlers(Plugin plugin) => this.plugin = plugin;

        public void Start()
        {
            PlayerEvents.InteractingDoor += OnInteractingDoor;
            PlayerEvents.UnlockingGenerator += OnUnlockingGenerator;
            PlayerEvents.InteractingLocker += OnInteractingLocker;
            PlayerEvents.UnlockingWarheadButton += OnUnlockingWarheadButton;
        }

        public void Stop()
        {
            PlayerEvents.InteractingDoor -= OnInteractingDoor;
            PlayerEvents.UnlockingGenerator -= OnUnlockingGenerator;
            PlayerEvents.InteractingLocker -= OnInteractingLocker;
            PlayerEvents.UnlockingWarheadButton -= OnUnlockingWarheadButton;
        }

        private void OnInteractingDoor(PlayerInteractingDoorEventArgs ev)
        {
            try
            {
                // Sur une porte verrouillee, le jeu emet aussi cet event, et CanOpen y leve le verrou.
                if (!plugin.Config.AffectDoors || ev?.Player is null || ev.Door is null || !ev.IsAllowed || ev.CanOpen || ev.Door.IsLocked)
                    return;

                if (DoorLockRegistry.IsLocked(ev.Door.DoorName, out string lockOwner))
                {
                    Logger.Debug($"{ev.Door.DoorName} laissee fermee pour {ev.Player.Nickname} : verrou pose par {lockOwner}.", plugin.Config.Debug);
                    return;
                }

                if (!ev.Player.HasKeycardPermission(ev.Door.Permissions, ev.Door.Base.RequiredPermissions.RequireAll))
                    return;

                if (!Approve(ev.Player, RemoteTarget.Door, ev))
                    return;

                ev.CanOpen = true;
                Trace(ev.Player, "porte", ev.Door.DoorName.ToString());
            }
            catch (Exception e)
            {
                Logger.Error($"OnInteractingDoor: {e}");
            }
        }

        private void OnUnlockingGenerator(PlayerUnlockingGeneratorEventArgs ev)
        {
            try
            {
                if (!plugin.Config.AffectGenerators || ev?.Player is null || ev.Generator is null || !ev.IsAllowed || ev.CanOpen)
                    return;

                if (!ev.Player.HasKeycardPermission(ev.Generator.RequiredPermissions))
                    return;

                if (!Approve(ev.Player, RemoteTarget.Generator, ev))
                    return;

                ev.CanOpen = true;
                Trace(ev.Player, "generateur", ev.Generator.Room?.Name.ToString());
            }
            catch (Exception e)
            {
                Logger.Error($"OnUnlockingGenerator: {e}");
            }
        }

        private void OnUnlockingWarheadButton(PlayerUnlockingWarheadButtonEventArgs ev)
        {
            try
            {
                // Ici IsAllowed porte le resultat du controle de carte, pas une annulation.
                if (!plugin.Config.AffectWarheadPanel || ev?.Player is null || ev.IsAllowed)
                    return;

                if (!ev.Player.HasKeycardPermission(DoorPermissionFlags.AlphaWarhead))
                    return;

                if (!Approve(ev.Player, RemoteTarget.WarheadPanel, ev))
                    return;

                ev.IsAllowed = true;
                Trace(ev.Player, "panneau d'ogive", null);
            }
            catch (Exception e)
            {
                Logger.Error($"OnUnlockingWarheadButton: {e}");
            }
        }

        private void OnInteractingLocker(PlayerInteractingLockerEventArgs ev)
        {
            try
            {
                if (!plugin.Config.AffectScpLockers || ev?.Player is null || ev.Chamber is null || !ev.IsAllowed || ev.CanOpen)
                    return;

                if (!ev.Player.HasKeycardPermission(ev.Chamber.RequiredPermissions, true))
                    return;

                if (!Approve(ev.Player, RemoteTarget.ScpLocker, ev))
                    return;

                ev.CanOpen = true;
                Trace(ev.Player, "casier", null);
            }
            catch (Exception e)
            {
                Logger.Error($"OnInteractingLocker: {e}");
            }
        }

        private bool Approve(Player player, RemoteTarget target, object original)
        {
            if (!plugin.Config.Extras.EnableEvents)
                return true;

            UsingKeycardEventArgs args = new UsingKeycardEventArgs(player, target, original);
            Events.OnUsingKeycard(args);
            return args.IsAllowed;
        }

        private void Trace(Player player, string target, string detail)
        {
            if (!plugin.Config.Debug)
                return;

            Logger.Debug($"{player.Nickname} a ouvert un/une {target} a distance{(string.IsNullOrEmpty(detail) ? string.Empty : $" ({detail})")}.", true);
        }
    }
}
