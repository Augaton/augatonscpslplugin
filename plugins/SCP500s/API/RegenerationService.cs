using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public sealed class RegenerationService
    {
        private const float TickSeconds = 1f;

        private readonly Dictionary<Player, Regeneration> active = new Dictionary<Player, Regeneration>();
        private readonly List<Player> finished = new List<Player>(8);

        private CoroutineHandle loop;
        private bool running;

        public int Count => active.Count;

        public void Start(Player player, float perSecond, float seconds)
        {
            if (player is null || !player.IsAlive || perSecond <= 0f || seconds <= 0f)
                return;

            active[player] = new Regeneration(perSecond * TickSeconds, Time.time + seconds);

            if (running)
                return;

            running = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public void Remove(Player player)
        {
            if (player is not null)
                active.Remove(player);
        }

        public void Clear()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;
            active.Clear();
            finished.Clear();
        }

        private IEnumerator<float> Loop()
        {
            while (running && active.Count > 0)
            {
                yield return Timing.WaitForSeconds(TickSeconds);

                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"RegenerationService.Tick: {e}");
                }
            }

            running = false;
        }

        private void Tick()
        {
            float now = Time.time;
            finished.Clear();

            foreach (KeyValuePair<Player, Regeneration> entry in active)
            {
                Player player = entry.Key;

                if (player is null || player.IsDestroyed || !player.IsAlive || now >= entry.Value.Until)
                {
                    finished.Add(player);
                    continue;
                }

                player.Heal(entry.Value.Amount);
            }

            foreach (Player player in finished)
                active.Remove(player);
        }

        private readonly struct Regeneration
        {
            public Regeneration(float amount, float until)
            {
                Amount = amount;
                Until = until;
            }

            public float Amount { get; }

            public float Until { get; }
        }
    }
}
