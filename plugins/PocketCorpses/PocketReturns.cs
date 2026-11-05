using System;
using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PlayerRoles.PlayableScps.Scp106;
using RelativePositioning;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Random = UnityEngine.Random;

namespace PocketCorpses
{
    internal sealed class PocketReturns
    {
        private const float TickSeconds = 0.25f;

        private readonly Plugin plugin;
        private readonly List<FallingCorpse> corpses = new List<FallingCorpse>();
        private readonly HashSet<ItemPickupBase> announcedItems = new HashSet<ItemPickupBase>();
        private readonly Sinkholes sinkholes = new Sinkholes();

        private CoroutineHandle loop;
        private bool running;

        public PocketReturns(Plugin plugin)
        {
            this.plugin = plugin;
        }

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
            corpses.Clear();
            announcedItems.Clear();
            sinkholes.CloseAll();
        }

        public void Track(Ragdoll ragdoll)
        {
            Config config = plugin.Config;
            float delay = Random.Range(config.MinDelaySeconds, config.MaxDelaySeconds);

            corpses.Add(new FallingCorpse(ragdoll, NetworkTime.time + delay));
        }

        private IEnumerator<float> Loop()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(TickSeconds);

                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"PocketReturns: {e}");
                }
            }
        }

        private void Tick()
        {
            double now = NetworkTime.time;
            Config config = plugin.Config;

            UpdateCorpses(now, config);

            if (config.SinkholeUnderItems)
                AnnounceItems(now, config);

            sinkholes.CloseExpired(now);
        }

        private void UpdateCorpses(double now, Config config)
        {
            for (int i = corpses.Count - 1; i >= 0; i--)
            {
                FallingCorpse corpse = corpses[i];

                if (corpse.Ragdoll.IsDestroyed)
                {
                    corpses.RemoveAt(i);
                    continue;
                }

                double remaining = corpse.DropTime - now;

                if (remaining > config.WarningSeconds)
                    continue;

                if (corpse.Roof is null)
                {
                    // Sans joueur hors de la dimension, le corps attend : rien ne se perd.
                    if (DropPoints.TryFindRoof(out Vector3 roof))
                        Announce(corpse, roof, now, config);

                    continue;
                }

                if (remaining > 0d)
                    continue;

                corpses.RemoveAt(i);

                // Le client ne fait pas retomber un corps deplace : seul un corps recree reprend sa physique.
                corpse.Ragdoll.Position = corpse.Roof.Value;
                corpse.Ragdoll.UnFreeze();
            }
        }

        private void Announce(FallingCorpse corpse, Vector3 roof, double now, Config config)
        {
            corpse.Roof = roof;

            NetworkServer.SendToAll(
                new Scp106PocketItemManager.WarningMessage { Position = new RelativePosition(roof) },
                sendToReadyOnly: true);

            if (config.SinkholeUnderCorpses)
                sinkholes.OpenBelow(roof, now + config.SinkholeSeconds);
        }

        private void AnnounceItems(double now, Config config)
        {
            foreach (KeyValuePair<ItemPickupBase, Scp106PocketItemManager.PocketItem> tracked in Scp106PocketItemManager.TrackedItems)
            {
                Scp106PocketItemManager.PocketItem item = tracked.Value;

                if (item.Remove || item.TriggerTime - now > config.WarningSeconds)
                    continue;

                if (announcedItems.Add(tracked.Key))
                    sinkholes.OpenBelow(item.DropPosition.Position, now + config.SinkholeSeconds);
            }
        }
    }
}
