using System;
using AugatonLib.Random;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using SCP1162.API;

namespace SCP1162.Handlers
{
    public sealed class ServerHandlers
    {
        private readonly Plugin plugin;

        public ServerHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnWaitingForPlayers()
        {
            try
            {
                plugin.Service.Reset();

                if (!Chance.Roll(plugin.Config.SpawnChance))
                {
                    Logger.Debug("SCP-1162 n'apparait pas ce round.", plugin.Config.Debug);
                    return;
                }

                int spawned = plugin.Service.Spawn();

                Logger.Debug($"SCP-1162 : {spawned} emplacement(s) actif(s).", plugin.Config.Debug);
            }
            catch (Exception e)
            {
                Logger.Error($"OnWaitingForPlayers: {e}");
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            try
            {
                plugin.Service.ClearCooldowns();
                plugin.Hints.Clear();
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundEnded: {e}");
            }
        }

        public void OnRoundRestarted()
        {
            try
            {
                plugin.Service.Reset();
                plugin.Hints.Clear();
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundRestarted: {e}");
            }
        }
    }
}
