using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.Caching;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;
using SCPReplacer.API;
using SCPReplacer.Handlers;

namespace SCPReplacer
{
    public sealed class Plugin : AugatonPlugin<Config, Translations>
    {
        public override string Name => "SCPReplacer";

        public override string Description => "Permet aux joueurs de se porter volontaires pour remplacer un SCP parti en debut de round.";

        public override string Author => "Zone-Shilari (base: Jon M)";

        public const string DepartureOwner = "SCPReplacer";

        public static Plugin Instance { get; private set; }

        public ReplacementService Service { get; private set; }

        public CooldownTracker Cooldowns { get; private set; }

        private ServerHandlers serverHandlers;

        public override void Enable()
        {
            Instance = this;

            Service = new ReplacementService(this);
            Cooldowns = new CooldownTracker();
            serverHandlers = new ServerHandlers(this);

            // ServerEvents.RoundStarted part avant l'attribution des roles ; le rappel de .human lit le role.
            CharacterClassManager.OnRoundStarted += serverHandlers.OnRoundStarted;
            ServerEvents.RoundEnded += serverHandlers.OnRoundEnded;
            ServerEvents.RoundRestarted += serverHandlers.OnRoundRestarted;
            PlayerDeparture.Departing += serverHandlers.OnDeparting;

            DepartureArbiter.Declare(DepartureOwner, 100, Service.CanClaimDeparture);

            PluginBus.Subscribe(DepartureOwner, BusTopics.GamemodeStarted, _ => Service.GamemodeActive = true);
            PluginBus.Subscribe(DepartureOwner, BusTopics.GamemodeEnded, _ => Service.GamemodeActive = false);

            PluginDirectory.Register(this, Capability.Departure, Capability.Genocide, Capability.Bus);
        }

        public override void Disable()
        {
            if (serverHandlers is not null)
            {
                CharacterClassManager.OnRoundStarted -= serverHandlers.OnRoundStarted;
                ServerEvents.RoundEnded -= serverHandlers.OnRoundEnded;
                ServerEvents.RoundRestarted -= serverHandlers.OnRoundRestarted;
                PlayerDeparture.Departing -= serverHandlers.OnDeparting;
            }

            Service?.Reset();
            Cooldowns?.Clear();

            DepartureArbiter.Withdraw(DepartureOwner);
            PluginBus.UnsubscribeOwner(DepartureOwner);
            PluginDirectory.Unregister(this);

            serverHandlers = null;
            Service = null;
            Cooldowns = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.RequiredHealthPercentage < 0f || Config.RequiredHealthPercentage > 100f)
            {
                Logger.Warn($"RequiredHealthPercentage ({Config.RequiredHealthPercentage}) hors bornes, remis a 95.");
                Config.RequiredHealthPercentage = 95f;
            }

            if (Config.LotteryPeriodSeconds <= 0f)
            {
                Logger.Warn($"LotteryPeriodSeconds ({Config.LotteryPeriodSeconds}) invalide, remis a 10.");
                Config.LotteryPeriodSeconds = 10f;
            }

            if (Config.ReplaceCutoff < Config.QuitCutoff)
            {
                Logger.Warn($"ReplaceCutoff ({Config.ReplaceCutoff}) est inferieur a QuitCutoff ({Config.QuitCutoff}), aligne sur QuitCutoff.");
                Config.ReplaceCutoff = Config.QuitCutoff;
            }

            Config.CommandCooldownSeconds = AtLeast("command_cooldown_seconds", Config.CommandCooldownSeconds, 1f);

            if (Config.IgnoredRoles is null)
            {
                Logger.Warn("IgnoredRoles absent, remis a une liste vide.");
                Config.IgnoredRoles = new List<RoleTypeId>();
            }

            if (Config.HumanCommandRoles is null)
            {
                Logger.Warn("HumanCommandRoles absent, remis a une table vide.");
                Config.HumanCommandRoles = new Dictionary<RoleTypeId, int>();
            }

            if (Config.ClassDBonusItems is null)
            {
                Logger.Warn("ClassDBonusItems absent, remis a une liste vide.");
                Config.ClassDBonusItems = new List<ItemType>();
            }
        }
    }
}
