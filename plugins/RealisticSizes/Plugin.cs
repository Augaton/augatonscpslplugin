using System;
using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;
using RealisticSizes.API;
using RealisticSizes.Handlers;

namespace RealisticSizes
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public override string Name => "RealisticSizes";

        public override string Description => "Donne a chaque joueur une taille legerement differente a l'apparition.";

        public override string Author => "Zone-Shilari (base: JesusQC)";

        public static Plugin Instance { get; private set; }

        private PlayerHandlers playerHandlers;

        public override void Enable()
        {
            Instance = this;

            playerHandlers = new PlayerHandlers(this);

            // PlayerEvents.Spawned ne part que pour les roles FPC dotes d'un point d'apparition, avant la
            // fin de l'initialisation du role : ChangedRole couvre aussi le passage en spectateur.
            PlayerEvents.ChangedRole += playerHandlers.OnChangedRole;
            PlayerEvents.Left += playerHandlers.OnLeft;

            PluginBus.Subscribe(Name, BusTopics.PlayerReplaced, playerHandlers.OnPlayerReplaced);
            PluginBus.Subscribe(Name, BusTopics.GamemodeStarted, _ => playerHandlers.GamemodeActive = true);
            PluginBus.Subscribe(Name, BusTopics.GamemodeEnded, _ => playerHandlers.GamemodeActive = false);

            ServerEvents.RoundStarted += playerHandlers.OnRoundStarted;
            ServerEvents.RoundEnded += playerHandlers.OnRoundEnded;
            ServerEvents.RoundRestarted += playerHandlers.OnRoundRestarted;

            PluginDirectory.Register(this, Capability.Scale, Capability.Bus);
        }

        public override void Disable()
        {
            if (playerHandlers is not null)
            {
                PlayerEvents.ChangedRole -= playerHandlers.OnChangedRole;
                PlayerEvents.Left -= playerHandlers.OnLeft;
                PluginBus.UnsubscribeOwner(Name);

                ServerEvents.RoundStarted -= playerHandlers.OnRoundStarted;
                ServerEvents.RoundEnded -= playerHandlers.OnRoundEnded;
                ServerEvents.RoundRestarted -= playerHandlers.OnRoundRestarted;

                playerHandlers.Reset();
            }

            PluginDirectory.Unregister(this);

            playerHandlers = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.ApplyDelay < 0f)
            {
                Logger.Warn($"ApplyDelay ({Config.ApplyDelay}) negatif, remis a 0.4.");
                Config.ApplyDelay = 0.4f;
            }

            if (Config.SpreadStep < 0f)
            {
                Logger.Warn($"SpreadStep ({Config.SpreadStep}) negatif, remis a 0.");
                Config.SpreadStep = 0f;
            }

            if (Config.SpreadMinPlayers < 1)
            {
                Logger.Warn($"SpreadMinPlayers ({Config.SpreadMinPlayers}) inferieur a 1, remis a 1.");
                Config.SpreadMinPlayers = 1;
            }

            Config.SpeedPercentPerTenth = UnityEngine.Mathf.Max(0f, Config.SpeedPercentPerTenth);
            Config.HealthPercentPerTenth = UnityEngine.Mathf.Max(0f, Config.HealthPercentPerTenth);
            Config.MaxSpeedPercent = UnityEngine.Mathf.Clamp(Config.MaxSpeedPercent, 0f, 50f);
            Config.MaxHealthPercent = UnityEngine.Mathf.Clamp(Config.MaxHealthPercent, 0f, 50f);

            if (Config.IgnoredRoles is null)
            {
                Logger.Warn("IgnoredRoles est absent, remis a une liste vide.");
                Config.IgnoredRoles = new List<RoleTypeId>();
            }

            if (Config.ManualRanges is null)
            {
                Logger.Warn("ManualRanges est absent, remis a une table vide.");
                Config.ManualRanges = new Dictionary<RoleTypeId, SizeRange>();
            }

            Check("RoleplayRange", Config.RoleplayRange);
            Check("FunRange", Config.FunRange);

            foreach (KeyValuePair<RoleTypeId, SizeRange> entry in Config.ManualRanges)
                Check($"ManualRanges[{entry.Key}]", entry.Value);
        }

        private static void Check(string name, SizeRange range)
        {
            if (range is null)
                return;

            if (range.MinHeight <= 0f || range.MinWidth <= 0f)
            {
                Logger.Warn($"{name} contient une valeur minimale nulle ou negative, remise a 0.5.");
                range.MinHeight = Math.Max(0.5f, range.MinHeight);
                range.MinWidth = Math.Max(0.5f, range.MinWidth);
            }

            if (range.MaxHeight < range.MinHeight)
            {
                Logger.Warn($"{name} : MaxHeight inferieur a MinHeight, aligne sur MinHeight.");
                range.MaxHeight = range.MinHeight;
            }

            if (range.MaxWidth < range.MinWidth)
            {
                Logger.Warn($"{name} : MaxWidth inferieur a MinWidth, aligne sur MinWidth.");
                range.MaxWidth = range.MinWidth;
            }

            if (range.MaxHeight > 3f || range.MaxWidth > 3f)
                Logger.Warn($"{name} depasse un facteur 3. Les tailles extremes cassent les collisions et la visee.");
        }
    }
}
