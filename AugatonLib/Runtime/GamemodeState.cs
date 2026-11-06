using AugatonLib.Bus;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// Dit si un mode de jeu tourne. Tous les modes ne posent pas WorldEvents : ce drapeau les voit tous.
    /// Il retombe en fin de round, jamais au debut, ou il partirait sur le meme evenement que le mode.
    /// </summary>
    public static class GamemodeState
    {
        private const string Source = "AugatonLib";

        private static bool hooked;

        public static bool IsActive { get; private set; }

        // Un bonus d'arcade fausserait un mode, et chargerait un serveur que TickGuard a mis en pause.
        public static bool BlocksBonuses => IsActive || WorldEvents.IsPaused;

        public static void Begin() => IsActive = true;

        public static void End() => IsActive = false;

        internal static void Hook()
        {
            if (hooked)
                return;

            hooked = true;
            PluginBus.Subscribe(Source, BusTopics.GamemodeStarted, _ => Begin());
            PluginBus.Subscribe(Source, BusTopics.GamemodeEnded, _ => End());
        }
    }
}
