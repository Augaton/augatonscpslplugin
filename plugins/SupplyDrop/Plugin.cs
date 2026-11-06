using System;
using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SupplyDrop.API;
using SupplyDrop.Handlers;

namespace SupplyDrop
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public override string Name => "SupplyDrop";

        public override string Description => "Largages de ravitaillement periodiques pour le MTF et l'Insurrection du Chaos.";

        public override string Author => "Zone-Shilari (base: Wafel, KadeDev, JesusQC)";

        public const string BusOwner = "SupplyDrop";

        public static Plugin Instance { get; private set; }

        public DropService Service { get; private set; }

        private ServerHandlers serverHandlers;

        public override void Enable()
        {
            Instance = this;

            Service = new DropService(this);
            serverHandlers = new ServerHandlers(Service);

            ServerEvents.RoundStarted += serverHandlers.OnRoundStarted;
            ServerEvents.RoundEnded += serverHandlers.OnRoundEnded;
            ServerEvents.RoundRestarted += serverHandlers.OnRoundRestarted;
            ServerEvents.WaitingForPlayers += serverHandlers.OnWaitingForPlayers;
            ServerEvents.GeneratorActivated += serverHandlers.OnGeneratorActivated;
            ServerEvents.LczDecontaminationStarted += serverHandlers.OnDecontaminationStarted;
            WarheadEvents.Detonated += serverHandlers.OnWarheadDetonated;

            PluginBus.Subscribe(BusOwner, BusTopics.TeamWiped, OnTeamWiped);

            SelfCheck.Register(Name, "point de largage en surface", () => Door.Get(DoorName.SurfaceNuke) is null ? "introuvable" : null);

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            if (serverHandlers is not null)
            {
                ServerEvents.RoundStarted -= serverHandlers.OnRoundStarted;
                ServerEvents.RoundEnded -= serverHandlers.OnRoundEnded;
                ServerEvents.RoundRestarted -= serverHandlers.OnRoundRestarted;
                ServerEvents.WaitingForPlayers -= serverHandlers.OnWaitingForPlayers;
                ServerEvents.GeneratorActivated -= serverHandlers.OnGeneratorActivated;
                ServerEvents.LczDecontaminationStarted -= serverHandlers.OnDecontaminationStarted;
                WarheadEvents.Detonated -= serverHandlers.OnWarheadDetonated;
            }

            Service?.Stop();

            PluginBus.UnsubscribeOwner(BusOwner);
            PluginDirectory.Unregister(this);

            serverHandlers = null;
            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.TrapChancePercent = AtLeast("trap_chance_percent", Config.TrapChancePercent, 0f);
            Config.TrapRange = AtLeast("trap_range", Config.TrapRange, 1f);
            Config.TrapFuseSeconds = AtLeast("trap_fuse_seconds", Config.TrapFuseSeconds, 0.5f);

            if (Config.MinPlayers < 0)
            {
                Logger.Warn($"MinPlayers ({Config.MinPlayers}) negatif, remis a 0.");
                Config.MinPlayers = 0;
            }

            if (Config.RetrySeconds < 5f)
            {
                Logger.Warn($"RetrySeconds ({Config.RetrySeconds}) trop bas, remis a 60.");
                Config.RetrySeconds = 60f;
            }

            if (Config.SchedulerTick < 0.5f)
            {
                Logger.Warn($"SchedulerTick ({Config.SchedulerTick}) trop bas, remis a 1.");
                Config.SchedulerTick = 1f;
            }

            if (Config.MaxItemsPerDrop < 1)
            {
                Logger.Warn($"MaxItemsPerDrop ({Config.MaxItemsPerDrop}) invalide, remis a 60.");
                Config.MaxItemsPerDrop = 60;
            }

            if (Config.SpawnHeightOffset < 0f || Config.SpawnHeightOffset > 10f)
            {
                Logger.Warn($"SpawnHeightOffset ({Config.SpawnHeightOffset}) hors plage, remis a 0.5.");
                Config.SpawnHeightOffset = 0.5f;
            }

            if (Config.Profiles is null)
                Config.Profiles = new List<DropProfile>();

            if (Config.Profiles.Count == 0)
            {
                Logger.Warn("Aucun profil de largage configure, le plugin ne fera rien.");
                return;
            }

            HashSet<string> keys = new HashSet<string>();

            foreach (DropProfile profile in Config.Profiles)
            {
                if (profile is null)
                    continue;

                profile.PillDraws = UnityEngine.Mathf.Clamp(profile.PillDraws, 0, 16);
                profile.PillChance = UnityEngine.Mathf.Clamp(profile.PillChance, 0, 100);

                if (string.IsNullOrEmpty(profile.Key))
                {
                    Logger.Warn("Un profil de largage n'a pas de cle, il sera ignore.");
                    profile.IsEnabled = false;
                    continue;
                }

                if (!keys.Add(profile.Key))
                {
                    Logger.Warn($"Cle de profil \"{profile.Key}\" en double, la seconde entree est desactivee.");
                    profile.IsEnabled = false;
                    continue;
                }

                if (profile.IntervalSeconds < 30f)
                {
                    Logger.Warn($"IntervalSeconds du profil \"{profile.Key}\" ({profile.IntervalSeconds}) trop bas, remis a 30.");
                    profile.IntervalSeconds = 30f;
                }

                if (profile.FirstDropSeconds < 0f)
                {
                    Logger.Warn($"FirstDropSeconds du profil \"{profile.Key}\" negatif, remis a 0.");
                    profile.FirstDropSeconds = 0f;
                }

                if (profile.AnnouncementLeadSeconds < 0f)
                {
                    Logger.Warn($"AnnouncementLeadSeconds du profil \"{profile.Key}\" negatif, remis a 0.");
                    profile.AnnouncementLeadSeconds = 0f;
                }

                if (profile.ScatterRadius < 0f)
                    profile.ScatterRadius = 0f;
            }
        }

        private void OnTeamWiped(BusMessage message)
        {
            try
            {
                if (message is null || !message.TryGet(out Team team))
                    return;

                if (Config.TeamWipeDrops is null
                    || !Config.TeamWipeDrops.TryGetValue(team, out string key)
                    || string.IsNullOrEmpty(key))
                {
                    return;
                }

                if (!Round.IsRoundInProgress || Service is null || !Service.MarkTeamWiped(team))
                    return;

                DropProfile profile = Service.Find(key);

                if (profile is null)
                {
                    Logger.Warn($"team_wipe_drops pointe vers le profil inconnu \"{key}\" pour l'equipe {team}.");
                    return;
                }

                Logger.Info($"[SupplyDrop] Extinction de {team} : declenchement du largage \"{profile.Key}\".");
                Service.Trigger(profile, true);
            }
            catch (Exception e)
            {
                Logger.Error($"OnTeamWiped: {e}");
            }
        }
    }
}
