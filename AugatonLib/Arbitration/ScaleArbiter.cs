using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace AugatonLib.Arbitration
{
    public static class ScaleArbiter
    {
        private sealed class Layer
        {
            public Layer(string owner, Vector3 scale)
            {
                Owner = owner;
                Scale = scale;
            }

            public string Owner { get; }

            public Vector3 Scale { get; }
        }

        private sealed class State
        {
            public Vector3 Baseline { get; set; } = Vector3.one;

            public List<Layer> Layers { get; } = new List<Layer>(2);
        }

        private static readonly Dictionary<string, State> States =
            new Dictionary<string, State>(StringComparer.Ordinal);

        private static bool hooked;

        public static int TrackedPlayers => States.Count;

        public static int LayerCount
        {
            get
            {
                int total = 0;

                foreach (State state in States.Values)
                    total += state.Layers.Count;

                return total;
            }
        }

        internal static void Hook()
        {
            if (hooked)
                return;

            hooked = true;
            PlayerEvents.ChangedRole += OnChangedRole;
        }

        public static void Reset(Player player, Vector3 baseline)
        {
            if (!TryKey(player, out string userId))
                return;

            State state = Ensure(userId);
            state.Layers.Clear();
            state.Baseline = Sanitize(baseline);

            Apply(player, state);
        }

        public static void SetBaseline(Player player, Vector3 baseline)
        {
            if (!TryKey(player, out string userId))
                return;

            State state = Ensure(userId);
            state.Baseline = Sanitize(baseline);

            Apply(player, state);
        }

        public static Vector3 BaselineOf(Player player)
        {
            return TryKey(player, out string userId) && States.TryGetValue(userId, out State state)
                ? state.Baseline
                : Vector3.one;
        }

        public static void Request(Player player, string owner, Vector3 scale)
        {
            if (string.IsNullOrEmpty(owner) || !TryKey(player, out string userId))
                return;

            State state = Ensure(userId);
            state.Layers.RemoveAll(layer => string.Equals(layer.Owner, owner, StringComparison.Ordinal));
            state.Layers.Add(new Layer(owner, Sanitize(scale)));

            Apply(player, state);
        }

        public static void Release(Player player, string owner)
        {
            if (string.IsNullOrEmpty(owner) || !TryKey(player, out string userId))
                return;

            if (!States.TryGetValue(userId, out State state))
                return;

            if (state.Layers.RemoveAll(layer => string.Equals(layer.Owner, owner, StringComparison.Ordinal)) == 0)
                return;

            Apply(player, state);
        }

        public static void ReleaseOwner(string owner)
        {
            if (string.IsNullOrEmpty(owner))
                return;

            foreach (KeyValuePair<string, State> entry in States)
            {
                if (entry.Value.Layers.RemoveAll(layer => string.Equals(layer.Owner, owner, StringComparison.Ordinal)) == 0)
                    continue;

                Apply(Player.Get(entry.Key), entry.Value);
            }
        }

        public static void Forget(Player player)
        {
            if (TryKey(player, out string userId))
                States.Remove(userId);
        }

        public static void Clear()
        {
            foreach (KeyValuePair<string, State> entry in States)
            {
                entry.Value.Layers.Clear();
                entry.Value.Baseline = Vector3.one;
                Apply(Player.Get(entry.Key), entry.Value);
            }

            States.Clear();
        }

        internal static void ForgetAll() => States.Clear();

        public static string Describe()
        {
            if (States.Count == 0)
                return "aucun joueur suivi";

            return $"{States.Count} joueur(s) suivi(s), {LayerCount} calque(s) actif(s)";
        }

        // Player.Scale ne fait rien hors role FPC : une taille liberee a la mort n'atteint pas le jeu, qui
        // la reapplique au role suivant. Sans ce rattrapage, le joueur la garderait jusqu'a la fin du round.
        private static void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            try
            {
                if (TryKey(ev?.Player, out string userId) && States.TryGetValue(userId, out State state))
                    Apply(ev.Player, state);
            }
            catch (Exception e)
            {
                Logger.Error($"ScaleArbiter: {e}");
            }
        }

        private static void Apply(Player player, State state)
        {
            if (player is null || player.IsDestroyed)
                return;

            Vector3 target = state.Layers.Count == 0
                ? state.Baseline
                : state.Layers[state.Layers.Count - 1].Scale;

            if (player.Scale == target)
                return;

            player.Scale = target;
        }

        private static State Ensure(string userId)
        {
            if (States.TryGetValue(userId, out State existing))
                return existing;

            State created = new State();
            States[userId] = created;
            return created;
        }

        private static Vector3 Sanitize(Vector3 scale)
        {
            if (scale.x <= 0f || scale.y <= 0f || scale.z <= 0f)
                return Vector3.one;

            return scale;
        }

        private static bool TryKey(Player player, out string userId)
        {
            userId = player?.UserId;

            return !string.IsNullOrEmpty(userId);
        }
    }
}
