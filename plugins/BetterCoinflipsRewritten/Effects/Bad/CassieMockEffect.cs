using System.Collections.Generic;
using BetterCoinflipsRewritten.API;
using Cassie;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class CassieMockEffect : ICoinEffect
    {
        private const string CooldownKey = "cassie";

        private readonly List<string> lines;
        private readonly List<string> pendingLines;
        private readonly float glitchChance;
        private readonly float jamChance;

        public CassieMockEffect(
            IEnumerable<string> announcements,
            float glitchChance,
            float jamChance)
        {
            this.glitchChance = glitchChance;
            this.jamChance = jamChance;

            lines = new List<string>(4);
            pendingLines = new List<string>(4);

            if (announcements is null)
                return;

            foreach (string line in announcements)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                pendingLines.Add(line);
            }
        }

        public string Name
        {
            get { return "CassieMock"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .CassieMockEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   GlobalCooldown.IsReady(
                       CooldownKey,
                       Plugin.Instance.Config.FacilityEffectCooldown) &&
                   HasPlayableLine();
        }

        public void Execute(Player player)
        {
            float cooldown =
                Plugin.Instance.Config.FacilityEffectCooldown;

            if (!GlobalCooldown.TryConsume(CooldownKey, cooldown))
                return;

            string line = lines[Rng.Next(0, lines.Count)];

            // Announcer.GlitchyMessage force des sous-titres vides : ce payload garde les sous-titres automatiques.
            Announcer.Message(
                new CassieTtsPayload(CassieGlitchifier.Glitchify(line, glitchChance, jamChance)),
                0f,
                0f);
        }

        private bool HasPlayableLine()
        {
            if (pendingLines.Count > 0 &&
                Announcer.LineDatabase != null)
            {
                ValidatePendingLines();
            }

            return lines.Count > 0;
        }

        private void ValidatePendingLines()
        {
            foreach (string line in pendingLines)
            {
                if (Announcer.CalculateDuration(line, default(CassiePlaybackModifiers)) > 0d)
                {
                    lines.Add(line);
                    continue;
                }

                Logger.Warn(
                    "[BetterCoinflipsRewritten] C.A.S.S.I.E. announcement " +
                    "refused by the game, it will never be played: \"" +
                    line +
                    "\".");
            }

            pendingLines.Clear();
        }
    }
}
