using System;

namespace AugatonLib.Arcade
{
    public static class ScpHealthScale
    {
        public static float Factor(int players, int referencePlayers, float percentPerPlayer, float minFactor, float maxFactor)
        {
            float factor = 1f + ((players - referencePlayers) * percentPerPlayer / 100f);

            return Math.Min(maxFactor, Math.Max(minFactor, factor));
        }
    }
}
