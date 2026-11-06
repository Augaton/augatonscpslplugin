using System;
using System.Collections.Generic;
using BetterCoinflipsRewritten.Effects;
using BetterCoinflipsRewritten.Effects.Bad;
using BetterCoinflipsRewritten.Effects.Good;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten
{
    public sealed class CoinService
    {
        private readonly Plugin plugin;

        private readonly Dictionary<string, CoinState> playerStates;
        private readonly List<WeightedEffect> goodEffects;
        private readonly List<WeightedEffect> badEffects;
        private readonly List<WeightedEffect> candidateBuffer;

        public CoinService(Plugin plugin)
        {
            this.plugin = plugin;

            playerStates = new Dictionary<string, CoinState>();
            goodEffects = new List<WeightedEffect>();
            badEffects = new List<WeightedEffect>();
            candidateBuffer = new List<WeightedEffect>(24);

            RegisterEffects();
        }

        public void RegisterEffects()
        {
            goodEffects.Clear();
            badEffects.Clear();

            goodEffects.Add(
                new WeightedEffect(
                    new RandomKeycardEffect(plugin.Config),
                    plugin.Config.KeycardChance));

            goodEffects.Add(
                new WeightedEffect(
                    new SecondChanceEffect(),
                    plugin.Config.SecondChanceChance));

            goodEffects.Add(
                new WeightedEffect(
                    new JackpotEffect(plugin.Config.JackpotItems, plugin.Config.JackpotAmount),
                    plugin.Config.JackpotChance));

            goodEffects.Add(
                new WeightedEffect(
                    new FullMagazineEffect(),
                    plugin.Config.FullMagazineChance));

            goodEffects.Add(
                new WeightedEffect(
                    new GroupHealEffect(plugin.Config.GroupHealAmount, plugin.Config.GroupHealRange),
                    plugin.Config.GroupHealChance));

            goodEffects.Add(
                new WeightedEffect(
                    new DetectorEffect(),
                    plugin.Config.DetectorChance));

            goodEffects.Add(
                new WeightedEffect(
                    new MedicalKitEffect(),
                    plugin.Config.MedicalKitChance));

            goodEffects.Add(
                new WeightedEffect(
                    new TeleportToEscapeEffect(),
                    plugin.Config.TpToEscapeChance));

            goodEffects.Add(
                new WeightedEffect(
                    new HealEffect(plugin.Config.HealAmount),
                    plugin.Config.HealChance));

            goodEffects.Add(
                new WeightedEffect(
                    new MoreHealthEffect(plugin.Config.MoreHpPercent),
                    plugin.Config.MoreHpChance));

            goodEffects.Add(
                new WeightedEffect(
                    new HatEffect(),
                    plugin.Config.HatChance));

            goodEffects.Add(
                new WeightedEffect(
                    new OneAmmoLogicerEffect(),
                    plugin.Config.OneAmmoLogicerChance));

            goodEffects.Add(
                new WeightedEffect(
                    new LightbulbEffect(),
                    plugin.Config.LightbulbChance));

            goodEffects.Add(
                new WeightedEffect(
                    new PinkCandyEffect(),
                    plugin.Config.PinkCandyChance));

            goodEffects.Add(
                new WeightedEffect(
                    new SizeChangeEffect(),
                    plugin.Config.SizeChangeChance));

            goodEffects.Add(
                new WeightedEffect(
                    new AhpShieldEffect(plugin.Config.AhpShieldAmount),
                    plugin.Config.AhpShieldChance));

            goodEffects.Add(
                new WeightedEffect(
                    new GhostlyEffect(plugin.Config.GhostlyDuration),
                    plugin.Config.GhostlyChance));

            goodEffects.Add(
                new WeightedEffect(
                    new SugarRushEffect(plugin.Config.SugarRushDuration),
                    plugin.Config.SugarRushChance));

            goodEffects.Add(
                new WeightedEffect(
                    new DoubleCoinEffect(plugin.Config.MaxInventorySlots),
                    plugin.Config.DoubleCoinChance));

            badEffects.Add(
    new WeightedEffect(
        new HpReductionEffect(
            plugin.Config.HpReductionPercent),
        plugin.Config.HpReductionChance));

            badEffects.Add(
                new WeightedEffect(
                    new TeleportToClassDCellsEffect(),
                    plugin.Config.TpToClassDCellsChance));

            badEffects.Add(
                new WeightedEffect(
                    new LightsOutEffect(
                        plugin.Config.LightsOutDuration),
                    plugin.Config.LightsOutChance));

            badEffects.Add(
                new WeightedEffect(
                    new TrollFlashEffect(
                        plugin.Config.TrollFlashFuseTime),
                    plugin.Config.TrollFlashChance));

            badEffects.Add(
                new WeightedEffect(
                    new DisarmEffect(),
                    plugin.Config.DisarmChance));

            badEffects.Add(
                new WeightedEffect(
                    new OneHpLeftEffect(),
                    plugin.Config.OneHpLeftChance));

            badEffects.Add(
                new WeightedEffect(
                    new InventoryResetEffect(),
                    plugin.Config.InventoryResetChance));

            badEffects.Add(
                new WeightedEffect(
                    new InstantExplosionEffect(
                        plugin.Config.InstantExplosionFuseTime),
                    plugin.Config.InstantExplosionChance));

            badEffects.Add(
                new WeightedEffect(
                    new RandomTeleportEffect(),
                    plugin.Config.RandomTeleportChance));

            badEffects.Add(
                new WeightedEffect(
                    new DrunkEffect(plugin.Config.DrunkDuration),
                    plugin.Config.DrunkChance));

            badEffects.Add(
                new WeightedEffect(
                    new PrismaticTripEffect(
                        plugin.Config.PrismaticTripDuration),
                    plugin.Config.PrismaticTripChance));

            badEffects.Add(
                new WeightedEffect(
                    new AmnesiaEffect(plugin.Config.AmnesiaDuration),
                    plugin.Config.AmnesiaChance));

            badEffects.Add(
                new WeightedEffect(
                    new SkyLaunchEffect(plugin.Config.SkyLaunchHeight),
                    plugin.Config.SkyLaunchChance));

            badEffects.Add(
                new WeightedEffect(
                    new PositionSwapEffect(),
                    plugin.Config.PositionSwapChance));

            badEffects.Add(
                new WeightedEffect(
                    new FakeWarheadEffect(
                        plugin.Config.FakeWarheadDimDuration),
                    plugin.Config.FakeWarheadChance));

            badEffects.Add(
                new WeightedEffect(
                    new TantrumTrapEffect(),
                    plugin.Config.TantrumTrapChance));

            badEffects.Add(
                new WeightedEffect(
                    new PetrifiedEffect(plugin.Config.PetrifiedDuration),
                    plugin.Config.PetrifiedChance));

            badEffects.Add(
                new WeightedEffect(
                    new DiscoLightsEffect(
                        plugin.Config.DiscoLightsDuration,
                        plugin.Config.DiscoLightsInterval),
                    plugin.Config.DiscoLightsChance));

            badEffects.Add(
                new WeightedEffect(
                    new MiceInvasionEffect(plugin.Config.MiceInvasionType),
                    plugin.Config.MiceInvasionChance));

            badEffects.Add(
                new WeightedEffect(
                    new CassieMockEffect(
                        plugin.Config.CassieMockLines,
                        plugin.Config.CassieMockGlitchChance,
                        plugin.Config.CassieMockJamChance),
                    plugin.Config.CassieMockChance));

            badEffects.Add(
                new WeightedEffect(
                    new FakeExplosionEffect(
                        plugin.Config.FakeExplosionStunDuration),
                    plugin.Config.FakeExplosionChance));

            Logger.Info(
                "[BetterCoinflipsRewritten] Registered " +
                goodEffects.Count +
                " positive effects and " +
                badEffects.Count +
                " negative effects.");

            if (TotalWeight(goodEffects) <= 0)
            {
                Logger.Warn(
                    "[BetterCoinflipsRewritten] Every positive effect has a " +
                    "weight of 0, heads will never trigger an effect.");
            }

            if (TotalWeight(badEffects) <= 0)
            {
                Logger.Warn(
                    "[BetterCoinflipsRewritten] Every negative effect has a " +
                    "weight of 0, tails will never trigger an effect.");
            }
        }

        private static int TotalWeight(List<WeightedEffect> effects)
        {
            int total = 0;

            foreach (WeightedEffect weightedEffect in effects)
            {
                if (weightedEffect.Weight > 0)
                    total += weightedEffect.Weight;
            }

            return total;
        }

        public bool IsOnCooldown(
        Player player,
        out double secondsRemaining)
        {
            secondsRemaining = 0;

            if (player is null)
                return true;

            CoinState state;

            if (!playerStates.TryGetValue(player.UserId, out state))
                return false;

            double elapsedSeconds =
                (DateTime.UtcNow - state.LastFlipTime).TotalSeconds;

            double cooldown = plugin.Config.CoinCooldown;

            if (elapsedSeconds >= cooldown)
                return false;

            secondsRemaining = cooldown - elapsedSeconds;
            return true;
        }

        public void SetCooldown(Player player)
        {
            if (player is null)
                return;

            CoinState state;

            if (!playerStates.TryGetValue(player.UserId, out state))
            {
                state = new CoinState();
                playerStates.Add(player.UserId, state);
            }

            state.LastFlipTime = DateTime.UtcNow;
        }

        public ICoinEffect SelectEffect(
            bool isTails,
            Player player)
        {
            List<WeightedEffect> source =
                isTails ? badEffects : goodEffects;

            candidateBuffer.Clear();

            int totalWeight = 0;

            foreach (WeightedEffect weightedEffect in source)
            {
                if (weightedEffect is null ||
                    weightedEffect.Effect is null ||
                    weightedEffect.Weight <= 0)
                {
                    continue;
                }

                if (!weightedEffect.Effect.CanApply(player))
                    continue;

                candidateBuffer.Add(weightedEffect);
                totalWeight += weightedEffect.Weight;
            }

            if (candidateBuffer.Count == 0 || totalWeight <= 0)
                return null;

            int selectedNumber =
                API.Rng.NextWeight(totalWeight);

            ICoinEffect fallback =
                candidateBuffer[candidateBuffer.Count - 1].Effect;

            foreach (WeightedEffect weightedEffect in candidateBuffer)
            {
                selectedNumber -= weightedEffect.Weight;

                if (selectedNumber <= 0)
                {
                    candidateBuffer.Clear();
                    return weightedEffect.Effect;
                }
            }

            candidateBuffer.Clear();
            return fallback;
        }

        public void ExecuteFlip(
    Player player,
    bool isTails)
        {
            if (player is null ||
                player.IsDestroyed ||
                !player.IsAlive)
            {
                return;
            }

            ICoinEffect selectedEffect =
                SelectEffect(isTails, player);

            if (selectedEffect is null)
            {
                Logger.Warn(
                    "[BetterCoinflipsRewritten] No available effect was " +
                    "found for " +
                    player.Nickname +
                    ".");

                plugin.Hints.Show(
                    player,
                    plugin.Translation.CoinFlipTitle +
                    "\n" +
                    plugin.Translation.NoAvailableEffect,
                    plugin.Config.EffectMessageDuration);

                return;
            }

            try
            {
                selectedEffect.Execute(player);

                string translatedResult =
                    isTails
                        ? plugin.Translation.TailsName
                        : plugin.Translation.HeadsName;

                string resultLine =
                    plugin.Translation.ResultFormat.Replace(
                        "{result}",
                        translatedResult);

                string effectLine =
                    plugin.Translation.EffectFormat.Replace(
                        "{effect}",
                        selectedEffect.Message);

                string resultMessage =
                    plugin.Translation.CoinFlipTitle +
                    "\n" +
                    resultLine +
                    "\n" +
                    effectLine;

                if (plugin.Config.ConsumeCoinOnFlip)
                {
                    resultMessage +=
                        "\n" +
                        plugin.Translation.CoinConsumedLine;
                }

                plugin.Hints.Show(
                    player,
                    resultMessage,
                    plugin.Config.EffectMessageDuration);

                if (plugin.Config.LogFlips)
                {
                    Logger.Info(
                        "[BetterCoinflipsRewritten] " +
                        player.Nickname +
                        " (" + player.UserId + ") flipped " +
                        (isTails ? "tails" : "heads") +
                        ". Effect: " +
                        selectedEffect.Name +
                        ".");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(
                    "[BetterCoinflipsRewritten] Error while executing effect " +
                    selectedEffect.Name +
                    ": " +
                    exception);

                if (!player.IsDestroyed)
                {
                    plugin.Hints.Show(
                        player,
                        plugin.Translation.EffectError,
                        plugin.Config.EffectMessageDuration);
                }
            }
        }

        public void RemovePlayer(Player player)
        {
            if (player is null ||
                string.IsNullOrEmpty(player.UserId))
            {
                return;
            }

            playerStates.Remove(player.UserId);
        }

        public void Clear()
        {
            playerStates.Clear();
            candidateBuffer.Clear();
        }
    }
}
