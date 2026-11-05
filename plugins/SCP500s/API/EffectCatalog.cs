using System;
using System.Collections.Generic;
using AugatonLib.Effects;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace SCP500s.API
{
    public static class EffectCatalog
    {
        private static readonly HashSet<string> Negative = new HashSet<string>(StringComparer.Ordinal)
        {
            "AmnesiaItems",
            "AmnesiaVision",
            "Asphyxiated",
            "Bleeding",
            "Blindness",
            "Burned",
            "Concussed",
            "Corroding",
            "Deafened",
            "Decontaminating",
            "Disabled",
            "Ensnared",
            "Exhausted",
            "Flashed",
            "Hemorrhage",
            "Hypothermia",
            "InsufficientLighting",
            "Poisoned",
            "Scanned",
            "Scp207",
            "SeveredHands",
            "Sinkhole",
            "Slowness",
            "SoundtrackMute",
            "Stained",
            "Strangled",
        };

        private static readonly string[] Positive =
        {
            "AntiScp207",
            "BodyshotReduction",
            "DamageReduction",
            "Ghostly",
            "Invigorated",
            "Invisible",
            "MovementBoost",
            "RainbowTaste",
            "Scp1344",
            "Scp1853",
            "Scp207",
            "Vitality",
        };

        private static readonly string[] Harmful =
        {
            "Asphyxiated",
            "Bleeding",
            "Corroding",
            "Decontaminating",
            "Hemorrhage",
            "Hypothermia",
            "Poisoned",
            "Scp207",
            "SeveredHands",
            "Strangled",
        };

        public static void CureNegative(Player player)
        {
            foreach (StatusEffectBase effect in player.ReferenceHub.playerEffectsController.AllEffects)
            {
                if (Negative.Contains(effect.GetType().Name))
                    effect.IsEnabled = false;
            }
        }

        public static void ApplyRandom(Player player, byte intensity, float duration)
        {
            string[] pool = UnityEngine.Random.Range(0, 2) == 0 ? Positive : Harmful;
            string className = pool[UnityEngine.Random.Range(0, pool.Length)];

            if (EffectLookup.TryGet(player, className, out StatusEffectBase effect))
                player.EnableEffect(effect, intensity, duration);
        }
    }
}
