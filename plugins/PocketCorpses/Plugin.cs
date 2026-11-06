using System;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.World;
using Hazards;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace PocketCorpses
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        private PocketReturns returns;

        public static Plugin Instance { get; private set; }

        internal PocketReturns Returns => returns;

        public override string Name => "PocketCorpses";

        public override string Description => "Les corps laisses dans la dimension de poche retombent dans le complexe.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            returns = new PocketReturns(this);

            PlayerEvents.SpawnedRagdoll += OnSpawnedRagdoll;
            ServerEvents.RoundStarted += returns.Start;
            ServerEvents.RoundRestarted += returns.Stop;

            SelfCheck.Register(Name, "prefab de gouffre", () => NetworkPrefabs.Find<SinkholeEnvironmentalHazard>() is null ? "introuvable" : null);
            SelfCheck.Register(Name, "dimension de poche", () => PocketDimension.Instance is null ? "introuvable" : null);

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
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.ReturnChance = Math.Min(100f, AtLeast("return_chance", Config.ReturnChance, 0f));
            Config.WarningSeconds = AtLeast("warning_seconds", Config.WarningSeconds, 0f);
            Config.MinDelaySeconds = AtLeast("min_delay_seconds", Config.MinDelaySeconds, Config.WarningSeconds);
            Config.MaxDelaySeconds = AtLeast("max_delay_seconds", Config.MaxDelaySeconds, Config.MinDelaySeconds);
            Config.SinkholeSeconds = AtLeast("sinkhole_seconds", Config.SinkholeSeconds, 0f);
        }

        private void OnSpawnedRagdoll(PlayerSpawnedRagdollEventArgs ev)
        {
            try
            {
                if (ev?.Ragdoll is null)
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
