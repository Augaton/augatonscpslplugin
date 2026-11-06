using System;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.Caching;
using AugatonLib.Runtime;
using AugatonLib.World;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace Scp261
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public const string Owner = "Scp261";

        private readonly CooldownTracker cooldowns = new CooldownTracker();

        public static Plugin Instance { get; private set; }

        internal VendingMachines Machines => machines;

        private VendingMachines machines;
        private VendingOutcomes outcomes;

        public override string Name => Owner;

        public override string Description => "Machine distributrice SCP-261 : une piece contre un resultat au hasard.";

        public override string Author => "MedveMarci (modifie par Zone-Shilari)";

        public override void Enable()
        {
            Instance = this;
            machines = new VendingMachines(this);
            outcomes = new VendingOutcomes(this);

            PlayerEvents.InteractedToy += OnInteractedToy;
            PlayerEvents.ChangedRole += OnChangedRole;
            ServerEvents.RoundStarted += machines.SpawnForRound;
            ServerEvents.RoundRestarted += OnRoundRestarted;

            SelfCheck.Register(Name, "machine en place", () => Round.IsRoundStarted && machines.Count == 0 ? "absente de ce round" : null);

            PluginDirectory.Register(this, Capability.Hints, Capability.Scale, Capability.Bus);
        }

        public override void Disable()
        {
            PlayerEvents.InteractedToy -= OnInteractedToy;
            PlayerEvents.ChangedRole -= OnChangedRole;
            ServerEvents.RoundStarted -= machines.SpawnForRound;
            ServerEvents.RoundRestarted -= OnRoundRestarted;

            OnRoundRestarted();
            PluginDirectory.Unregister(this);

            machines = null;
            outcomes = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.UseCooldownSeconds = AtLeast("use_cooldown_seconds", Config.UseCooldownSeconds, 0.5f);
        }

        private void OnRoundRestarted()
        {
            machines.Clear();
            cooldowns.Clear();
            Hints.Clear();
            ScaleArbiter.ReleaseOwner(Owner);
        }

        private void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                ScaleArbiter.Release(ev?.Player, Owner);
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangedRole: {e}");
            }
        }

        private void OnInteractedToy(PlayerInteractedToyEventArgs ev)
        {
            try
            {
                if (ev?.Player is not null && ev.Interactable?.GameObject?.name == VendingMachines.InteractableName)
                    Use(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnInteractedToy: {e}");
            }
        }

        private void Use(Player player)
        {
            if (cooldowns.IsOnCooldown(player, Config.UseCooldownSeconds, out _))
                return;

            cooldowns.Register(player);

            if (player.Role == RoleTypeId.Scp3114 && !Config.CanScp3114Use)
            {
                Hints.Show(player, Translation.Denied, Config.HintDuration);
                return;
            }

            if (Config.PauseDuringGamemodes && WorldEvents.IsPaused)
            {
                Hints.Show(player, Translation.Paused, Config.HintDuration);
                return;
            }

            Item held = player.CurrentItem;

            if (held is null || SpentItems.IsSpent(held.Serial) || Config.ActivationItems is null || !Config.ActivationItems.Contains(held.Type))
            {
                Hints.Show(player, Translation.MissingItem, Config.HintDuration);
                return;
            }

            player.RemoveItem(held);
            outcomes.Roll(player);
            PluginBus.Publish(BusTopics.VendingUsed, Owner, player);
        }
    }
}
