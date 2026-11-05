using System;
using AugatonLib.World;
using BetterCoinflipsRewritten.API;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class MiceInvasionEffect : ICoinEffect
    {
        private const string CooldownKey = "mice";

        private readonly byte miceType;

        public MiceInvasionEffect(byte miceType)
        {
            this.miceType = miceType;
        }

        public string Name
        {
            get { return "MiceInvasion"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .MiceInvasionEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   GlobalCooldown.IsReady(
                       CooldownKey,
                       Plugin.Instance.Config.FacilityEffectCooldown);
        }

        public void Execute(Player player)
        {
            float cooldown =
                Plugin.Instance.Config.FacilityEffectCooldown;

            if (!GlobalCooldown.TryConsume(CooldownKey, cooldown))
                return;

            if (!Mice.TrySpawn(miceType))
                throw new InvalidOperationException("Mice type " + miceType + " cannot be spawned.");
        }
    }
}
