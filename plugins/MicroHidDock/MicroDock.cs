using System;
using System.Collections.Generic;
using AugatonLib.Caching;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace MicroHidDock
{
    internal sealed class MicroDock
    {
        private const float TickSeconds = 1f;

        private readonly Plugin plugin;
        private readonly List<MicroHIDPickup> docked = new List<MicroHIDPickup>();
        private readonly CooldownTracker cooldowns = new CooldownTracker();

        private CoroutineHandle loop;
        private bool running;

        public MicroDock(Plugin plugin)
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
            docked.Clear();
            cooldowns.Clear();
        }

        public bool IsOnCooldown(Player player)
        {
            return cooldowns.IsOnCooldown(player, plugin.Config.DockCooldownSeconds, out _);
        }

        public bool TryFindFreeChamber(Vector3 position, out LockerChamber chamber)
        {
            float range = plugin.Config.DockRange;

            foreach (MicroPedestal pedestal in MicroPedestal.List)
            {
                chamber = pedestal.Chambers[0];

                if ((chamber.Base.Spawnpoint.position - position).sqrMagnitude <= range * range && chamber.IsEmpty)
                    return true;
            }

            chamber = null;
            return false;
        }

        // Le pickup reste celui que le joueur portait : son serial garde l'energie et l'etat du Micro-HID.
        public void Seat(Player player, MicroHIDPickup pickup, LockerChamber chamber)
        {
            if (pickup is null || !pickup.GameObject.TryGetComponent(out Rigidbody body))
                return;

            Transform spot = chamber.Base.Spawnpoint;

            // Cinematique pour qu'il ne glisse ni ne tombe. Le jeu synchronise Rigidbody.position, pas le
            // transform : le corps doit bouger tout de suite, sinon les clients gardent le point de chute.
            body.isKinematic = true;
            body.position = spot.position;
            body.rotation = spot.rotation;
            pickup.Position = spot.position;
            pickup.Rotation = spot.rotation;

            chamber.Base.Content.Add(pickup.Base);
            docked.Add(pickup);
            cooldowns.Register(player);

            plugin.Hints.Show(player, plugin.Translation.Docked, plugin.Config.HintDuration);
        }

        private IEnumerator<float> Loop()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(TickSeconds);

                try
                {
                    Recharge();
                }
                catch (Exception e)
                {
                    Logger.Error($"MicroDock: {e}");
                }
            }
        }

        private void Recharge()
        {
            float rate = plugin.Config.RechargePerSecond;
            float limit = plugin.Config.RechargeLimit;

            for (int i = docked.Count - 1; i >= 0; i--)
            {
                MicroHIDPickup pickup = docked[i];

                if (pickup.IsDestroyed)
                {
                    docked.RemoveAt(i);
                    continue;
                }

                float energy = pickup.Energy;

                if (rate > 0f && energy < limit)
                    pickup.Energy = Mathf.Min(limit, energy + (rate * TickSeconds));
            }
        }
    }
}
