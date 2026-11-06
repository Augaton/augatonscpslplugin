using System.Collections.Generic;

namespace AugatonLib.World
{
    /// <summary>
    /// Objets deja utilises mais pas encore detruits. Une piece lancee reste en main le temps de son
    /// animation : sans cette liste, elle pourrait servir une seconde fois dans un autre plugin.
    /// </summary>
    public static class SpentItems
    {
        private static readonly HashSet<ushort> Serials = new HashSet<ushort>();

        public static bool IsSpent(ushort serial) => Serials.Contains(serial);

        public static void Mark(ushort serial) => Serials.Add(serial);

        public static void Release(ushort serial) => Serials.Remove(serial);

        public static void Clear() => Serials.Clear();
    }
}
