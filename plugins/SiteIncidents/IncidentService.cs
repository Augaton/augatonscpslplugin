using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.Chaos;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.Text;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SiteIncidents
{
    public sealed class IncidentService
    {
        private const float MinIntervalSeconds = 30f;

        private static readonly FacilityZone[] Zones =
        {
            FacilityZone.LightContainment,
            FacilityZone.HeavyContainment,
            FacilityZone.Entrance,
        };

        private static readonly IncidentType[] Types =
        {
            IncidentType.Blackout,
            IncidentType.Alert,
            IncidentType.Lockdown,
        };

        private readonly Plugin plugin;
        private readonly List<CoroutineHandle> timers = new List<CoroutineHandle>(4);
        private readonly List<Door> lockedDoors = new List<Door>(16);

        private CoroutineHandle loop;
        private bool running;

        public IncidentService(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        public void Start()
        {
            Stop();

            running = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public void Stop()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;

            foreach (CoroutineHandle timer in timers)
                Timing.KillCoroutines(timer);

            timers.Clear();
            LightArbiter.ReleaseOwner(Plugin.Owner);
            UnlockDoors();
        }

        public string CheckCassieWords()
        {
            Translation text = plugin.Translation;
            string unknown = CassieLines.FindUnknownWord(new[] { text.BlackoutCassie, text.AlertCassie, text.LockdownCassie });

            return unknown is null ? null : $"mot inconnu : {unknown}";
        }

        public bool Trigger(IncidentType type)
        {
            if (!TryPickZone(out FacilityZone zone))
                return false;

            switch (type)
            {
                case IncidentType.Blackout:
                    LightArbiter.Blackout(Plugin.Owner, Config.BlackoutDuration, new[] { zone });
                    Announce(plugin.Translation.BlackoutCassie, plugin.Translation.BlackoutSubtitles, zone);
                    break;

                case IncidentType.Alert:
                    StartAlert(zone);
                    Announce(plugin.Translation.AlertCassie, plugin.Translation.AlertSubtitles, zone);
                    break;

                case IncidentType.Lockdown:
                    StartLockdown(zone);
                    Announce(plugin.Translation.LockdownCassie, plugin.Translation.LockdownSubtitles, zone);
                    break;

                default:
                    return false;
            }

            PluginBus.Publish(BusTopics.SiteIncident, Plugin.Owner, type.ToString());
            Logger.Debug($"Incident {type} en {zone}.", Config.Debug);
            return true;
        }

        private IEnumerator<float> Loop()
        {
            yield return Timing.WaitForSeconds(Mathf.Max(0f, Config.FirstIncidentSeconds));

            while (running)
            {
                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"IncidentService.Tick: {e}");
                }

                yield return Timing.WaitForSeconds(NextDelay());
            }
        }

        private void Tick()
        {
            if (!Round.IsRoundInProgress || Player.Count < Config.MinPlayers)
                return;

            if (Config.PauseDuringGamemodes && WorldEvents.IsPaused)
                return;

            IncidentType? type = PickType();

            if (type.HasValue)
                Trigger(type.Value);
        }

        private float NextDelay()
        {
            float min = Mathf.Max(MinIntervalSeconds, Config.MinIntervalSeconds);
            float delay = UnityEngine.Random.Range(min, Mathf.Max(min, Config.MaxIntervalSeconds));

            return Escalation.Shrink(delay, Config.IntervalPerChaosLevel, ChaosState.BonusLevel, MinIntervalSeconds);
        }

        private IncidentType? PickType()
        {
            int total = 0;

            foreach (IncidentType type in Types)
                total += Math.Max(0, WeightOf(type));

            if (total == 0)
                return null;

            return Chance.PickWeighted(Types, WeightOf);
        }

        private int WeightOf(IncidentType type)
        {
            switch (type)
            {
                case IncidentType.Blackout: return Config.BlackoutWeight;
                case IncidentType.Alert: return Config.AlertWeight;
                case IncidentType.Lockdown: return Config.LockdownWeight;
                default: return 0;
            }
        }

        private static bool TryPickZone(out FacilityZone zone)
        {
            List<FacilityZone> available = new List<FacilityZone>(Zones.Length);

            foreach (FacilityZone candidate in Zones)
            {
                bool lost = Warhead.IsDetonated
                    || (candidate == FacilityZone.LightContainment && Decontamination.IsDecontaminating);

                if (!lost)
                    available.Add(candidate);
            }

            zone = Chance.Pick(available);
            return available.Count > 0;
        }

        private void StartAlert(FacilityZone zone)
        {
            if (!ColorUtility.TryParseHtmlString(Config.AlertColor, out Color color))
                color = Color.red;

            foreach (Room room in Room.Get(zone))
                LightArbiter.TintRoom(Plugin.Owner, room, color);

            timers.Add(Timing.CallDelayed(Config.AlertDuration, () => LightArbiter.ReleaseRoomTints(Plugin.Owner)));
        }

        // Isolation est un drapeau distinct d'AdminCommand : lever ce verrou ne touche pas aux portes
        // bloquees par le staff, et les portes deja verrouillees sont de toute facon ignorees.
        private void StartLockdown(FacilityZone zone)
        {
            List<Door> candidates = new List<Door>(64);

            foreach (Door door in Door.List)
            {
                if (door.Zone == zone && !door.IsLocked)
                    candidates.Add(door);
            }

            Chance.Shuffle(candidates);

            int count = Mathf.Min(Config.LockdownDoors, candidates.Count);

            for (int i = 0; i < count; i++)
            {
                candidates[i].Lock(DoorLockReason.Isolation, true);
                lockedDoors.Add(candidates[i]);
            }

            timers.Add(Timing.CallDelayed(Config.LockdownDuration, UnlockDoors));
        }

        private void UnlockDoors()
        {
            foreach (Door door in lockedDoors)
            {
                if (door.Base != null)
                    door.Lock(DoorLockReason.Isolation, false);
            }

            lockedDoors.Clear();
        }

        private void Announce(string cassie, string subtitles, FacilityZone zone)
        {
            if (string.IsNullOrEmpty(cassie))
                return;

            Announcer.Message(cassie, subtitles.Replace("%ZONE%", ZoneName(zone)), false, 0f, 0f);
        }

        private string ZoneName(FacilityZone zone)
        {
            switch (zone)
            {
                case FacilityZone.LightContainment: return plugin.Translation.LightContainmentName;
                case FacilityZone.HeavyContainment: return plugin.Translation.HeavyContainmentName;
                default: return plugin.Translation.EntranceName;
            }
        }
    }
}
