using System;
using System.Collections.Generic;
using System.Linq;
using AugatonLib.Bus;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.Spawning;
using Cassie;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms.Attachments;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SupplyDrop.API
{
    public sealed class DropService
    {
        private const float TrapTickSeconds = 0.5f;

        private readonly Plugin plugin;
        private readonly Dictionary<string, DropState> states = new Dictionary<string, DropState>();
        private readonly List<CoroutineHandle> pending = new List<CoroutineHandle>(8);
        private readonly List<LightSourceToy> beacons = new List<LightSourceToy>(4);

        private CoroutineHandle scheduler;
        private bool running;
        private int generatorsActivated;
        private readonly HashSet<Team> wipedTeams = new HashSet<Team>();

        public DropService(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        private bool IsPaused => Config.PauseDuringGamemodes && WorldEvents.IsPaused;

        public bool MarkTeamWiped(Team team) => wipedTeams.Add(team);

        public void OnGeneratorActivated()
        {
            generatorsActivated++;
            FireEventDrops(profile => profile.GeneratorsTrigger == generatorsActivated);
        }

        public void OnDecontaminationStarted() => FireEventDrops(profile => profile.DecontaminationTrigger);

        public void OnWarheadDetonated() => FireEventDrops(profile => profile.WarheadTrigger);

        private void FireEventDrops(Func<DropProfile, bool> isTriggered)
        {
            if (!Round.IsRoundInProgress || IsPaused || Player.Count < Config.MinPlayers)
                return;

            foreach (DropProfile profile in Config.Profiles)
            {
                if (profile is null || !profile.IsEnabled || !isTriggered(profile))
                    continue;

                if (string.IsNullOrEmpty(profile.Key) || !states.TryGetValue(profile.Key, out DropState state))
                    continue;

                if (profile.MaxDropsPerRound > 0 && state.DropsUsed >= profile.MaxDropsPerRound)
                    continue;

                state.DropsUsed++;
                Trigger(profile, true);
            }
        }

        public void Start()
        {
            Stop();

            foreach (DropProfile profile in Config.Profiles)
            {
                if (profile is null || !profile.IsEnabled || string.IsNullOrEmpty(profile.Key))
                    continue;

                states[profile.Key] = new DropState { NextDropAt = profile.FirstDropSeconds };
            }

            if (states.Count == 0)
                return;

            running = true;
            scheduler = Timing.RunCoroutine(Schedule(), Segment.FixedUpdate);
        }

        public void Stop()
        {
            generatorsActivated = 0;
            wipedTeams.Clear();
            if (running)
                Timing.KillCoroutines(scheduler);

            running = false;

            foreach (CoroutineHandle handle in pending)
                Timing.KillCoroutines(handle);

            pending.Clear();

            foreach (LightSourceToy beacon in beacons)
                DestroyBeacon(beacon);

            beacons.Clear();
            states.Clear();
        }

        public DropProfile Find(string key)
        {
            foreach (DropProfile profile in Config.Profiles)
            {
                if (profile is not null && string.Equals(profile.Key, key, StringComparison.OrdinalIgnoreCase))
                    return profile;
            }

            return null;
        }

        public void Trigger(DropProfile profile, bool announce, Vector3? forcedPosition = null)
        {
            if (profile is null)
                return;

            if (!announce)
            {
                Deliver(profile, forcedPosition);
                return;
            }

            Announce(profile);

            if (profile.AnnouncementLeadSeconds <= 0f)
            {
                Deliver(profile, forcedPosition);
                return;
            }

            string key = profile.Key;

            pending.Add(Timing.CallDelayed(profile.AnnouncementLeadSeconds, () => Deliver(Find(key), forcedPosition)));
        }

        private IEnumerator<float> Schedule()
        {
            float tick = Mathf.Max(0.5f, Config.SchedulerTick);

            while (running)
            {
                yield return Timing.WaitForSeconds(tick);

                if (!Round.IsRoundInProgress || IsPaused)
                    continue;

                float elapsed = (float)Round.Duration.TotalSeconds;

                foreach (DropProfile profile in Config.Profiles)
                {
                    if (profile is null || !profile.IsEnabled || string.IsNullOrEmpty(profile.Key))
                        continue;

                    if (!states.TryGetValue(profile.Key, out DropState state))
                        continue;

                    if (elapsed < state.NextDropAt)
                        continue;

                    if (profile.MaxDropsPerRound > 0 && state.DropsUsed >= profile.MaxDropsPerRound)
                        continue;

                    if (Player.Count < Config.MinPlayers)
                    {
                        state.NextDropAt = elapsed + Config.RetrySeconds;
                        continue;
                    }

                    if (!EventSpacing.TryClaim(Time.time, Config.MinEventGapSeconds))
                    {
                        state.NextDropAt = elapsed + Config.RetrySeconds;
                        continue;
                    }

                    state.DropsUsed++;
                    state.NextDropAt = elapsed + profile.AnnouncementLeadSeconds + profile.IntervalSeconds;

                    Trigger(profile, true);
                }
            }
        }

        private static void Announce(DropProfile profile)
        {
            try
            {
                if (!string.IsNullOrEmpty(profile.Broadcast))
                    Server.SendBroadcast(profile.Broadcast, profile.BroadcastDuration);

                if (string.IsNullOrEmpty(profile.CassieAnnouncement))
                    return;

                if (string.IsNullOrEmpty(profile.CassieSubtitles))
                    Announcer.Message(new CassieTtsPayload(profile.CassieAnnouncement, false, false), 0f, 0f);
                else
                    Announcer.Message(profile.CassieAnnouncement, profile.CassieSubtitles, false, 0f, 0f);
            }
            catch (Exception e)
            {
                Logger.Error($"DropService.Announce: {e}");
            }
        }

        private void Deliver(DropProfile profile, Vector3? forcedPosition)
        {
            try
            {
                if (profile is null || !Round.IsRoundInProgress)
                    return;

                if (profile.Items is null || profile.Items.Count == 0)
                {
                    Logger.Warn($"Le profil de largage \"{profile.Key}\" ne contient aucun objet.");
                    return;
                }

                Vector3 anchor = forcedPosition ?? ResolveAnchor(profile);

                if (anchor == Vector3.zero)
                {
                    Logger.Error(
                        $"Largage \"{profile.Key}\" annule : aucune position exploitable. " +
                        $"Le role de repli {profile.FallbackRole} n'expose aucun point d'apparition et aucune " +
                        "coordonnee n'est configuree dans le profil.");
                    return;
                }

                int spawned = PlaceItems(profile, forcedPosition, anchor, out int attempted);

                PlacePills(profile, forcedPosition, anchor);

                PlaceBeacon(profile, forcedPosition ?? ResolveBeaconPosition(profile, anchor));

                PluginBus.Publish(BusTopics.SupplyDropped, Plugin.BusOwner, profile.Key);

                if (Chance.Roll(Config.TrapChancePercent))
                    pending.Add(Timing.RunCoroutine(WatchTrap(anchor)));

                if (Config.LogDrops)
                    Logger.Info($"[SupplyDrop] Largage \"{profile.Key}\" : {spawned}/{attempted} objets places autour de {Describe(anchor)}.");
            }
            catch (Exception e)
            {
                Logger.Error($"DropService.Deliver: {e}");
            }
        }

        private IEnumerator<float> WatchTrap(Vector3 anchor)
        {
            float end = Time.time + Config.TrapLifetimeSeconds;

            while (Time.time < end)
            {
                yield return Timing.WaitForSeconds(TrapTickSeconds);

                if (!IsHumanNear(anchor))
                    continue;

                ItemType grenade = Chance.Roll(50f) ? ItemType.GrenadeHE : ItemType.GrenadeFlash;

                TimedGrenadeProjectile.SpawnActive(anchor + (Vector3.up * 0.5f), grenade, null, Config.TrapFuseSeconds);
                yield break;
            }
        }

        // Chemin chaud : Player.ReadyList allouerait un iterateur LINQ a chaque tick.
        private bool IsHumanNear(Vector3 anchor)
        {
            float rangeSquared = Config.TrapRange * Config.TrapRange;

            foreach (Player player in Player.Dictionary.Values)
            {
                if (player.IsPlayer && player.IsHuman && !player.IsTutorial && (player.Position - anchor).sqrMagnitude <= rangeSquared)
                    return true;
            }

            return false;
        }

        private int PlaceItems(DropProfile profile, Vector3? forcedPosition, Vector3 anchor, out int attempted)
        {
            Vector3 lift = Vector3.up * Config.SpawnHeightOffset;
            int spawned = 0;

            attempted = 0;

            foreach (DropItem entry in profile.Items)
            {
                if (entry is null || entry.Item == ItemType.None || entry.Quantity <= 0)
                    continue;

                Vector3 origin = forcedPosition ?? ResolvePosition(profile, entry.Item, anchor);
                int chance = Mathf.Clamp(entry.Chance, 0, 100);

                for (int i = 0; i < entry.Quantity; i++)
                {
                    if (spawned >= Config.MaxItemsPerDrop)
                    {
                        Logger.Warn($"Largage \"{profile.Key}\" tronque a {Config.MaxItemsPerDrop} objets (max_items_per_drop).");
                        return spawned;
                    }

                    attempted++;

                    if (UnityEngine.Random.Range(0, 100) >= chance)
                        continue;

                    if (SpawnPickup(entry.Item, Scatter(origin, profile.ScatterRadius) + lift))
                        spawned++;
                }
            }

            return spawned;
        }

        private void PlacePills(DropProfile profile, Vector3? forcedPosition, Vector3 anchor)
        {
            Vector3 origin = forcedPosition ?? ResolvePosition(profile, ItemType.SCP500, anchor);

            for (int i = 0; i < profile.PillDraws; i++)
            {
                if (UnityEngine.Random.Range(0, 100) >= profile.PillChance)
                    continue;

                Vector3 position = Scatter(origin, profile.ScatterRadius) + (Vector3.up * Config.SpawnHeightOffset);

                if (PluginBus.Publish(BusTopics.PillSpawnRequested, Plugin.BusOwner, position) == 0)
                    return;
            }
        }

        private static bool SpawnPickup(ItemType type, Vector3 position)
        {
            Pickup pickup = Pickup.Create(type, position, Quaternion.identity, Vector3.one, false);

            if (pickup is null)
                return false;

            if (pickup is FirearmPickup firearm)
                LoadFirearm(firearm);

            pickup.Spawn();
            return true;
        }

        private static void LoadFirearm(FirearmPickup firearm)
        {
            // Un pickup cree de zero a un chargeur vide. OnDistributed le remplit comme une arme de la carte,
            // mais tirerait des accessoires au hasard si aucun code n'etait deja enregistre pour ce numero de serie.
            firearm.AttachmentCode = AttachmentPreview.Get(firearm.Type, 0u, true).GetCurrentAttachmentsCode();
            firearm.Base.OnDistributed();
        }

        private static Vector3 ResolvePosition(DropProfile profile, ItemType type, Vector3 anchor)
        {
            InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase template);
            ItemCategory category = template is null ? ItemCategory.None : template.Category;

            if (category == ItemCategory.Ammo && profile.AmmoPosition != Vector3.zero)
                return profile.AmmoPosition;

            if (category == ItemCategory.Armor && profile.ArmorPosition != Vector3.zero)
                return profile.ArmorPosition;

            if ((IsWeapon(type, template) || IsThrowable(type)) && profile.WeaponPosition != Vector3.zero)
                return profile.WeaponPosition;

            if (IsGear(type, category) && profile.ItemPosition != Vector3.zero)
                return profile.ItemPosition;

            return anchor;
        }

        private static bool IsWeapon(ItemType type, ItemBase template)
        {
            return template is InventorySystem.Items.Firearms.Firearm
                || type is ItemType.MicroHID or ItemType.Jailbird or ItemType.SCP1509;
        }

        private static bool IsThrowable(ItemType type)
        {
            return type is ItemType.GrenadeHE
                or ItemType.GrenadeFlash
                or ItemType.SCP018
                or ItemType.SCP2176
                or ItemType.Snowball
                or ItemType.Coal
                or ItemType.SpecialCoal;
        }

        private static bool IsGear(ItemType type, ItemCategory category)
        {
            return category is ItemCategory.Keycard or ItemCategory.Medical or ItemCategory.SCPItem
                || type is ItemType.Radio or ItemType.Flashlight;
        }

        private static Vector3 ResolveBeaconPosition(DropProfile profile, Vector3 anchor)
            => profile.ItemPosition != Vector3.zero ? profile.ItemPosition : anchor;

        private static Vector3 ResolveAnchor(DropProfile profile)
        {
            if (RoleSpawn.TryGetPosition(profile.FallbackRole, out Vector3 spawn) && spawn != Vector3.zero)
                return spawn;

            Door nuke = Door.Get(DoorName.SurfaceNuke);

            if (nuke is not null)
                return nuke.Position + (Vector3.up * 1.5f) + (nuke.Transform.forward * 3f);

            Room surface = Room.Get(RoomName.Outside).FirstOrDefault();

            return surface is null ? Vector3.zero : surface.Position;
        }

        private static string Describe(Vector3 position)
            => $"{position.x:0.0} / {position.y:0.0} / {position.z:0.0}";

        private static Vector3 Scatter(Vector3 origin, float radius)
        {
            if (radius <= 0f)
                return origin;

            Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;

            return new Vector3(origin.x + offset.x, origin.y, origin.z + offset.y);
        }

        private void PlaceBeacon(DropProfile profile, Vector3 position)
        {
            if (!profile.EnableBeacon || position == Vector3.zero)
                return;

            if (!ColorUtility.TryParseHtmlString(profile.BeaconColor, out Color color))
                color = Color.white;

            LightSourceToy beacon = LightSourceToy.Create(position + (Vector3.up * 1.5f));

            if (beacon is null)
                return;

            beacon.Color = color;
            beacon.Intensity = 3f;
            beacon.Range = 12f;
            beacon.ShadowType = LightShadows.None;

            beacons.Add(beacon);

            pending.Add(Timing.CallDelayed(Mathf.Max(1f, profile.BeaconDuration), () =>
            {
                beacons.Remove(beacon);
                DestroyBeacon(beacon);
            }));
        }

        private static void DestroyBeacon(LightSourceToy beacon)
        {
            try
            {
                if (beacon is null || beacon.IsDestroyed)
                    return;

                beacon.Destroy();
            }
            catch (Exception e)
            {
                Logger.Error($"DropService.DestroyBeacon: {e}");
            }
        }
    }
}
