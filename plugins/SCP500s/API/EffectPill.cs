using LabApi.Features.Wrappers;
using UnityEngine;

namespace SCP500s.API
{
    public sealed class EffectPill : PillBase
    {
        public EffectPill(PillDefinition definition, Color glowColor)
            : base(definition, glowColor)
        {
        }

        public override void Consume(Player player) => PayloadExecutor.Apply(player, Definition, Key);
    }
}
