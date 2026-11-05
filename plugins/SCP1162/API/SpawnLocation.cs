using System.ComponentModel;
using MapGeneration;
using UnityEngine;

namespace SCP1162.API
{
    public sealed class SpawnLocation
    {
        [Description("Salle servant de reference (MapGeneration.RoomName). Une salle est tiree au hasard parmi celles qui correspondent. Les couloirs valent tous Unnamed : les distinguer avec zone et shape.")]
        public RoomName Room { get; set; } = RoomName.Lcz173;

        [Description("Zone de la salle (MapGeneration.FacilityZone). None = toutes les zones.")]
        public FacilityZone Zone { get; set; } = FacilityZone.None;

        [Description("Forme de la salle (MapGeneration.RoomShape). Undefined = toutes les formes.")]
        public RoomShape Shape { get; set; } = RoomShape.Undefined;

        [Description("Decalage local par rapport a l'origine de la salle.")]
        public Vector3 Offset { get; set; } = new Vector3(17f, 13f, 3.59f);

        [Description("Rotation locale de l'anomalie, en degres.")]
        public Vector3 Rotation { get; set; } = new Vector3(90f, 0f, 0f);

        [Description("Echelle de l'anomalie.")]
        public Vector3 Scale { get; set; } = new Vector3(1.3f, 0.1f, 1.3f);

        public bool Matches(LabApi.Features.Wrappers.Room room)
        {
            return room.Name == Room
                && (Zone == FacilityZone.None || room.Zone == Zone)
                && (Shape == RoomShape.Undefined || room.Shape == Shape);
        }
    }
}
