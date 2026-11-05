using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Voice;
using ScpProximityChat.Enums;
using UnityEngine;
using UserSettings.ServerSpecific;
using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Networking;
using Logger = LabApi.Features.Console.Logger;

namespace ScpProximityChat.Handlers
{
    public sealed class ProximityChatHandlers
    {
        private readonly Plugin plugin;
        private readonly ActivationType activationType;
        private readonly Dictionary<string, SpeakerToy> toggledPlayers = new Dictionary<string, SpeakerToy>();
        private readonly Dictionary<string, float> lastToggle = new Dictionary<string, float>();

        public ProximityChatHandlers(Plugin plugin)
        {
            this.plugin = plugin;
            activationType = plugin.Config.ActivationType;
        }

        private Config config => plugin.Config;

        public void RegisterEvents()
        {
            ServerEvents.RoundRestarted += OnRoundRestarted;
            PlayerEvents.SendingVoiceMessage += OnSendingVoiceMessage;
            PlayerEvents.ChangingRole += OnChangingRole;
            PlayerEvents.Left += OnLeft;

            switch (activationType)
            {
                case ActivationType.ServerSpecificSettings:
                    ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
                    PlayerEvents.Joined += OnJoined;
                    break;

                case ActivationType.NoClip:
                    PlayerEvents.TogglingNoclip += OnTogglingNoclip;
                    break;
            }
        }

        public void UnregisterEvents()
        {
            ServerEvents.RoundRestarted -= OnRoundRestarted;
            PlayerEvents.SendingVoiceMessage -= OnSendingVoiceMessage;
            PlayerEvents.ChangingRole -= OnChangingRole;
            PlayerEvents.Left -= OnLeft;

            switch (activationType)
            {
                case ActivationType.ServerSpecificSettings:
                    ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
                    PlayerEvents.Joined -= OnJoined;
                    break;

                case ActivationType.NoClip:
                    PlayerEvents.TogglingNoclip -= OnTogglingNoclip;
                    break;
            }

            ClearAll();
        }

        public void ClearAll()
        {
            foreach (KeyValuePair<string, SpeakerToy> entry in toggledPlayers)
                DestroySpeaker(entry.Value);

            toggledPlayers.Clear();
            lastToggle.Clear();
            OpusHandler.ClearAll();
            plugin.Hints.Clear();
        }

        private void OnRoundRestarted()
        {
            try
            {
                ClearAll();
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundRestarted: {e}");
            }
        }

        private void OnSendingVoiceMessage(PlayerSendingVoiceMessageEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || !ev.IsAllowed || ev.Message.Channel != VoiceChatChannel.ScpChat)
                    return;

                if (!toggledPlayers.TryGetValue(ev.Player.UserId, out SpeakerToy speaker) || speaker.IsDestroyed)
                    return;

                OpusHandler opusHandler = OpusHandler.Get(ev.Player);

                if (opusHandler is null)
                    return;

                Config settings = config;
                float gain = Mathf.Clamp(settings.Volume, 0f, settings.MaxVolume);
                float[] samples = opusHandler.SampleBuffer;

                if (opusHandler.Decoder.Decode(ev.Message.Data, ev.Message.DataLength, samples) != samples.Length)
                    return;

                for (int i = 0; i < samples.Length; i++)
                    samples[i] = Mathf.Clamp(samples[i] * gain, -1f, 1f);

                byte[] encoded = opusHandler.EncodedBuffer;
                int dataLength = opusHandler.Encoder.Encode(samples, encoded);

                AudioMessage audioMessage = new AudioMessage(speaker.ControllerId, encoded, dataLength);

                // Chemin chaud (un paquet par trame voix) : Player.ReadyList allouerait un iterateur LINQ a chaque paquet.
                foreach (Player target in Player.Dictionary.Values)
                {
                    if (!target.IsPlayer || !target.IsReady)
                        continue;

                    if (settings.UseDefaultScpChat && target.IsSCP)
                        continue;

                    if (target.RoleBase is not IVoiceRole voiceRole)
                        continue;

                    if (voiceRole.VoiceModule.ValidateReceive(ev.Player.ReferenceHub, VoiceChatChannel.Proximity) == VoiceChatChannel.None)
                        continue;

                    target.ReferenceHub.connectionToClient.Send(audioMessage);
                }

