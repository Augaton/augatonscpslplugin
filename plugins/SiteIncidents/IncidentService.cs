using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.Chaos;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.Text;
using AugatonLib.World;
using CustomPlayerEffects;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles.PlayableScps.Scp079;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SiteIncidents
{
    public sealed class IncidentService
    {
        private const float MinIntervalSeconds = 30f;
        private const float EventRetrySeconds = 20f;
        private const float GasTickSeconds = 1f;
        private const float GasEffectSeconds = 2f;
        private const float EvacuationSeconds = 60f;

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
            IncidentType.GasLeak,
            IncidentType.TeslaSurge,
            IncidentType.CameraOutage,
            IncidentType.ElevatorOutage,
        };

        private readonly Plugin plugin;
        private readonly List<CoroutineHandle> timers = new List<CoroutineHandle>(4);
        private readonly List<Door> lockedDoors = new List<Door>(16);

        private CoroutineHandle loop;
        private bool running;
        private bool postponed;
        private bool elevatorsLocked;

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
            UnlockElevators();
        }

        public string CheckCassieWords()
        {
            Translation text = plugin.Translation;
            string unknown = CassieLines.FindUnknownWord(new[]
            {
                text.BlackoutCassie,
                text.AlertCassie,
                text.LockdownCassie,
                text.GasLeakCassie,
                text.TeslaSurgeCassie,
                text.CameraOutageCassie,
                text.ElevatorOutageCassie,
            });

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

                case IncidentType.GasLeak:
                    if (!StartGasLeak(zone))
                        return false;

                    Announce(plugin.Translation.GasLeakCassie, plugin.Translation.GasLeakSubtitles, zone);
                    break;

                case IncidentType.TeslaSurge:
                    if (Tesla.List.Count == 0)
                        return false;

                    zone = FacilityZone.HeavyContainment;
                    timers.Add(Timing.RunCoroutine(SurgeTeslas()));
                    Announce(plugin.Translation.TeslaSurgeCassie, plugin.Translation.TeslaSurgeSubtitles, zone);
                    break;

                case IncidentType.CameraOutage:
                    if (!CutCameras())
                        return false;

                    Announce(plugin.Translation.CameraOutageCassie, plugin.Translation.CameraOutageSubtitles, zone);
                    break;

                case IncidentType.ElevatorOutage:
                    if (elevatorsLocked || !ElevatorsCanStop())
                        return false;

                    LockElevators();
                    Announce(plugin.Translation.ElevatorOutageCassie, plugin.Translation.ElevatorOutageSubtitles, zone);
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

                yield return Timing.WaitForSeconds(postponed ? EventRetrySeconds : NextDelay());
            }
        }

        private void Tick()
        {
            postponed = false;

            if (!Round.IsRoundInProgress || Player.Count < Config.MinPlayers)
                return;

            if (Config.PauseDuringGamemodes && WorldEvents.IsPaused)
                return;

            postponed = !EventSpacing.TryClaim(Time.time, Config.MinEventGapSeconds);

            if (postponed)
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
                case IncidentType.Blackout: return IsSiteDark() ? 0 : Config.BlackoutWeight;
                case IncidentType.Alert: return Config.AlertWeight;
                case IncidentType.Lockdown: return Config.LockdownWeight;
                case IncidentType.GasLeak: return Config.GasLeakWeight;
                case IncidentType.TeslaSurge: return Config.TeslaSurgeWeight;
                case IncidentType.CameraOutage: return HasScp079() ? Config.CameraOutageWeight : 0;
                case IncidentType.ElevatorOutage: return ElevatorsCanStop() ? Config.ElevatorOutageWeight : 0;
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
        // Bloquer les ascenseurs pendant l'ogive ou juste avant la decontamination condamnerait ceux qui fuient.
        private bool ElevatorsCanStop()
        {
            return !Warhead.IsDetonationInProgress && !SiteDanger.LightZoneClosesWithin(Config.ElevatorOutageDuration + EvacuationSeconds);
        }

        private void LockElevators()
        {
            SetElevatorLock(true);
            elevatorsLocked = true;
            timers.Add(Timing.CallDelayed(Config.ElevatorOutageDuration, UnlockElevators));
        }

        public void UnlockElevators()
        {
            if (elevatorsLocked)
                SetElevatorLock(false);

            elevatorsLocked = false;
        }

        // Le panneau refuse tout verrou, et NoPower reste distinct du verrou que le staff pose et leve lui-meme.
        private static void SetElevatorLock(bool locked)
        {
            foreach (Elevator elevator in Elevator.List)
                elevator.Base.ServerLockAllDoors(DoorLockReason.NoPower, locked);
        }

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

        // La fuite vise une salle occupee quand il y en a une : dans une salle vide, personne ne la verrait.
        private bool StartGasLeak(FacilityZone zone)
        {
            List<Room> candidates = new List<Room>(16);

            foreach (Player player in Player.ReadyList)
            {
                if (player.IsHuman && !player.IsTutorial && player.Zone == zone && player.Room is not null)
                    candidates.Add(player.Room);
            }

            if (candidates.Count == 0)
                candidates.AddRange(Room.Get(zone));

            Room room = Chance.Pick(candidates);

            if (room is null)
                return false;

            if (!ColorUtility.TryParseHtmlString(Config.GasLeakColor, out Color color))
                color = Color.green;

            LightArbiter.TintRoom(Plugin.Owner, room, color);
            timers.Add(Timing.RunCoroutine(LeakGas(room)));
            return true;
        }

        private IEnumerator<float> LeakGas(Room room)
        {
            float end = Time.time + Config.GasLeakDuration;

            while (Time.time < end)
            {
                PoisonOccupants(room);
                yield return Timing.WaitForSeconds(GasTickSeconds);
            }

            LightArbiter.ReleaseRoomTint(Plugin.Owner, room);
        }

        private static void PoisonOccupants(Room room)
        {
            try
            {
                foreach (Player player in Player.ReadyList)
                {
                    if (player.IsHuman && !player.IsTutorial && player.Room == room)
                        player.EnableEffect<Poisoned>(1, GasEffectSeconds);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"PoisonOccupants: {e}");
            }
        }

        private IEnumerator<float> SurgeTeslas()
        {
            float end = Time.time + Config.TeslaSurgeDuration;

            while (Time.time < end)
            {
                TriggerTeslas();
                yield return Timing.WaitForSeconds(Config.TeslaSurgeInterval);
            }
        }

        private static void TriggerTeslas()
        {
            try
            {
                foreach (Tesla tesla in Tesla.List)
                    tesla.Trigger();
            }
            catch (Exception e)
            {
                Logger.Error($"TriggerTeslas: {e}");
            }
        }

        private bool CutCameras()
        {
            bool found = false;

            foreach (Player player in Player.ReadyList)
            {
                if (player.RoleBase is Scp079Role role && role.SubroutineModule.TryGetSubroutine(out Scp079LostSignalHandler handler))
                {
                    handler.ServerLoseSignal(Config.CameraOutageDuration);
                    found = true;
                }
            }

            return found;
        }

        private static bool IsSiteDark()
        {
            foreach (FacilityZone zone in Zones)
            {
                if (!LightArbiter.IsBlackedOut(zone))
                    return false;
            }

            return true;
        }

        private static bool HasScp079()
        {
            foreach (Player player in Player.ReadyList)
            {
                if (player.RoleBase is Scp079Role)
                    return true;
            }

            return false;
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
