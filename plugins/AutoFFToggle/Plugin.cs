using AugatonLib.FriendlyFire;
using AugatonLib.Runtime;
using AutoFFToggle.Handlers;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace AutoFFToggle
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public override string Name => "AutoFFToggle";

        public override string Description => "Coupe le tir allie pendant le round et le reactive a la fin.";

        public override string Author => "Zone-Shilari (base: Kognity)";

        public static Plugin Instance { get; private set; }

        private ServerHandlers serverHandlers;

        public override void Enable()
        {
            Instance = this;

            serverHandlers = new ServerHandlers(this);

            ServerEvents.RoundStarted += serverHandlers.OnRoundStarted;
            ServerEvents.RoundEnded += serverHandlers.OnRoundEnded;
            ServerEvents.RoundRestarted += serverHandlers.OnRoundRestarted;

            if (Round.IsRoundInProgress)
                serverHandlers.OnRoundStarted();

            PluginDirectory.Register(this, Capability.FriendlyFire);
        }

        public override void Disable()
        {
            if (serverHandlers is not null)
            {
                ServerEvents.RoundStarted -= serverHandlers.OnRoundStarted;
                ServerEvents.RoundEnded -= serverHandlers.OnRoundEnded;
                ServerEvents.RoundRestarted -= serverHandlers.OnRoundRestarted;

                serverHandlers.Reset();
            }

            RestoreOriginalFriendlyFire();

            PluginDirectory.Unregister(this);

            serverHandlers = null;
            Instance = null;
        }

        public void RestoreOriginalFriendlyFire()
        {
            ServerHandlers.ReleaseArbiter();

            Logger.Debug($"Tir allie relache. {FriendlyFireArbiter.Describe()}", Config.Debug);
        }

        protected override void ValidateConfig()
        {
            if (Config.EnableDelay < 0f)
            {
                Logger.Warn($"EnableDelay ({Config.EnableDelay}) negatif, remis a 0.");
                Config.EnableDelay = 0f;
            }

            if (Server.FriendlyFire && Config.DisableDuringRound)
                Logger.Warn("Le tir allie est active dans la config serveur. AutoFFToggle le coupera au debut de chaque round et le restaurera a la desactivation.");
        }
    }
}