                if (!settings.UseDefaultScpChat)
                    ev.IsAllowed = false;
            }
            catch (OpusException e)
            {
                Logger.Debug($"OnSendingVoiceMessage: paquet voix invalide ignore ({e.Message}).", config.Debug);
            }
            catch (Exception e)
            {
                Logger.Error($"OnSendingVoiceMessage: {e}");
            }
        }

        private void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || !ev.Player.IsReady)
                    return;

                if (config.DisableOnRoleChange && toggledPlayers.ContainsKey(ev.Player.UserId))
                    Disable(ev.Player, false);

                if (!config.ScpRoles.Contains(ev.NewRole))
                    return;

                Show(ev.Player, config.ProximityChatRole);
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangingRole: {e}");
            }
        }

        private void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || string.IsNullOrEmpty(ev.Player.UserId))
                    return;

                string userId = ev.Player.UserId;

                if (toggledPlayers.TryGetValue(userId, out SpeakerToy speaker))
                {
                    DestroySpeaker(speaker);
                    toggledPlayers.Remove(userId);
                }

                lastToggle.Remove(userId);
                OpusHandler.Remove(userId);
                plugin.Hints.Remove(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        private void OnJoined(PlayerJoinedEventArgs ev)
        {
            try
            {
                if (ev?.Player?.ReferenceHub is null)
                    return;

                ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
            }
            catch (Exception e)
            {
                Logger.Error($"OnJoined: {e}");
            }
        }

        private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            try
            {
                if (settingBase is not SSKeybindSetting keybind
                    || keybind.SettingId != config.KeybindId
                    || !keybind.SyncIsPressed)
                {
                    return;
                }

                Player player = Player.Get(hub);

                if (player is null || !config.ScpRoles.Contains(player.Role))
                    return;

                Toggle(player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnSettingValueReceived: {e}");
            }
        }

        private void OnTogglingNoclip(PlayerTogglingNoclipEventArgs ev)
        {
            try
            {
                if (ev?.Player is null)
                    return;

                if (FpcNoclip.IsPermitted(ev.Player.ReferenceHub) || !config.ScpRoles.Contains(ev.Player.Role))
                    return;

                ev.IsAllowed = false;
                Toggle(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnTogglingNoclip: {e}");
            }
        }

        private void Toggle(Player player)
        {
            if (player is null || string.IsNullOrEmpty(player.UserId))
                return;

            if (IsOnCooldown(player.UserId))
                return;

            lastToggle[player.UserId] = Time.realtimeSinceStartup;

            if (toggledPlayers.ContainsKey(player.UserId))
                Disable(player, true);
            else
                Enable(player);
        }

        private bool IsOnCooldown(string userId)
        {
            if (config.ToggleCooldownSeconds <= 0f)
                return false;

            return lastToggle.TryGetValue(userId, out float last)
                && Time.realtimeSinceStartup - last < config.ToggleCooldownSeconds;
        }

        private void Enable(Player player)
        {
            SpeakerToy speaker = SpeakerToy.Create(player.ReferenceHub.transform);

            speaker.ControllerId = (byte)player.PlayerId;
            speaker.MinDistance = config.MinDistance;
            speaker.MaxDistance = config.MaxDistance;

            toggledPlayers[player.UserId] = speaker;

            Show(player, config.ProximityChatEnabled);
        }

        private void Disable(Player player, bool notify)
        {
            if (!toggledPlayers.TryGetValue(player.UserId, out SpeakerToy speaker))
                return;

            DestroySpeaker(speaker);
            toggledPlayers.Remove(player.UserId);
            OpusHandler.Remove(player);

            if (notify)
                Show(player, config.ProximityChatDisabled);
        }

        private static void DestroySpeaker(SpeakerToy speaker)
        {
            if (speaker is null || speaker.IsDestroyed)
                return;

            speaker.Destroy();
        }

        private static void Show(Player player, Message message)
        {
            if (player is null || message is null || !message.Show || string.IsNullOrEmpty(message.Content))
                return;

            switch (message.Type)
            {
                case MessageType.Broadcast:
                    player.SendBroadcast(message.Content, message.Duration);
                    break;

                case MessageType.Hint:
                    Plugin.Instance.Hints.Show(player, message.Content, message.Duration);
                    break;
            }
        }
    }
}
