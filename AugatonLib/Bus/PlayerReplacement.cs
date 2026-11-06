using LabApi.Features.Wrappers;
using UnityEngine;

namespace AugatonLib.Bus
{
    public sealed class PlayerReplacement
    {
        public PlayerReplacement(Player replacement, Vector3 size, string replacedUserId, bool healthCarried)
        {
            Replacement = replacement;
            Size = size;
            ReplacedUserId = replacedUserId;
            HealthCarried = healthCarried;
        }

        public Player Replacement { get; }

        public Vector3 Size { get; }

        public string ReplacedUserId { get; }

        // Vrai si le remplacant a deja recu les PV du joueur parti : un bonus de vie n'est pas a refaire.
        public bool HealthCarried { get; }
    }
}
