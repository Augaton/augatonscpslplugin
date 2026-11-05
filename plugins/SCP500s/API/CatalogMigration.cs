using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SCP500s.API
{
    public static class CatalogMigration
    {
        private static readonly Dictionary<string, string> LegacyPillEffects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["shadow"] = "Ghostly:1:7",
            ["sonic"] = "MovementBoost:30:7",
            ["super"] = "Invisible:1:7;SilentWalk:1:7;MovementBoost:20:7",
            ["vitality"] = "Vitality:1:20",
            ["adrenaline"] = "DamageReduction:50:15;MovementBoost:15:15",
            ["surgeon"] = "Scp1853:1:30",
            ["juggernaut"] = "BodyshotReduction:50:60",
            ["nightvision"] = "NightVision:1:60;FocusedVision:1:60",
            ["feather"] = "Lightweight:1:30;MovementBoost:10:30",
            ["regeneration"] = "AnomalousRegeneration:1:25",
            ["cola"] = "Scp207:2:0",
            ["anticola"] = "AntiScp207:2:0",
            ["fog"] = "FogControl:2:60",
        };

        private static readonly Dictionary<string, string> LegacyPillDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["shadow"] = "Vous rend invisible quelques secondes.",
            ["vitality"] = "Regeneration continue pendant vingt secondes.",
            ["surgeon"] = "Vos soins deviennent instantanes.",
            ["juggernaut"] = "Vie maximale doublee et resistance aux tirs.",
            ["regeneration"] = "Votre chair se recompose toute seule.",
            ["cola"] = "Les effets d'une bouteille de SCP-207, jusqu'a la mort.",
            ["anticola"] = "L'inverse exact d'une bouteille de SCP-207, jusqu'a la mort.",
            ["fog"] = "Noie le site dans le brouillard a vos yeux.",
        };

        private static readonly Dictionary<string, string> LegacyOutcomeEffects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["voile"] = "Ghostly:1:8;SilentWalk:1:8",
            ["nyctalope"] = "NightVision:1:60;FocusedVision:1:60",
            ["cuirasse"] = "BodyshotReduction:50:45;DamageReduction:40:45",
            ["regeneration"] = "AnomalousRegeneration:1:25;Vitality:1:25",
            ["cola"] = "Scp207:2:0",
            ["anticola"] = "AntiScp207:2:0",
            ["brouillard"] = "FogControl:2:60",
        };

        private static readonly Dictionary<string, ChaosCategory> LegacyOutcomeCategories = new Dictionary<string, ChaosCategory>(StringComparer.OrdinalIgnoreCase)
        {
            ["anticola"] = ChaosCategory.Bad,
            ["brouillard"] = ChaosCategory.Neutral,
        };

        private static readonly Dictionary<string, string> LegacyMessages = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["<color=#FFA500>Plus personne ne vous voit.</color>"] = Messages.Shadow,
            ["<color=#7CFC00>Vos blessures se referment lentement.</color>"] = Messages.Vitality,
            ["<color=#40E0D0>Vos soins sont desormais instantanes.</color>"] = Messages.Surgeon,
            ["<color=#FF1493>Un gout familier de SCP-207. Ca ne s'arretera plus.</color>"] = Messages.Cola,
            ["<color=#C71585>Le SCP-207 a tourne. Definitivement.</color>"] = Messages.AntiCola,
        };

        private static readonly string[] LegacyRandomOnly =
        {
            "anticola",
            "cola",
            "lumien",
            "xerneas",
            "swap",
            "summon",
            "wave",
            "blackout",
            "tantrum",
            "contagion",
            "disguise",
            "prism",
            "fog",
        };

        public static List<PillDefinition> UpgradePills(List<PillDefinition> current)
        {
            if (current is null)
                return null;

            Dictionary<string, PillDefinition> latest = new Dictionary<string, PillDefinition>(StringComparer.OrdinalIgnoreCase);

            foreach (PillDefinition pill in DefaultPills.Create())
                latest[pill.Key] = pill;

            foreach (PillDefinition pill in current)
            {
                if (pill is null || string.IsNullOrEmpty(pill.Key) || !latest.TryGetValue(pill.Key, out PillDefinition target))
                    continue;

                if (LegacyPillEffects.TryGetValue(pill.Key, out string effects) && Signature(pill.Effects) == effects)
                    CopyMechanics(target, pill);

                if (LegacyPillDescriptions.TryGetValue(pill.Key, out string description) && pill.Description == description)
                    pill.Description = target.Description;

                pill.Message = UpgradeMessage(pill.Message);
            }

            return current;
        }

        public static List<ChaosOutcome> UpgradeOutcomes(List<ChaosOutcome> current)
        {
            if (current is null)
                return null;

            Dictionary<string, ChaosOutcome> latest = new Dictionary<string, ChaosOutcome>(StringComparer.OrdinalIgnoreCase);

            foreach (ChaosOutcome outcome in DefaultChaosOutcomes.Create())
                latest[outcome.Key] = outcome;

            foreach (ChaosOutcome outcome in current)
            {
                if (outcome is null || string.IsNullOrEmpty(outcome.Key) || !latest.TryGetValue(outcome.Key, out ChaosOutcome target))
                    continue;

                if (LegacyOutcomeEffects.TryGetValue(outcome.Key, out string effects) && Signature(outcome.Effects) == effects)
                {
                    CopyMechanics(target, outcome);

                    if (LegacyOutcomeCategories.TryGetValue(outcome.Key, out ChaosCategory legacy) && outcome.Category == legacy)
                        outcome.Category = target.Category;
                }

                outcome.Message = UpgradeMessage(outcome.Message);
            }

            return current;
        }

        public static List<string> UpgradeRandomOnly(List<string> current)
        {
            if (current is null || current.Count != LegacyRandomOnly.Length)
                return current;

            HashSet<string> keys = new HashSet<string>(current, StringComparer.OrdinalIgnoreCase);

            foreach (string legacy in LegacyRandomOnly)
            {
                if (!keys.Contains(legacy))
                    return current;
            }

            return DefaultPills.RandomOnlyKeys();
        }

        private static void CopyMechanics(EffectPayload source, EffectPayload target)
        {
            target.Effects = source.Effects;
            target.RegenerationPerSecond = source.RegenerationPerSecond;
            target.RegenerationSeconds = source.RegenerationSeconds;
        }

        private static string UpgradeMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            return LegacyMessages.TryGetValue(message, out string upgraded) ? upgraded : message;
        }

        private static string Signature(List<EffectEntry> effects)
        {
            if (effects is null || effects.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();

            foreach (EffectEntry effect in effects)
            {
                if (effect is null)
                    continue;

                if (builder.Length > 0)
                    builder.Append(';');

                builder.Append(effect.Type)
                    .Append(':')
                    .Append(effect.Intensity)
                    .Append(':')
                    .Append(effect.Duration.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }
    }
}
