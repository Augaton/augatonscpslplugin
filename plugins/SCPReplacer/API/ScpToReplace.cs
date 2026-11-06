using System.Collections.Generic;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace SCPReplacer.API
{
    public sealed class ScpToReplace
    {
        public string Name { get; set; }

        public RoleTypeId Role { get; set; }

        public Vector3 Size { get; set; } = Vector3.one;

        public string DepartedUserId { get; set; }

        public List<string> Volunteers { get; } = new List<string>(8);

        public CoroutineHandle LotteryTimeout { get; set; }

        public bool LotteryRunning { get; set; }
    }
}
