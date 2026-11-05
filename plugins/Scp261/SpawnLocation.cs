using System.ComponentModel;
using MapGeneration;
using UnityEngine;

namespace Scp261
{
    public sealed class SpawnLocation
    {
        [Description("Salle ou placer la machine.")]
        public RoomName RoomName { get; set; }

        [Description("Position relative a la salle.")]
        public Vector3 Position { get; set; }

        [Description("Rotation relative a la salle.")]
        public Vector3 Rotation { get; set; }

        [Description("Chance en pourcent d'apparition. Avec spawn_only_once, sert de poids.")]
        public float Chance { get; set; } = 100f;
    }
}
