using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Caching;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;
using Replacer.API;
using Replacer.Handlers;

namespace Replacer
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public override string Name => "Replacer";

        public override string Description => "Remplace un joueur qui quitte la partie par un spectateur.";

        public override string Author => "Zone-Shilari (base: DGvagabond)";

        public const string DepartureOwner = "Replacer";

        public static Plugin Instance { get; private set; }

        public SpectatorQueue Queue { get; private set; }

        public CooldownTracker Cooldowns { get; private set; }

        private PlayerHandlers playerHandlers;

        public override void Enable()
        {
            Instance = this;


            Queue = new SpectatorQueue();
            Cooldowns = new CooldownTracker();
            playerHandlers = new PlayerHandlers(this, Queue);

            PlayerDeparture.Departing += playerHandlers.OnDeparting;
            PlayerEvents.ChangingRole += playerHandlers.OnChangingRole;

            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += OnRoundRestarted;

            DepartureArbiter.Declare(DepartureOwner, 50, player => player is not null);

            PluginDirectory.Register(
                this,
                Capability.Hints,
                Capability.Departure,
                Capability.Bus);
        }

        public override void Disable()
        {
            if (playerHandlers is not null)
            {
                PlayerDeparture.Departing -= playerHandlers.OnDeparting;
                PlayerEvents.ChangingRole -= playerHandlers.OnChangingRole;

                playerHandlers.Reset();
            }

            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= OnRoundRestarted;

            Queue?.Clear();
            Cooldowns?.Clear();
            Hints.Clear();

            DepartureArbiter.Withdraw(DepartureOwner);
            PluginDirectory.Unregister(this);

            playerHandlers = null;
            Queue = null;
            Cooldowns = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.ReplacementDelay < 0f)
            {
                Logger.Warn($"ReplacementDelay ({Config.ReplacementDelay}) est negatif, remis a 0.4.");
                Config.ReplacementDelay = 0.4f;
            }

            if (Config.MinimumPlayers < 1)
            {
                Logger.Warn($"MinimumPlayers ({Config.MinimumPlayers}) est invalide, remis a 1.");
                Config.MinimumPlayers = 1;
            }

            if (Config.PerPlayerCooldownSeconds < 0f)
            {
                Logger.Warn($"PerPlayerCooldownSeconds ({Config.PerPlayerCooldownSeconds}) est negatif, remis a 0.");
                Config.PerPlayerCooldownSeconds = 0f;
            }

            if (Config.PendingRoleSeconds < 0f)
            {
                Logger.Warn($"PendingRoleSeconds ({Config.PendingRoleSeconds}) est negatif, remis a 0.");
                Config.PendingRoleSeconds = 0f;
            }

            if (Config.ReplaceableRoles is null)
            {
                Logger.Warn("ReplaceableRoles est absent, remis a une liste vide.");
                Config.ReplaceableRoles = new List<RoleTypeId>();
            }

            if (Config.IgnoredRoles is null)
            {
                Logger.Warn("IgnoredRoles est absent, remis a une liste vide.");
                Config.IgnoredRoles = new List<RoleTypeId>();
            }
        }

        private void OnRoundStarted()
        {
            Queue.Clear();
            Hints.Clear();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Queue.Clear();

        private void OnRoundRestarted()
        {
            Queue.Clear();
            Queue.ClearOptOuts();
            Cooldowns.Clear();
            playerHandlers.Reset();
            Hints.Clear();
        }
    }
}
