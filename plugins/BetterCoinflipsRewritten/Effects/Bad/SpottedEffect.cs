using AugatonLib.Text;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class SpottedEffect : ICoinEffect
    {
        private const float ReportSeconds = 4f;

        private readonly float duration;

        public SpottedEffect(float duration)
        {
            this.duration = duration;
        }

        public string Name
        {
            get { return "Spotted"; }
        }

        public string Message
        {
            get { return Plugin.Instance.Translation.SpottedEffectMessage; }
        }

        public bool CanApply(Player player)
        {
            if (player is null || player.IsDestroyed || !player.IsAlive)
                return false;

            foreach (Player other in Player.ReadyList)
            {
                if (other.IsSCP)
                    return true;
            }

            return false;
        }

        public void Execute(Player player)
        {
            Report(player.UserId, Mathf.CeilToInt(duration / ReportSeconds));
        }

        private static void Report(string userId, int remaining)
        {
            Player target = Player.Get(userId);

            if (target is null || target.IsDestroyed || !target.IsAlive)
                return;

            Plugin plugin = Plugin.Instance;
            string name = SafeText.Nickname(target);

            foreach (Player scp in Player.ReadyList)
            {
                if (!scp.IsSCP)
                    continue;

                int distance = Mathf.RoundToInt(Vector3.Distance(scp.Position, target.Position));

                plugin.Hints.Show(
                    scp,
                    plugin.Translation.SpottedScpHint.Replace("{player}", name).Replace("{distance}", distance.ToString()),
                    ReportSeconds + 1f);
            }

            if (remaining > 1)
                Scheduler.Delay(ReportSeconds, delegate { Report(userId, remaining - 1); });
        }
    }
}
