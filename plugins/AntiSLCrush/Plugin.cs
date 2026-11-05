// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using AntiSLCrush.Patches;
using AugatonLib.Runtime;
using HarmonyLib;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins.Enums;

namespace AntiSLCrush
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public const string PluginName = "AntiSLCrush";

        private static readonly Harmony Patcher = new Harmony(PluginName);

        public override string Name => PluginName;

        public override string Description => "Protege le serveur contre certains crashs du jeu.";

        public override string Author => "ui_2506 (modifie par Zone-Shilari)";

        public override LoadPriority Priority => LoadPriority.Lowest;

        public static Plugin Instance { get; private set; }

        public override void Enable()
        {
            Instance = this;

            Patcher.PatchAll();
            ServerEvents.RoundRestarted += OnTransportDataPatch.Clear;

            PluginDirectory.Register(this);
        }

        public override void Disable()
        {
            Patcher.UnpatchAll(PluginName);
            ServerEvents.RoundRestarted -= OnTransportDataPatch.Clear;

            PluginDirectory.Unregister(this);
            Instance = null;
        }

        internal static void Report(string message) => Logger.Debug(message, Instance?.Config.Debug ?? false);
    }
}
