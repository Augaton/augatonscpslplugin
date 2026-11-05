using System;

namespace AugatonLib.Chaos
{
    /// <summary>
    /// Paliers de chaos ajoutes par un plugin comme ChaosDirector, en plus de ceux lies au temps.
    /// Sans lui, la valeur reste a zero et chaque plugin garde sa montee en puissance.
    /// </summary>
    public static class ChaosState
    {
        public static int BonusLevel { get; private set; }

        public static void SetBonusLevel(int level) => BonusLevel = Math.Max(0, level);

        public static void Reset() => BonusLevel = 0;
    }
}
