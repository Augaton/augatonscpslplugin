using AugatonLib.Effects;
using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.Runtime;
using Cassie;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using SCP500s.API;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public override string Name => "SCP500s";

        public override string Description => "44 pilules SCP-500 personnalisees et une roulette d'effets.";

        public override string Author => "Zone-Shilari (base: VividZap, Lumi)";

        public static Plugin Instance { get; private set; }

        public int RoundId { get; private set; }

        public GlowService Glow { get; private set; }

        public ScaleTracker Scales { get; private set; }

        public RoomTinter Tints { get; private set; }

        public DelayedRunner Delayed { get; private set; }

        public AugatonLib.Caching.RoomCache Rooms { get; private set; }

        public TargetPicker Targets { get; private set; }

        public RegenerationService Regeneration { get; private set; }

        public PillRegistry Registry => registry;

        private PillRegistry registry;
        private LootReplacementService lootReplacement;
        private ContinuousSpawnService continuousSpawn;
        private CoroutineHandle spawnHandle;
        private bool spawnScheduled;
        private bool cassieLinesChecked;

        public override void Enable()
        {
            Instance = this;


            Scales = new ScaleTracker();
            Tints = new RoomTinter();
            Delayed = new DelayedRunner();
            Rooms = new AugatonLib.Caching.RoomCache();
            Targets = new TargetPicker();
            Regeneration = new RegenerationService();
            registry = new PillRegistry();
            Glow = new GlowService(registry);
            lootReplacement = new LootReplacementService(registry);
            continuousSpawn = new ContinuousSpawnService(registry, lootReplacement);

            BuildPills();

            registry.Start();
            Glow.Start();

            // ServerEvents.RoundStarted part avant l'attribution des roles.
            CharacterClassManager.OnRoundStarted += OnRoundStarted;
            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundRestarted += OnRoundRestarted;
            PlayerEvents.ChangedRole += OnChangedRole;
            PlayerEvents.Left += OnLeft;

            PluginBus.Subscribe(Interop.Owner, BusTopics.PillSpawnRequested, OnPillSpawnRequested);
            PluginBus.Subscribe(Interop.Owner, BusTopics.PillConsumed, OnPillConsumed);
            PluginBus.Subscribe(Interop.Owner, BusTopics.PillGiveRequested, OnPillGiveRequested);

            PluginDirectory.Register(
                this,
                Capability.Hints,
                Capability.Scale,
                Capability.Light,
                Capability.Bus);

            Logger.Info($"{registry.Pills.Count} pilules SCP-500 enregistrees, mode de spawn {Config.SpawnMode}.");
        }

        public override void Disable()
        {
            CharacterClassManager.OnRoundStarted -= OnRoundStarted;
            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
            PlayerEvents.ChangedRole -= OnChangedRole;
            PlayerEvents.Left -= OnLeft;

            KillPendingSpawn();

            registry?.Stop();
            Glow?.Stop();
            Hints.Clear();
            continuousSpawn?.Stop();
            Regeneration?.Clear();
            lootReplacement?.Reset();
            Delayed?.Clear();
            Tints?.Clear();
            Scales?.Clear();
            Rooms?.Clear();
            Targets?.Clear();

            LightArbiter.ReleaseOwner(Interop.Owner);
            PluginBus.UnsubscribeOwner(Interop.Owner);
            PluginDirectory.Unregister(this);

            registry = null;
            lootReplacement = null;
            continuousSpawn = null;
            Glow = null;
            Delayed = null;
            Tints = null;
            Scales = null;
            Rooms = null;
            Targets = null;
            Regeneration = null;
            Instance = null;
        }

        private void BuildPills()
        {
            HashSet<uint> seenIds = new HashSet<uint>();
            HashSet<string> seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> seenNames = new HashSet<string>(StringComparer.Ordinal);
            HashSet<uint> seenColors = new HashSet<uint>();

            foreach (PillDefinition definition in Config.Pills)
            {
                if (definition is null || !definition.IsEnabled)
                    continue;

                if (string.IsNullOrEmpty(definition.Key))
                {
                    Logger.Warn($"Pilule id {definition.Id} sans cle, ignoree.");
                    continue;
                }

                if (!seenIds.Add(definition.Id))
                {
                    Logger.Warn($"Identifiant {definition.Id} en double ({definition.Key}), pilule ignoree.");
                    continue;
                }

                if (!seenKeys.Add(definition.Key))
                {
                    Logger.Warn($"Cle {definition.Key} en double, pilule ignoree.");
                    continue;
                }

                if (definition.DisplayName is null || !seenNames.Add(definition.DisplayName))
                {
                    Logger.Warn($"Nom affiche \"{definition.DisplayName}\" vide ou en double ({definition.Key}), pilule ignoree.");
                    continue;
                }

                if (!ColorUtility.TryParseHtmlString(definition.GlowColor, out Color color))
                {
                    Logger.Warn($"Couleur {definition.GlowColor} invalide pour {definition.Key}, blanc utilise.");
                    color = Color.white;
                }

                if (!seenColors.Add((uint)color.GetHashCode()))
                    Logger.Warn($"La couleur {definition.GlowColor} de {definition.Key} est deja utilisee par une autre pilule.");

                PillBase pill = CreatePill(definition, color);

                if (Config.SpawnMode == SpawnMode.LootReplacement)
                    pill.ClearSpawnPoints();
                else
                    pill.BuildSpawnPoints();

                registry.Add(pill);
            }

            if (registry.Find(Config.RandomPillKey) is null)
                Logger.Warn($"Aucune pilule enregistree avec la cle {Config.RandomPillKey}, la part de pilules aleatoires ne sera pas appliquee.");
        }

        private PillBase CreatePill(PillDefinition definition, Color color)
        {
            if (string.Equals(definition.Key, Config.RandomPillKey, StringComparison.OrdinalIgnoreCase))
            {
                ChaosPill chaos = new ChaosPill(definition, color, Config.ChaosOutcomes);

                if (chaos.OutcomeCount == 0)
                    Logger.Warn("chaos_outcomes est vide, la pilule aleatoire ne fera rien.");

                return chaos;
            }

            return new EffectPill(definition, color);
        }

        protected override void ValidateConfig()
        {
            EnsureLists();

            ClampPercent(nameof(Config.ReplacementChancePercent), Config.ReplacementChancePercent, v => Config.ReplacementChancePercent = v);
            ClampPercent(nameof(Config.CoinSharePercent), Config.CoinSharePercent, v => Config.CoinSharePercent = v);
            ClampPercent(nameof(Config.RandomPillSharePercent), Config.RandomPillSharePercent, v => Config.RandomPillSharePercent = v);
            ClampUnit(nameof(Config.CassieGlitchChance), Config.CassieGlitchChance, v => Config.CassieGlitchChance = v);
            ClampUnit(nameof(Config.CassieJamChance), Config.CassieJamChance, v => Config.CassieJamChance = v);

            if (string.IsNullOrEmpty(Config.RandomPillKey))
            {
                Logger.Warn("RandomPillKey est vide, remis a chaos.");
                Config.RandomPillKey = "chaos";
            }

            if (Config.ChaosGoodWeight < 0f || Config.ChaosBadWeight < 0f || Config.ChaosNeutralWeight < 0f)
            {
                Logger.Warn("Un poids de categorie de la pilule aleatoire est negatif, les trois sont remis a 33.");
                Config.ChaosGoodWeight = 33f;
                Config.ChaosBadWeight = 33f;
                Config.ChaosNeutralWeight = 33f;
            }

            if (Config.HintDuration <= 0f)
            {
                Logger.Warn($"HintDuration ({Config.HintDuration}) invalide, remis a 5.");
                Config.HintDuration = 5f;
            }

            if (Config.RakunScale <= 0f || Config.RakunScale > 1f)
            {
                Logger.Warn($"RakunScale ({Config.RakunScale}) hors bornes, remis a 0.5.");
                Config.RakunScale = 0.5f;
            }

            if (Config.GiantScale < 1f || Config.GiantScale > 3f)
            {
                Logger.Warn($"GiantScale ({Config.GiantScale}) hors bornes, remis a 1.4.");
                Config.GiantScale = 1.4f;
            }

            if (Config.MaxEffectDuration <= 0f)
            {
                Logger.Warn($"MaxEffectDuration ({Config.MaxEffectDuration}) invalide, remis a 300.");
                Config.MaxEffectDuration = 300f;
            }

            if (Config.MaxActiveGlows < 0)
            {
                Logger.Warn($"MaxActiveGlows ({Config.MaxActiveGlows}) negatif, remis a 60.");
                Config.MaxActiveGlows = 60;
            }

            if (Config.MaxInventorySlots < 1 || Config.MaxInventorySlots > 8)
            {
                Logger.Warn($"MaxInventorySlots ({Config.MaxInventorySlots}) hors bornes, remis a 8.");
                Config.MaxInventorySlots = 8;
            }

            if (Config.PillCooldownSeconds < 0f)
            {
                Logger.Warn($"PillCooldownSeconds ({Config.PillCooldownSeconds}) negatif, remis a 0.");
                Config.PillCooldownSeconds = 0f;
            }

            if (Config.SpawnDelaySeconds < 0f)
            {
                Logger.Warn($"SpawnDelaySeconds ({Config.SpawnDelaySeconds}) negatif, remis a 0.");
                Config.SpawnDelaySeconds = 0f;
            }

            if (Config.HintYCoordinate < 0f || Config.HintYCoordinate > 1080f)
            {
                Logger.Warn($"HintYCoordinate ({Config.HintYCoordinate}) hors de l'ecran, remis a 700.");
                Config.HintYCoordinate = 700f;
            }

            if (Config.HintFontSize < 10 || Config.HintFontSize > 60)
            {
                Logger.Warn($"HintFontSize ({Config.HintFontSize}) hors bornes 10-60, remis a 20.");
                Config.HintFontSize = 20;
            }

            if (Config.ContinuousSpawnIntervalSeconds < 30f)
            {
                Logger.Warn($"ContinuousSpawnIntervalSeconds ({Config.ContinuousSpawnIntervalSeconds}) trop bas, remis a 30.");
                Config.ContinuousSpawnIntervalSeconds = 30f;
            }

            Config.MaxPillsOnGround = Mathf.Clamp(Config.MaxPillsOnGround, 1, 200);

            Config.RenewalDelaySeconds = Mathf.Max(0f, Config.RenewalDelaySeconds);

            if (Config.ContinuousSpawnTarget < 0)
            {
                Logger.Warn($"ContinuousSpawnTarget ({Config.ContinuousSpawnTarget}) negatif, remis a 0.");
                Config.ContinuousSpawnTarget = 0;
            }

            if (Config.ContinuousSpawnBatch < 1)
            {
                Logger.Warn($"ContinuousSpawnBatch ({Config.ContinuousSpawnBatch}) invalide, remis a 1.");
                Config.ContinuousSpawnBatch = 1;
            }

            if (Config.ContinuousSpawnPlayerDistance < 0f)
            {
                Logger.Warn($"ContinuousSpawnPlayerDistance ({Config.ContinuousSpawnPlayerDistance}) negatif, remis a 0.");
                Config.ContinuousSpawnPlayerDistance = 0f;
            }

            if (Config.ContinuousSpawnSpacing < 0f)
            {
                Logger.Warn($"ContinuousSpawnSpacing ({Config.ContinuousSpawnSpacing}) negatif, remis a 0.");
                Config.ContinuousSpawnSpacing = 0f;
            }

            if (Config.RadarMaxEntries < 1)
            {
                Logger.Warn($"RadarMaxEntries ({Config.RadarMaxEntries}) invalide, remis a 1.");
                Config.RadarMaxEntries = 1;
            }

            if (Config.ContagionRadius <= 0f)
            {
                Logger.Warn($"ContagionRadius ({Config.ContagionRadius}) invalide, remis a 12.");
                Config.ContagionRadius = 12f;
            }

            if (Config.Pills.Count == 0)
                Logger.Warn("Le catalogue de pilules est vide, aucune pilule ne sera enregistree.");

            if (Config.SpawnMode != SpawnMode.SpawnPoints && Config.ReplaceableSourceItems.Count == 0)
                Logger.Warn("ReplaceableSourceItems est vide, le remplacement du loot ne fera rien.");

            ValidateEffects();
            ValidateCassieLines();
        }

        private void ValidateEffects()
        {
            foreach (PillDefinition pill in Config.Pills)
            {
                if (pill is not null)
                    ValidateEffects(pill.Key, pill.Effects);
            }

            foreach (ChaosOutcome outcome in Config.ChaosOutcomes)
            {
                if (outcome is not null)
                    ValidateEffects(outcome.Key, outcome.Effects);
            }
        }

        private static void ValidateEffects(string owner, List<EffectEntry> effects)
        {
            if (effects is null)
                return;

            foreach (EffectEntry effect in effects)
            {
                if (effect is null || string.IsNullOrEmpty(effect.Type) || EffectLookup.IsKnown(effect.Type))
                    continue;

                Logger.Warn($"Effet inconnu \"{effect.Type}\" dans {owner}, il sera ignore.");
            }
        }

        private void EnsureLists()
        {
            Config.Pills = Replace(nameof(Config.Pills), Config.Pills);
            Config.ChaosOutcomes = Replace(nameof(Config.ChaosOutcomes), Config.ChaosOutcomes);
            Config.ReplaceableSourceItems = Replace(nameof(Config.ReplaceableSourceItems), Config.ReplaceableSourceItems);
            Config.IgnoredRoles = Replace(nameof(Config.IgnoredRoles), Config.IgnoredRoles);
            Config.ForceWaveFactions = Replace(nameof(Config.ForceWaveFactions), Config.ForceWaveFactions);
            Config.CassieLines = Replace(nameof(Config.CassieLines), Config.CassieLines);
            Config.RandomOnlyPills = Replace(nameof(Config.RandomOnlyPills), Config.RandomOnlyPills);
            Config.KeycardUpgradePath = Replace(nameof(Config.KeycardUpgradePath), Config.KeycardUpgradePath);
        }

        private static List<T> Replace<T>(string name, List<T> value)
        {
            if (value is not null)
                return value;

            Logger.Warn($"{name} est nul dans la config, liste vide utilisee.");
            return new List<T>();
        }

        private void ValidateCassieLines()
        {
            if (cassieLinesChecked || Announcer.LineDatabase == null)
                return;

            cassieLinesChecked = true;

            foreach (string line in Config.CassieLines)
            {
                if (!string.IsNullOrEmpty(line) && Announcer.CalculateDuration(line, new CassiePlaybackModifiers()) > 0d)
                    continue;

                Logger.Warn($"Annonce Cassie refusee par le jeu, elle ne sera jamais jouee : \"{line}\".");
            }
        }

        private static void ClampPercent(string name, float value, Action<float> setter)
        {
            if (value >= 0f && value <= 100f)
                return;

            float clamped = value < 0f ? 0f : 100f;
            Logger.Warn($"{name} ({value}) hors bornes 0-100, borne a {clamped}.");
            setter(clamped);
        }

        private static void ClampUnit(string name, float value, Action<float> setter)
        {
            if (value >= 0f && value <= 1f)
                return;

            float clamped = value < 0f ? 0f : 1f;
            Logger.Warn($"{name} ({value}) hors bornes 0-1, borne a {clamped}.");
            setter(clamped);
        }

        private void OnRoundStarted()
        {
            try
            {
                StartRound();
            }
            catch (Exception e)
            {
                Logger.Error($"OnRoundStarted: {e}");
            }
        }

        private void StartRound()
        {
            ValidateCassieLines();
            RoundId++;
            Delayed.Clear();
            Tints.Clear();
            Scales.Clear();
            LightArbiter.ReleaseOwner(Interop.Owner);
            Rooms.Rebuild();
            registry.ClearCooldowns();
            continuousSpawn.Stop();
            Regeneration.Clear();
            lootReplacement.Reset();

            KillPendingSpawn();
            spawnScheduled = true;
            spawnHandle = Timing.CallDelayed(Config.SpawnDelaySeconds, () =>
            {
                spawnScheduled = false;

                try
                {
                    if (Config.SpawnMode != SpawnMode.SpawnPoints && !(Config.PauseDuringGamemodes && WorldEvents.IsPaused))
                        lootReplacement.Run();

                    continuousSpawn.Start();
                }
                catch (Exception e)
                {
                    Logger.Error($"StartRound (apparition des pilules): {e}");
                }
            });
        }

        private void OnRoundEnded(RoundEndedEventArgs ev) => Cleanup();

        private void OnRoundRestarted() => Cleanup();

        private void Cleanup()
        {
            try
            {
                ResetState();
            }
            catch (Exception e)
            {
                Logger.Error($"Cleanup: {e}");
            }
        }

        private void ResetState()
        {
            RoundId++;
            KillPendingSpawn();
            Hints.Clear();
            Delayed.Clear();
            Tints.Clear();
            Scales.Clear();
            LightArbiter.ReleaseOwner(Interop.Owner);
            Glow.ClearAll();
            Rooms.Clear();
            Targets.Clear();
            registry.ClearCooldowns();
            continuousSpawn.Stop();
            Regeneration.Clear();
            lootReplacement.Reset();
        }

        private void KillPendingSpawn()
        {
            if (!spawnScheduled)
                return;

            Timing.KillCoroutines(spawnHandle);
            spawnScheduled = false;
        }

        // PlayerEvents.Spawned ne part que pour les roles FPC : un joueur tue passerait au spectateur
        // sans liberer sa taille.
        private void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                Scales.Restore(ev?.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangedRole: {e}");
            }
        }

        private void OnPillConsumed(BusMessage message) => continuousSpawn.OnPillConsumed();

        private void OnPillGiveRequested(BusMessage message)
        {
            if (message.TryGet(out Player player))
                lootReplacement.GiveTo(player);
        }

        private void OnPillSpawnRequested(BusMessage message)
        {
            if (message.TryGet(out Vector3 position))
                lootReplacement.SpawnAt(position);
        }

        private void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                Scales.Remove(ev?.Player);
                Regeneration.Remove(ev?.Player);
                Hints.Remove(ev?.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }
    }
}
