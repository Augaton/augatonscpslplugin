using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.API
{
    public static class RoomCache
    {
        private static readonly AugatonLib.Caching.RoomCache Cache = new AugatonLib.Caching.RoomCache();

        public static int Count => Cache.Count;

        public static void Rebuild() => Cache.Rebuild();

        public static void Clear() => Cache.Clear();

        public static Room PickRandom() => Cache.PickRandom();
    }
}
