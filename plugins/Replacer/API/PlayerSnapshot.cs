using System.Collections.Generic;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;

namespace Replacer.API
{
    public sealed class PlayerSnapshot
    {
        public string Nickname { get; set; }

        public RoleTypeId Role { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public float Health { get; set; }

        public float MaxHealth { get; set; }

        public float HumeShield { get; set; }

        public Team Team { get; set; }

        public bool IsDeferred { get; set; }

        public List<Player> Captives { get; } = new List<Player>(2);

        public float ArtificialHealth { get; set; }

        public bool IsDisarmed { get; set; }

        public Player Disarmer { get; set; }

        public List<EffectSnapshot> Effects { get; } = new List<EffectSnapshot>();

        public bool IsScp079 { get; set; }

        public int Scp079Experience { get; set; }

        public float Scp079Energy { get; set; }
    }
}
