namespace AugatonLib.Runtime
{
    /// <summary>
    /// Espace les gros evenements automatiques. Chaque plugin a des delais raisonnables seul, mais deux
    /// qui tombent ensemble noient les joueurs : le second attend. Sans plugin qui le demande, rien ne change.
    /// </summary>
    public static class EventSpacing
    {
        private static double nextAllowed = double.NegativeInfinity;

        public static bool TryClaim(double now, double gapSeconds)
        {
            if (now < nextAllowed)
                return false;

            nextAllowed = now + gapSeconds;
            return true;
        }

        public static void Clear() => nextAllowed = double.NegativeInfinity;
    }
}
