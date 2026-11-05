// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using AntiSLCrush.Patches;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins.Enums;

namespace AntiSLCrush
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public const string PluginName = "AntiSLCrush";

        public override string Name => PluginName;

        public override string Description => "Protege le serveur contre certains crashs du jeu.";

        public override string Author => "ui_2506 (modifie par Zone-Shilari)";

        public override LoadPriority Priority => LoadPriority.Lowest;

        public static Plugin Instance { get; private set; }

        public override void Enable()
        {
            Instance = this;

            Protections.Apply(Config);
            ServerEvents.RoundRestarted += OnTransportDataPatch.Clear;

            if (Config.CleanObservers)
                ServerEvents.WaitingForPlayers += ObserverSweep.Start;

            SelfCheck.Register(Name, "protections en place", () => Protections.Failures.Count == 0 ? null : string.Join(", ", Protections.Failures));

            PluginDirectory.Register(this);
        }

        public override void Disable()
        {
            Protections.Remove();
            ServerEvents.RoundRestarted -= OnTransportDataPatch.Clear;
            ServerEvents.WaitingForPlayers -= ObserverSweep.Start;
            ObserverSweep.Stop();

            PluginDirectory.Unregister(this);
            Instance = null;
        }

        internal static void Report(string message) => Logger.Debug(message, Instance?.Config.Debug ?? false);
    }
}
