using System;

namespace AugatonLib.Chaos
{
    /// <summary>
    /// Fait monter un reglage par paliers au fil du round. Le calcul reste sans etat : le palier se
    /// deduit de la duree du round, et un rechargement de config s'applique donc aussitot.
    /// </summary>
    public static class Escalation
    {
        public static int Level(double elapsedSeconds, float stepSeconds, int maxLevel)
        {
            if (stepSeconds <= 0f || maxLevel <= 0 || elapsedSeconds <= 0d)
                return 0;

            return (int)Math.Min((elapsedSeconds / stepSeconds) + ChaosState.BonusLevel, maxLevel);
        }

        public static int Grow(int baseValue, int perLevel, int level) => baseValue + (Math.Max(0, perLevel) * level);

        public static float Shrink(float baseValue, float perLevel, int level, float floor)
        {
            return Math.Max(floor, baseValue - (Math.Max(0f, perLevel) * level));
        }
    }
}
