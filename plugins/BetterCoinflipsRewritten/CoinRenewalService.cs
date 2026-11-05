using System;
using System.Collections.Generic;
using AugatonLib.Chaos;
using AugatonLib.Runtime;
using AugatonLib.World;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace BetterCoinflipsRewritten
{
    public sealed class CoinRenewalService
    {
        private const float MinIntervalSeconds = 30f;

        private readonly Plugin plugin;
        private readonly LootSpots spots = new LootSpots();

        private CoroutineHandle loop;
        private bool running;

        public CoinRenewalService(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void Start()
        {
            Stop();

            if (!plugin.Config.CoinRenewal || plugin.Config.CoinRenewalTarget <= 0)
                return;

            spots.Collect();

            if (spots.Count == 0)
            {
                Logger.Warn("[BetterCoinflipsRewritten] No loot spot found, coins will not renew this round.");
                return;
            }

            running = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public void Stop()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;
            spots.Clear();
        }

        private IEnumerator<float> Loop()
        {
            while (running)
            {
                yield return Timing.WaitForSeconds(Mathf.Max(MinIntervalSeconds, plugin.Config.CoinRenewalIntervalSeconds));

                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"CoinRenewalService.Tick: {e}");
                }
            }
        }

        private void Tick()
        {
            Config config = plugin.Config;

            if (!Round.IsRoundInProgress || (config.PauseDuringGamemodes && WorldEvents.IsPaused))
                return;

            int level = Escalation.Level(Round.Duration.TotalSeconds, config.CoinChaosStepSeconds, config.CoinChaosMaxLevel);
            int target = Escalation.Grow(config.CoinRenewalTarget, config.CoinChaosTargetPerLevel, level);
            int missing = Mathf.Min(target - CountCoinsOnGround(), config.CoinRenewalBatch);

            for (int i = 0; i < missing; i++)
            {
                if (!spots.TryPick(IsNearPlayer, out Vector3 position))
                    return;

                CoinSpawnService.SpawnCoin(position, Quaternion.identity);
            }
        }

        private bool IsNearPlayer(Vector3 position)
        {
            return LootSpots.IsNearLivingPlayer(position, plugin.Config.CoinRenewalPlayerDistance);
        }

        private static int CountCoinsOnGround()
        {
            int count = 0;

            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup is not null && pickup.Type == ItemType.Coin && pickup.IsSpawned)
                    count++;
            }

            return count;
        }
    }
}
