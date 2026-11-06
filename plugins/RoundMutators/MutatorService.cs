using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Arcade;
using AugatonLib.Bus;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.World;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace RoundMutators
{
    public sealed class MutatorService
    {
        private const float RefreshSeconds = 2f;
        private const float DarknessSeconds = 30f;
        private const float DarknessRefreshSeconds = 20f;
        private const int RemovalsPerFrame = 10;

        private static readonly ItemType[] AmmoTypes =
        {
            ItemType.Ammo9x19,
            ItemType.Ammo556x45,
            ItemType.Ammo762x39,
            ItemType.Ammo44cal,
            ItemType.Ammo12gauge,
        };

        private static readonly ItemType[] MedicalTypes =
        {
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Adrenaline,
        };

        private static readonly MutatorType[] Types =
        {
            MutatorType.LowGravity,
            MutatorType.Swift,
            MutatorType.Tiny,
            MutatorType.Tireless,
            MutatorType.NightFall,
            MutatorType.ScarceAmmo,
            MutatorType.ScarceMedical,
            MutatorType.SwiftScps,
            MutatorType.FragileHumans,
        };

        private readonly Plugin plugin;
        private readonly RoundPacing pacing = new RoundPacing();
        private readonly HashSet<string> equipped = new HashSet<string>();

        private CoroutineHandle draw;
        private CoroutineHandle loop;
        private CoroutineHandle thinning;
        private bool drawScheduled;
        private bool looping;
        private bool thinningOut;
        private float nextDarkness;

        public MutatorService(Plugin plugin) => this.plugin = plugin;

        public MutatorType? Active { get; private set; }

        private Config Config => plugin.Config;

        public void ScheduleDraw()
        {
            CancelDraw();

            drawScheduled = true;
            draw = Timing.CallDelayed(Config.StartDelaySeconds, Draw);
        }

        public void Begin(MutatorType type)
        {
            Stop();

            Active = type;
            looping = true;
            loop = Timing.RunCoroutine(Refresh());

            if (type == MutatorType.ScarceAmmo || type == MutatorType.ScarceMedical)
            {
                thinningOut = true;
                thinning = Timing.RunCoroutine(ThinOut(type == MutatorType.ScarceAmmo ? AmmoTypes : MedicalTypes));
            }

            PluginBus.Publish(BusTopics.RoundMutatorStarted, Plugin.Owner, new RoundMutatorInfo(type.ToString(), NameOf(type)));

            plugin.Hints.ShowAll(plugin.Translation.Announcement.Replace("%NAME%", NameOf(type)), Config.HintDuration);
        }

        public void Stop()
        {
            CancelDraw();

            if (looping)
                Timing.KillCoroutines(loop);

            looping = false;

            if (thinningOut)
                Timing.KillCoroutines(thinning);

            thinningOut = false;
            nextDarkness = 0f;
            equipped.Clear();

            if (!Active.HasValue)
                return;

            MutatorType previous = Active.Value;

            Active = null;

            foreach (Player player in Player.ReadyList)
                Clear(player, previous);

            ScaleArbiter.ReleaseOwner(Plugin.Owner);
            LightArbiter.ReleaseOwner(Plugin.Owner);
        }

        public void OnGamemodeStarted(BusMessage message)
        {
            if (Config.PauseDuringGamemodes)
                Stop();
        }

        // Le chaos monte en cours de round : ce mutateur arrive quand la partie s'emballe, pas au depart.
        public void OnChaosLevelChanged(BusMessage message)
        {
            if (!message.TryGet(out int level) || Config.ChaosLevelForMutator <= 0 || level < Config.ChaosLevelForMutator)
                return;

            if (Active.HasValue || !Round.IsRoundInProgress || Player.Count < Config.MinPlayers)
                return;

            if (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses)
                return;

            BeginRandom();
        }

        public string NameOf(MutatorType type)
        {
            switch (type)
            {
                case MutatorType.LowGravity: return plugin.Translation.LowGravity;
                case MutatorType.Swift: return plugin.Translation.Swift;
                case MutatorType.Tiny: return plugin.Translation.Tiny;
                case MutatorType.Tireless: return plugin.Translation.Tireless;
                case MutatorType.NightFall: return plugin.Translation.NightFall;
                case MutatorType.ScarceAmmo: return plugin.Translation.ScarceAmmo;
                case MutatorType.ScarceMedical: return plugin.Translation.ScarceMedical;
                case MutatorType.SwiftScps: return plugin.Translation.SwiftScps;
                case MutatorType.FragileHumans: return plugin.Translation.FragileHumans;
                default: return type.ToString();
            }
        }

        private void Draw()
        {
            drawScheduled = false;

            try
            {
                if (!Round.IsRoundInProgress || Player.Count < Config.MinPlayers)
                    return;

                if (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses)
                    return;

                if (!pacing.TryAdvance(Config.RoundsBetween) || !Chance.Roll(Config.ChancePercent))
                    return;

                BeginRandom();
            }
            catch (Exception e)
            {
                Logger.Error($"MutatorService.Draw: {e}");
            }
        }

        private void BeginRandom()
        {
            int total = 0;

            foreach (MutatorType type in Types)
                total += Math.Max(0, WeightOf(type));

            if (total == 0)
                return;

            Begin(Chance.PickWeighted(Types, WeightOf));
            pacing.MarkUsed();
        }

        private int WeightOf(MutatorType type)
        {
            switch (type)
            {
                case MutatorType.LowGravity: return Config.LowGravityWeight;
                case MutatorType.Swift: return Config.SwiftWeight;
                case MutatorType.Tiny: return Config.TinyWeight;
                case MutatorType.Tireless: return Config.TirelessWeight;
                case MutatorType.NightFall: return Config.NightFallWeight;
                case MutatorType.ScarceAmmo: return Config.ScarceAmmoWeight;
                case MutatorType.ScarceMedical: return Config.ScarceMedicalWeight;
                case MutatorType.SwiftScps: return Config.SwiftScpsWeight;
                case MutatorType.FragileHumans: return Config.FragileHumansWeight;
                default: return 0;
            }
        }

        // Une seule boucle pour tous : elle rattrape les renforts et chaque changement de role, et passe
        // apres RealisticSizes, qui remet les tailles a zero au changement de role.
        private IEnumerator<float> Refresh()
        {
            while (Active.HasValue)
            {
                try
                {
                    KeepDark();

                    foreach (Player player in Player.ReadyList)
                        Apply(player, Active.Value);
                }
                catch (Exception e)
                {
                    Logger.Error($"MutatorService.Refresh: {e}");
                }

                yield return Timing.WaitForSeconds(RefreshSeconds);
            }
        }

        private void Apply(Player player, MutatorType type)
        {
            bool eligible = player.IsHuman && !player.IsTutorial;

            switch (type)
            {
                case MutatorType.LowGravity:
                    ApplyGravity(player, eligible);
                    break;

                case MutatorType.Swift when eligible:
                    if (!player.TryGetEffect(out MovementBoost boost) || boost.Intensity < Config.SpeedPercent)
                        player.EnableEffect<MovementBoost>((byte)Config.SpeedPercent);
                    break;

                case MutatorType.Tiny:
                    ApplySize(player, eligible);
                    break;

                case MutatorType.Tireless when eligible:
                    if (!player.HasEffect<Invigorated>())
                        player.EnableEffect<Invigorated>();
                    break;

                case MutatorType.NightFall when eligible:
                    if (equipped.Add(player.UserId))
                        ItemGrant.Give(player, ItemType.Flashlight);
                    break;

                // Un mort perd sa lampe : il doit en recevoir une autre s'il revient par une vague.
                case MutatorType.NightFall:
                    equipped.Remove(player.UserId);
                    break;

                case MutatorType.SwiftScps when player.IsSCP:
                    if (!player.TryGetEffect(out MovementBoost scpBoost) || scpBoost.Intensity < Config.SwiftScpPercent)
                        player.EnableEffect<MovementBoost>((byte)Config.SwiftScpPercent);
                    break;

                case MutatorType.FragileHumans when eligible:
                    ApplyFragility(player);
                    break;
            }
        }

        // Refait a chaque tour : le jeu rend sa vie normale a tout joueur qui change de role.
        private void ApplyFragility(Player player)
        {
            if (player.RoleBase is not IHealthbarRole role)
                return;

            float limit = role.MaxHealth * Config.FragileHealthPercent / 100f;

            if (player.MaxHealth <= limit)
                return;

            player.MaxHealth = limit;
            player.Health = Mathf.Min(player.Health, limit);
        }

        private void KeepDark()
        {
            if (Active != MutatorType.NightFall || Time.time < nextDarkness)
                return;

            nextDarkness = Time.time + DarknessRefreshSeconds;
            LightArbiter.Blackout(Plugin.Owner, DarknessSeconds, LightArbiter.AllZones);
        }

        private IEnumerator<float> ThinOut(ItemType[] types)
        {
            List<Pickup> targets = new List<Pickup>(64);

            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup is not null && Array.IndexOf(types, pickup.Type) >= 0)
                    targets.Add(pickup);
            }

            Chance.Shuffle(targets);

            int count = Mathf.RoundToInt(targets.Count * Config.ScarcePercent / 100f);

            for (int i = 0; i < count; i++)
            {
                if (!targets[i].IsDestroyed)
                    targets[i].Destroy();

                if ((i + 1) % RemovalsPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }
        }

        // La gravite reste attachee au joueur d'un role a l'autre : un humain devenu SCP doit la perdre.
        private void ApplyGravity(Player player, bool eligible)
        {
            Vector3 target = eligible
                ? FpcGravityController.DefaultGravity * Config.GravityFactor
                : FpcGravityController.DefaultGravity;

            if (FpcGravityController.GetGravity(player.ReferenceHub) != target)
                player.Gravity = target;
        }

        // Un autre calque passe avant : une pilule ou SCP-261 qui rapetisse un joueur ne doit pas etre ecrase.
        private void ApplySize(Player player, bool eligible)
        {
            if (!eligible)
            {
                ScaleArbiter.Release(player, Plugin.Owner);
                return;
            }

            if (!ScaleArbiter.HasLayer(player))
                ScaleArbiter.Request(player, Plugin.Owner, ScaleArbiter.BaselineOf(player) * Config.SizeFactor);
        }

        // Un mutateur peut s'arreter en plein round : il ne reprend que ce qu'il a pose, pas la vitesse de
        // ScpLevels ni les PV de RealisticSizes.
        private void Clear(Player player, MutatorType type)
        {
            bool eligible = player.IsHuman && !player.IsTutorial;

            switch (type)
            {
                case MutatorType.LowGravity:
                    player.Gravity = FpcGravityController.DefaultGravity;
                    break;

                case MutatorType.Swift when eligible:
                    ClearBoost(player, Config.SpeedPercent);
                    break;

                case MutatorType.SwiftScps when player.IsSCP:
                    ClearBoost(player, Config.SwiftScpPercent);
                    break;

                case MutatorType.FragileHumans when eligible:
                    ClearFragility(player);
                    break;

                case MutatorType.Tireless when eligible:
                    player.DisableEffect<Invigorated>();
                    break;
            }
        }

        private static void ClearBoost(Player player, int percent)
        {
            if (player.TryGetEffect(out MovementBoost boost) && boost.Intensity == percent)
                player.DisableEffect<MovementBoost>();
        }

        private void ClearFragility(Player player)
        {
            if (player.RoleBase is not IHealthbarRole role)
                return;

            if (Mathf.Approximately(player.MaxHealth, role.MaxHealth * Config.FragileHealthPercent / 100f))
                player.MaxHealth = role.MaxHealth;
        }

        private void CancelDraw()
        {
            if (drawScheduled)
                Timing.KillCoroutines(draw);

            drawScheduled = false;
        }
    }
}
