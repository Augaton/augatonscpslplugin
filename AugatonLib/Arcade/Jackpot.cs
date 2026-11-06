using System;

namespace AugatonLib.Arcade
{
    /// <summary>
    /// Une cagnotte qui grossit a chaque mise : plus elle attend, plus elle a de chances de tomber
    /// et plus elle rapporte. Elle repart de zero quand quelqu'un l'emporte.
    /// </summary>
    public sealed class Jackpot
    {
        public int Stakes { get; private set; }

        public void Add() => Stakes++;

        public float ChancePercent(float basePercent, float percentPerStake)
        {
            return Math.Min(100f, basePercent + (percentPerStake * Stakes));
        }

        // Au moins un lot, et jamais plus que maxPrizes : une longue attente ne vide pas la carte.
        public int Claim(int stakesPerPrize, int maxPrizes)
        {
            int prizes = stakesPerPrize > 0 ? 1 + (Stakes / stakesPerPrize) : 1;

            Stakes = 0;
            return Math.Max(1, Math.Min(prizes, maxPrizes));
        }

        public void Clear() => Stakes = 0;
    }
}
