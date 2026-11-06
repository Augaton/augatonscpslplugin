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

        // Un remplacant sans objectif reprend celui en cours du joueur parti, et un remplacant qui en a un le garde.
        // L'ancien titulaire conserve une entree close : revenu dans le round, il n'en recoit pas un second.
        public bool Transfer(string from, string to)
        {
            if (from is null || string.IsNullOrEmpty(to) || objectives.ContainsKey(to))
                return false;

            if (!objectives.TryGetValue(from, out Objective objective) || objective.Progress >= objective.Target)
                return false;

            objectives[to] = new Objective { Kind = objective.Kind, Target = objective.Target, Progress = objective.Progress };
            objective.Progress = objective.Target;
            return true;
        }

        // Refuse sur un objectif rempli : ce joueur a deja touche sa recompense.
        public bool Discard(string player)
        {
            if (player is null || !objectives.TryGetValue(player, out Objective objective))
                return false;

            return objective.Progress < objective.Target && objectives.Remove(player);
        }

        public void Clear() => objectives.Clear();
    }
}
