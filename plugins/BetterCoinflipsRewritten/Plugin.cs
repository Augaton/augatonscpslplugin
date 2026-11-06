using MapGeneration;
using BetterCoinflipsRewritten.Effects.Bad;
using System.Linq;
using AugatonLib.Runtime;
using AugatonLib.Text;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        private static readonly FacilityZone[] FalseAlarmZones =
        {
            FacilityZone.LightContainment,
            FacilityZone.HeavyContainment,
            FacilityZone.Entrance,
            FacilityZone.Surface,
        };

        public override string Name => "BetterCoinflipsRewritten";

        public override string Description => "Pile ou face : la piece declenche un effet benefique ou nefaste.";

        public override string Author => "Matisiowy, original plugin written by Mikihero";

        public static Plugin Instance { get; private set; }

        private CoinService coinService;
        private EventHandlers eventHandlers;
        private CoinSpawnService coinSpawnService;
        private CoinConsumeService coinConsumeService;
        private CoinRenewalService coinRenewalService;

        public override void Enable()
        {
            Instance = this;


            coinService = new CoinService(this);
            coinSpawnService = new CoinSpawnService(this);
            coinConsumeService = new CoinConsumeService(this);
            coinRenewalService = new CoinRenewalService(this);

            eventHandlers = new EventHandlers(
                this,
                coinService,
                coinSpawnService,
                coinConsumeService,
                coinRenewalService);

            PlayerEvents.FlippingCoin += eventHandlers.OnFlippingCoin;
            PlayerEvents.Left += eventHandlers.OnLeft;
            PlayerEvents.ChangedRole += eventHandlers.OnChangedRole;
            PlayerEvents.Dying += eventHandlers.OnDying;

            CharacterClassManager.OnRoundStarted += eventHandlers.OnRoundStarted;
            ServerEvents.RoundRestarted += eventHandlers.OnRoundRestarted;

            if (Round.IsRoundStarted)
                API.RoomCache.Rebuild();

            SelfCheck.Register(Name, "mots de C.A.S.S.I.E", CheckCassieWords);

            PluginDirectory.Register(
                this,
                Capability.Hints,
                Capability.Scale,
                Capability.Light,
                Capability.Bus);

            Logger.Info($"[BetterCoinflipsRewritten] Coins on map: {Config.MapCoinAmount}, extra coins in loot pools: {(Config.AddBonusCoinsToLoot ? Config.BonusCoinAmount : 0)}, coin consumed on flip: {Config.ConsumeCoinOnFlip}.");
        }

        private string CheckCassieWords()
        {
            string unknown = CassieLines.FindUnknownWord(Config.CassieMockLines)
                ?? CassieLines.FindUnknownWord(FalseAlarmZones.Select(FalseAlarmEffect.CassieLine));

            return unknown is null ? null : $"mot inconnu : {unknown}";
        }

        public override void LoadConfigs()
        {
            base.LoadConfigs();
            coinService?.RegisterEffects();
        }

        protected override void ValidateConfig()
        {
            Config.FacilityEffectCooldown = AtLeast("facility_effect_cooldown", Config.FacilityEffectCooldown, 1f);
            Config.CoinCooldown = AtLeast("coin_cooldown", Config.CoinCooldown, 0.5f);
            Config.SpottedDuration = AtLeast("spotted_duration", Config.SpottedDuration, 4f);
            Config.GroupHealRange = AtLeast("group_heal_range", Config.GroupHealRange, 1f);
            Config.JackpotAmount = (int)AtLeast("jackpot_amount", Config.JackpotAmount, 1f);
            Config.SecondChanceHealth = AtLeast("second_chance_health", Config.SecondChanceHealth, 1f);

            Config.CoinConsumeDelay = Clamp(
                "coin_consume_delay",
                Config.CoinConsumeDelay,
                0.5f,
                10f);

            Config.PetrifiedDuration = Clamp(
                "petrified_duration",
                Config.PetrifiedDuration,
                0f,
                15f);

            Config.SkyLaunchHeight = Clamp(
                "sky_launch_height",
                Config.SkyLaunchHeight,
                0f,
                100f);

            Config.DiscoLightsInterval = Clamp(
                "disco_lights_interval",
                Config.DiscoLightsInterval,
                0.2f,
                10f);

            Config.CassieMockGlitchChance = Clamp(
                "cassie_mock_glitch_chance",
                Config.CassieMockGlitchChance,
                0f,
                1f);

            Config.CassieMockJamChance = Clamp(
                "cassie_mock_jam_chance",
                Config.CassieMockJamChance,
                0f,
                1f);

            Config.MaxInventorySlots = (int)Clamp(
                "max_inventory_slots",
                Config.MaxInventorySlots,
                0f,
                InventorySystem.Inventory.MaxSlots);

            if (Config.AddBonusCoinsToLoot &&
                (Config.BonusCoinSourceItems is null ||
                 Config.BonusCoinSourceItems.Count == 0))
            {
                Logger.Warn(
                    "[BetterCoinflipsRewritten] bonus_coin_source_items is " +
                    "empty, no extra coin will reach the loot pools.");
            }

            if (!Config.ConsumeCoinOnFlip)
                return;

            if (Config.AddBonusCoinsToLoot || Config.SpawnCoinsOnMap)
                return;

            Logger.Warn(
                "[BetterCoinflipsRewritten] Coins are consumed on flip but " +
                "no coin is placed on the map. Only the vanilla loot table " +
                "will provide them.");
        }

        private static float Clamp(
            string name,
            float value,
            float minimum,
            float maximum)
        {
            if (value >= minimum && value <= maximum)
                return value;

            float clamped = value < minimum ? minimum : maximum;

            Logger.Warn(
                "[BetterCoinflipsRewritten] " +
                name +
                " (" +
                value.ToString("0.##") +
                ") is out of the " +
                minimum.ToString("0.##") +
                "-" +
                maximum.ToString("0.##") +
                " range, clamped to " +
                clamped.ToString("0.##") +
                ".");

            return clamped;
        }

        public override void Disable()
        {
            if (eventHandlers is not null)
            {
                PlayerEvents.FlippingCoin -= eventHandlers.OnFlippingCoin;
                PlayerEvents.Left -= eventHandlers.OnLeft;
                PlayerEvents.ChangedRole -= eventHandlers.OnChangedRole;
                PlayerEvents.Dying -= eventHandlers.OnDying;

                CharacterClassManager.OnRoundStarted -= eventHandlers.OnRoundStarted;
                ServerEvents.RoundRestarted -= eventHandlers.OnRoundRestarted;

                eventHandlers.KillPendingSpawn();
            }

            coinService?.Clear();

            Hints.Clear();
            API.Scheduler.Clear();
            coinRenewalService?.Stop();
            API.GlobalCooldown.Clear();
            API.RoomCache.Clear();

            EventHandlers.ReleaseArbiters();
            PluginDirectory.Unregister(this);

            eventHandlers = null;
            coinService = null;
            coinSpawnService = null;
            coinConsumeService = null;
            Instance = null;
        }
    }
}
