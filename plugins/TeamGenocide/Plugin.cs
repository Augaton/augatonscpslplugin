using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;
using TeamGenocide.API;
using TeamGenocide.Handlers;

namespace TeamGenocide
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public override string Name => "TeamGenocide";

        public override string Description => "Annonce l'extinction complete d'une equipe.";

        public override string Author => "Zone-Shilari (base: Heisenberg3666)";

        public static Plugin Instance { get; private set; }

        private GenocideHandlers genocideHandlers;

        public override void Enable()
        {
            Instance = this;


            genocideHandlers = new GenocideHandlers(this);

            PlayerEvents.ChangingRole += genocideHandlers.OnChangingRole;
            ServerEvents.RoundStarted += genocideHandlers.OnRoundStarted;
            ServerEvents.RoundEnded += genocideHandlers.OnRoundEnded;
            ServerEvents.RoundRestarted += genocideHandlers.OnRoundRestarted;

            PluginBus.Subscribe(Name, BusTopics.TeamCheckRequested, genocideHandlers.OnTeamCheckRequested);

            PluginDirectory.Register(
                this,
                Capability.Hints,
                Capability.Light,
                Capability.Genocide,
                Capability.Bus);
        }

        public override void Disable()
        {
            if (genocideHandlers is not null)
            {
                PlayerEvents.ChangingRole -= genocideHandlers.OnChangingRole;
                ServerEvents.RoundStarted -= genocideHandlers.OnRoundStarted;
                ServerEvents.RoundEnded -= genocideHandlers.OnRoundEnded;
                ServerEvents.RoundRestarted -= genocideHandlers.OnRoundRestarted;

                PluginBus.UnsubscribeOwner(Name);

                genocideHandlers.Reset();
            }

            PluginDirectory.Unregister(this);

            genocideHandlers = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.ActivationDelay < 0f)
            {
                Logger.Warn($"ActivationDelay ({Config.ActivationDelay}) negatif, remis a 0.");
                Config.ActivationDelay = 0f;
            }

            if (Config.Announcements is null)
            {
                Logger.Warn("Announcements est vide, aucune extinction ne sera annoncee.");
                Config.Announcements = new Dictionary<Team, List<Announcement>>();
            }
        }
    }
}
