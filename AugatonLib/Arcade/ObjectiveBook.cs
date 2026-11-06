using System.Collections.Generic;

namespace AugatonLib.Arcade
{
    /// <summary>
    /// Un objectif par joueur et par round. Un objectif rempli reste inscrit : il n'en repart pas un autre
    /// avant le round suivant, et le joueur ne peut pas toucher sa recompense deux fois.
    /// </summary>
    public sealed class ObjectiveBook<TKind>
    {
        private sealed class Objective
        {
            public TKind Kind { get; set; }

            public int Target { get; set; }

            public int Progress { get; set; }
        }

        private readonly Dictionary<string, Objective> objectives = new Dictionary<string, Objective>();

        public bool Has(string player) => player is not null && objectives.ContainsKey(player);

        public bool Assign(string player, TKind kind, int target)
        {
            if (string.IsNullOrEmpty(player) || target <= 0 || objectives.ContainsKey(player))
                return false;

            objectives[player] = new Objective { Kind = kind, Target = target };
            return true;
        }

        public bool TryGet(string player, out TKind kind, out int progress, out int target)
        {
            kind = default;
            progress = 0;
            target = 0;

            if (player is null || !objectives.TryGetValue(player, out Objective objective))
                return false;

            kind = objective.Kind;
            progress = objective.Progress;
            target = objective.Target;
            return true;
        }

        // Vrai seulement a l'instant ou l'objectif se remplit.
        public bool Advance(string player, TKind kind, int amount = 1)
        {
            if (player is null || amount <= 0 || !objectives.TryGetValue(player, out Objective objective))
                return false;

            if (objective.Progress >= objective.Target || !EqualityComparer<TKind>.Default.Equals(objective.Kind, kind))
                return false;

            objective.Progress += amount;
            return objective.Progress >= objective.Target;
        }

        public void Forget(string player)
        {
            if (player is not null)
                objectives.Remove(player);
        }

        public void Clear() => objectives.Clear();
    }
}
