using System.Collections.Generic;

namespace BetterCoinflipsRewritten.API
{
    public static class GlobalCooldown
    {
        private static readonly Dictionary<string, float> LastUse = new Dictionary<string, float>();

        public static bool IsReady(string key, float durationSeconds)
        {
            if (durationSeconds <= 0f)
                return true;

            return !LastUse.TryGetValue(key, out float last) ||
                   UnityEngine.Time.realtimeSinceStartup - last >= durationSeconds;
        }

        public static bool TryConsume(string key, float durationSeconds)
        {
            if (durationSeconds <= 0f)
                return true;

            float now = UnityEngine.Time.realtimeSinceStartup;

            if (LastUse.TryGetValue(key, out float last) && now - last < durationSeconds)
                return false;

            LastUse[key] = now;
            return true;
        }

        public static void Clear() => LastUse.Clear();
    }
}
