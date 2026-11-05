using System;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// Suit les TPS du serveur. Plusieurs mesures basses de suite sont exigees avant de declarer une
    /// souffrance, et un seuil de reprise plus haut evite d'alterner autour de la limite.
    /// </summary>
    public sealed class LoadMonitor
    {
        private int lowChecks;

        public bool IsDegraded { get; private set; }

        public float Lowest { get; private set; } = float.MaxValue;

        public int Episodes { get; private set; }

        // Vrai quand l'etat vient de changer.
        public bool Report(float tps, float minimum, float recovery, int checksBeforeAlert)
        {
            Lowest = Math.Min(Lowest, tps);

            if (IsDegraded)
            {
                if (tps < recovery)
                    return false;

                IsDegraded = false;
                lowChecks = 0;
                return true;
            }

            lowChecks = tps < minimum ? lowChecks + 1 : 0;

            if (lowChecks < checksBeforeAlert)
                return false;

            IsDegraded = true;
            Episodes++;
            return true;
        }

        public void Reset()
        {
            lowChecks = 0;
            IsDegraded = false;
            Lowest = float.MaxValue;
            Episodes = 0;
        }
    }
}
