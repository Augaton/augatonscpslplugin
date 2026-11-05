using LabApi.Features.Wrappers;
using UnityEngine;

namespace AugatonLib.Bus
{
    public sealed class PlayerReplacement
    {
        public PlayerReplacement(Player replacement, Vector3 size)
        {
            Replacement = replacement;
            Size = size;
        }

        public Player Replacement { get; }

        public Vector3 Size { get; }
    }
}
