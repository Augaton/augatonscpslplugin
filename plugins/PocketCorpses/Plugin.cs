using System;
using AugatonLib.Random;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace PocketCorpses
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        private PocketReturns returns;

        public override string Name => "PocketCorpses";

        public override string Description => "Les corps laisses dans la dimension de poche retombent dans le complexe.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            returns = new PocketReturns(this);

            PlayerEvents.SpawnedRagdoll += OnSpawnedRagdoll;
            ServerEvents.RoundStarted += returns.Start;
            ServerEvents.RoundRestarted += returns.Stop;

            PluginDirectory.Register(this);
        }

        public override void Disable()
        {
            PlayerEvents.SpawnedRagdoll -= OnSpawnedRagdoll;
            ServerEvents.RoundStarted -= returns.Start;
            ServerEvents.RoundRestarted -= returns.Stop;

            returns.Stop();
            PluginDirectory.Unregister(this);

            returns = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.ReturnChance < 0f || Config.ReturnChance > 100f)
            {
                Logger.Warn($"return_chance {Config.ReturnChance} hors de 0-100 : valeur bornee.");
                Config.ReturnChance = Math.Max(0f, Math.Min(100f, Config.ReturnChance));
            }

            if (Config.WarningSeconds < 0f)
            {
                Logger.Warn("warning_seconds negatif : valeur remise a 0.");
                Config.WarningSeconds = 0f;
            }

            if (Config.MinDelaySeconds < Config.WarningSeconds)
            {
                Logger.Warn("min_delay_seconds inferieur a warning_seconds : valeur alignee.");
                Config.MinDelaySeconds = Config.WarningSeconds;
            }

            if (Config.MaxDelaySeconds < Config.MinDelaySeconds)
            {
                Logger.Warn("max_delay_seconds inferieur a min_delay_seconds : valeur alignee.");
                Config.MaxDelaySeconds = Config.MinDelaySeconds;
            }

            if (Config.SinkholeSeconds < 0f)
            {
                Logger.Warn("sinkhole_seconds negatif : valeur remise a 0.");
                Config.SinkholeSeconds = 0f;
            }
        }

        private void OnSpawnedRagdoll(PlayerSpawnedRagdollEventArgs ev)
        {
            try
            {
                if (ev?.Ragdoll is null || returns is null)
                    return;

                // Le transform du corps n'est place qu'a son Start : seule sa donnee synchronisee est exacte ici.
                if (DropPoints.IsInPocket(ev.Ragdoll.Base.Info.StartRelativePosition.Position) && Chance.Roll(Config.ReturnChance))
                    returns.Track(ev.Ragdoll);
            }
            catch (Exception e)
            {
                Logger.Error($"OnSpawnedRagdoll: {e}");
            }
        }
    }
}
