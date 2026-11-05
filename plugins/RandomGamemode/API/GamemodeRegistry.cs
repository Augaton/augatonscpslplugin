using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.FriendlyFire;
using AugatonLib.Random;
using AugatonLib.Runtime;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using RandomGamemode.Gamemodes;
using Respawning;
using Respawning.Waves;

namespace RandomGamemode.API
{
    public sealed class GamemodeRegistry
    {
        private readonly Plugin plugin;
        private readonly List<GamemodeBase> all = new List<GamemodeBase>(24);
        private readonly List<GamemodeBase> pool = new List<GamemodeBase>(24);

        private CoroutineHandle runHandle;
        private CoroutineHandle limitHandle;
        private bool runScheduled;
        private bool limitScheduled;
        private bool friendlyFireOverridden;
        private bool genocideSuppressed;
        private bool roundEndForced;

        private const string Owner = Interop.Owner;

        public GamemodeRegistry(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        public GamemodeBase Active { get; private set; }

        public GamemodeVote Vote { get; } = new GamemodeVote();

        public string Forced { get; set; }

        public IReadOnlyList<GamemodeBase> All => all;

        // Round.End(true) ne suffit pas : RoundSummary reecrit IsRoundEnded toutes les 2,5 s d'apres les
        // camps en vie, puis ne garde que CanEnd. Le forcage doit donc passer par cet event.
        public void ForceRoundEnd()
        {
            roundEndForced = true;
            Round.End(true);
        }

        public void ApplyForcedEnd(RoundEndingConditionsCheckEventArgs ev)
        {
            if (roundEndForced)
                ev.CanEnd = true;
        }

        public void Build()
        {
            all.Clear();

            all.Add(new Dodgeball());
            all.Add(new PeanutRaid());
            all.Add(new BlueScreenOfDeath());
            all.Add(new NightOfTheLivingNerd());
            all.Add(new Randomizer());
            all.Add(new AnnoyingMimicry());
            all.Add(new LockedIn());
            all.Add(new Infection());
            all.Add(new LivingLikeLarry());
            all.Add(new CustomRoleChaos());
            all.Add(new CustomTeamWar());
            all.Add(new CustomItemRush());
            all.Add(new PillParty());
            all.Add(new CoinRoyale());
            all.Add(new Juggernaut());
            all.Add(new Blackout());
            all.Add(new GunGame());

            Logger.Info($"[RandomGamemode] {all.Count} modes enregistres.");
        }

        public GamemodeBase Find(string key)
        {
            foreach (GamemodeBase gamemode in all)
            {
                if (string.Equals(gamemode.Key, key, StringComparison.OrdinalIgnoreCase))
                    return gamemode;
            }

            return null;
        }

        public void OnRoundStarted()
        {
            try
            {
                int players = Player.Count;
                GamemodeBase selected = null;

                if (!string.IsNullOrEmpty(Forced))
                {
                    selected = Find(Forced);
                    Forced = null;

                    if (selected is null)
                        Logger.Warn("[RandomGamemode] Le mode force n'existe plus, tirage normal.");

                    if (selected is not null && !selected.CanStart(players))
                    {
                        Logger.Warn($"[RandomGamemode] Prerequis manquants pour le mode force \"{selected.Key}\", tirage normal.");
                        selected = null;
                    }
                }

                selected ??= DrawMode(players);
                Vote.Cancel();

                if (selected is null)
                    return;

                Begin(selected);
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        // Le tirage part du lobby, et non du debut de round, pour laisser aux joueurs le temps de voter.
        public void PrepareVote()
        {
            Vote.Cancel();
            plugin.VoteCooldowns.Clear();

            if (!Config.EnableVote || !string.IsNullOrEmpty(Forced) || !RollsGamemode())
                return;

            List<GamemodeBase> candidates = DrawVoteOptions();

            if (candidates.Count < 2)
                return;

            Vote.Open(candidates);

            foreach (Player player in Player.ReadyList)
                ShowVote(player);
        }

        public void ShowVote(Player player)
        {
            if (Vote.IsOpen && player is not null)
                player.SendBroadcast(plugin.Translation.VoteBroadcast.Replace("%OPTIONS%", Vote.Describe()), Config.VoteBroadcastDuration);
        }

        private bool RollsGamemode() => UnityEngine.Random.Range(0, 100) < Config.GamemodeChance;

        private GamemodeBase DrawMode(int players)
        {
            if (!Config.EnableVote)
                return RollsGamemode() ? PickWeighted(players) : null;

            if (!Vote.IsOpen)
                return null;

            return Vote.Winner(gamemode => CanRun(gamemode, players)) ?? PickWeighted(players);
        }

        private List<GamemodeBase> DrawVoteOptions()
        {
            List<GamemodeBase> remaining = all.FindAll(gamemode => gamemode.Settings.IsEnabled && gamemode.Settings.Weight > 0 && gamemode.CanStart(int.MaxValue));
            List<GamemodeBase> drawn = new List<GamemodeBase>(Config.VoteOptions);

            while (drawn.Count < Config.VoteOptions && remaining.Count > 0)
            {
                GamemodeBase gamemode = Chance.PickWeighted(remaining, candidate => candidate.Settings.Weight);

                drawn.Add(gamemode);
                remaining.Remove(gamemode);
            }

            return drawn;
        }

        private static bool CanRun(GamemodeBase gamemode, int players)
        {
            GamemodeSettings settings = gamemode.Settings;

            return settings.IsEnabled
                && settings.Weight > 0
                && players >= settings.MinPlayers
                && (settings.MaxPlayers <= 0 || players <= settings.MaxPlayers)
                && gamemode.CanStart(players);
        }

        public void EnforceHumansOnly(Player player)
        {
            GamemodeBase active = Active;

            if (active is null || !active.HumansOnly)
                return;

            active.EnforceHumanRole(player);
        }

        public void Begin(GamemodeBase gamemode)
        {
            if (gamemode is null || Active is not null)
                return;

            Active = gamemode;

            if (gamemode.Settings.ForceFriendlyFire)
            {
                friendlyFireOverridden = true;
                FriendlyFireArbiter.RequestOn(Owner);
            }

            if (gamemode.Settings.SuppressGenocideAnnouncements)
            {
                genocideSuppressed = true;
                GenocideArbiter.Suppress(Owner);
            }

            if (gamemode.Settings.PauseRespawnWaves)
                SetWavesPaused(true);

            if (gamemode.PausesWorldEvents)
                WorldEvents.Pause(Owner);

            Server.SendBroadcast(string.Format(Config.StartText, gamemode.DisplayName), Config.HeaderDuration);

            runScheduled = true;
            runHandle = Timing.RunCoroutine(gamemode.Run(), Segment.FixedUpdate);

            if (gamemode.Settings.RoundTimeLimit > 0f)
            {
                limitScheduled = true;
                limitHandle = Timing.CallDelayed(gamemode.Settings.RoundTimeLimit, () =>
                {
                    limitScheduled = false;

                    try
                    {
                        ForceRoundEnd();
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Limite de temps du mode {gamemode.Key} : {e}");
                    }
                });
            }

            PluginBus.Publish(BusTopics.GamemodeStarted, Owner, gamemode.Key);

            Logger.Info($"[RandomGamemode] Mode \"{gamemode.Key}\" demarre.");
        }

        public void End(bool broadcast)
        {
            if (Active is null)
            {
                Cleanup();
                return;
            }

            GamemodeBase finished = Active;
            Active = null;

            try
            {
                finished.Stop();
                finished.RestoreHealth();
            }
            catch (Exception e)
            {
                Logger.Error($"Arret du mode {finished.Key} : {e}");
            }

            finished.KillTrackedCoroutines();
            Cleanup();

            if (broadcast)
                Server.SendBroadcast(string.Format(Config.EndText, finished.DisplayName), Config.HeaderDuration);

            PluginBus.Publish(BusTopics.GamemodeEnded, Owner, finished.Key);

            Logger.Info($"[RandomGamemode] Mode \"{finished.Key}\" termine.");
        }

        private void Cleanup()
        {
            if (runScheduled)
            {
                Timing.KillCoroutines(runHandle);
                runScheduled = false;
            }

            if (limitScheduled)
            {
                Timing.KillCoroutines(limitHandle);
                limitScheduled = false;
            }

            roundEndForced = false;
            WorldEvents.Resume(Owner);

            if (friendlyFireOverridden)
            {
                FriendlyFireArbiter.Release(Owner);
                friendlyFireOverridden = false;
            }

            if (genocideSuppressed)
            {
                GenocideArbiter.Allow(Owner);
                genocideSuppressed = false;
            }

            LightArbiter.ReleaseOwner(Owner);
            ScaleArbiter.ReleaseOwner(Owner);
            GateLock.Disengage();

            SetWavesPaused(false);
            plugin.Hints.Clear();
        }

        private static void SetWavesPaused(bool paused)
        {
            foreach (SpawnableWaveBase wave in WaveManager.Waves)
            {
                RespawnWave wrapper = RespawnWaves.Get(wave);

                if (wrapper is not null)
                    wrapper.IsForcefullyPaused = paused;
            }
        }

        private GamemodeBase PickWeighted(int players)
        {
            pool.Clear();
            int total = 0;

            foreach (GamemodeBase gamemode in all)
            {
                if (!CanRun(gamemode, players))
                    continue;

                pool.Add(gamemode);
                total += gamemode.Settings.Weight;
            }

            if (pool.Count == 0 || total <= 0)
                return null;

            int roll = UnityEngine.Random.Range(0, total);

            foreach (GamemodeBase gamemode in pool)
            {
                roll -= gamemode.Settings.Weight;

                if (roll < 0)
                    return gamemode;
            }

            return pool[pool.Count - 1];
        }
    }
}
