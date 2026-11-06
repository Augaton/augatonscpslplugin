namespace AugatonLib.Arcade
{
    /// <summary>
    /// Garde un nombre de rounds normaux entre deux evenements rares, pour qu'ils le restent.
    /// </summary>
    public sealed class RoundPacing
    {
        private int normalRounds = int.MaxValue;

        // A appeler une fois par round : le round compte comme normal s'il est refuse ici.
        public bool TryAdvance(int requiredNormalRounds)
        {
            if (normalRounds >= requiredNormalRounds)
                return true;

            normalRounds++;
            return false;
        }

        public void MarkUsed() => normalRounds = 0;
    }
}
