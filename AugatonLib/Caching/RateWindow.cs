namespace AugatonLib.Caching
{
    /// <summary>
    /// Limite un debit par fenetre de temps, sans allocation : pense pour les chemins tres frequents.
    /// </summary>
    public sealed class RateWindow
    {
        private float windowEnd;
        private int used;

        public bool TryTake(float now, int limit, float windowSeconds = 1f)
        {
            if (now >= windowEnd)
            {
                windowEnd = now + windowSeconds;
                used = 0;
            }

            if (used >= limit)
                return false;

            used++;
            return true;
        }
    }
}
