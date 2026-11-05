using System.Collections.Generic;
using MEC;
using PlayerRoles;

namespace SCPReplacer.API
{
    public sealed class ScpToReplace
    {
        public string Name { get; set; }

        public RoleTypeId Role { get; set; }

        public List<string> Volunteers { get; } = new List<string>(8);

        public CoroutineHandle LotteryTimeout { get; set; }

        public bool LotteryRunning { get; set; }
    }
}
