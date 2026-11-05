using System;
using System.Collections.Generic;
using System.Linq;
using AugatonLib.Runtime;
using PlayerRoles;
using ScpProximityChat.Enums;
using ScpProximityChat.Handlers;
using UnityEngine;
using UserSettings.ServerSpecific;
using Logger = LabApi.Features.Console.Logger;

namespace ScpProximityChat
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public override string Name => "ScpProximityChat";

        public override string Description => "Permet aux SCP de parler dans le chat de proximite.";

        public override string Author => "Zone-Shilari (base: Bolton)";

        public static Plugin Instance { get; private set; }

        private ProximityChatHandlers proximityChatHandlers;
        private ServerSpecificSettingBase[] registeredSettings;

        public override void Enable()
        {
            Instance = this;


            proximityChatHandlers = new ProximityChatHandlers(this);
            proximityChatHandlers.RegisterEvents();

            if (Config.ActivationType == ActivationType.ServerSpecificSettings)
                RegisterSettings();

            PluginDirectory.Register(this, Capability.Hints);
        }

        public override void Disable()
        {
            proximityChatHandlers?.UnregisterEvents();
            Hints.Clear();

            UnregisterSettings();

            PluginDirectory.Unregister(this);

            proximityChatHandlers = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.ScpRoles is null)
            {
                Logger.Warn("ScpRoles est absent, remis a un ensemble vide.");
                Config.ScpRoles = new HashSet<RoleTypeId>();
            }

            Config.ScpRoles.RemoveWhere(role => !IsEligibleScp(role));

            if (Config.ScpRoles.Count == 0)
                Logger.Warn("Aucun role SCP valide dans ScpRoles, le chat de proximite ne sera jamais activable.");

            if (Config.MaxVolume <= 0f)
            {
                Logger.Warn($"MaxVolume ({Config.MaxVolume}) invalide, remis a 25.");
                Config.MaxVolume = 25f;
            }

            if (Config.Volume <= 0f)
            {
                Logger.Warn($"Volume ({Config.Volume}) invalide, remis a 1.");
                Config.Volume = 1f;
            }

            if (Config.Volume > Config.MaxVolume)
                Logger.Warn($"Volume ({Config.Volume}) depasse MaxVolume ({Config.MaxVolume}), il sera borne.");

            if (Config.MinDistance < 0f)
            {
                Logger.Warn($"MinDistance ({Config.MinDistance}) negatif, remis a 0.");
                Config.MinDistance = 0f;
            }

            if (Config.MaxDistance <= Config.MinDistance)
            {
                Logger.Warn($"MaxDistance ({Config.MaxDistance}) doit etre superieur a MinDistance ({Config.MinDistance}), remis a MinDistance + 10.");
                Config.MaxDistance = Config.MinDistance + 10f;
            }

            if (Config.ToggleCooldownSeconds < 0f)
            {
                Logger.Warn($"ToggleCooldownSeconds ({Config.ToggleCooldownSeconds}) negatif, remis a 0.");
                Config.ToggleCooldownSeconds = 0f;
            }
        }

        // LoadConfigs tourne a l'amorcage du serveur : RoleTypeId.GetTeam() y chargerait les prefabs de roles trop tot.
        private static bool IsEligibleScp(RoleTypeId role)
        {
            switch (role)
            {
                case RoleTypeId.Scp173:
                case RoleTypeId.Scp106:
                case RoleTypeId.Scp049:
                case RoleTypeId.Scp096:
                case RoleTypeId.Scp0492:
                case RoleTypeId.Scp939:
                case RoleTypeId.Scp3114:
                case RoleTypeId.ZombieFlamingo:
                    return true;

                default:
                    return false;
            }
        }

        private void RegisterSettings()
        {
            registeredSettings = new ServerSpecificSettingBase[]
            {
                new SSGroupHeader(Config.KeybindId - 1, Config.SettingHeaderLabel, false, string.Empty),
                new SSKeybindSetting(Config.KeybindId, Config.KeybindLabel, KeyCode.None, false, false, Config.KeybindHint),
            };

            ServerSpecificSettingBase[] defined = ServerSpecificSettingsSync.DefinedSettings ?? Array.Empty<ServerSpecificSettingBase>();

            ServerSpecificSettingsSync.DefinedSettings = defined.Concat(registeredSettings).ToArray();
            ServerSpecificSettingsSync.SendToAll();
        }

        private void UnregisterSettings()
        {
            if (registeredSettings is null)
                return;

            ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings?.Except(registeredSettings).ToArray();
            ServerSpecificSettingsSync.SendToAll();

            registeredSettings = null;
        }
    }
}
