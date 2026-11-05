using System;
using System.Collections.Generic;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// Suspend ce qui apparait sur la carte sans action des joueurs : largages, pilules, pieces,
    /// incidents. Un mode de type arene pose cette pause, sinon un largage d'armes casserait un
    /// Gun Game. TickGuard la pose aussi quand le serveur souffre. Sans plugin pour la demander, rien
    /// n'est jamais suspendu.
    /// </summary>
    public static class WorldEvents
    {
        private static readonly HashSet<string> Pausers = new HashSet<string>(StringComparer.Ordinal);

        public static bool IsPaused => Pausers.Count > 0;

        public static void Pause(string owner)
        {
            if (!string.IsNullOrEmpty(owner))
                Pausers.Add(owner);
        }

        public static void Resume(string owner)
        {
            if (!string.IsNullOrEmpty(owner))
                Pausers.Remove(owner);
        }

        public static void Clear() => Pausers.Clear();
    }
}
