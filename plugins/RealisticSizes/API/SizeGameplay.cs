using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;

namespace RealisticSizes.API
{
    public static class SizeGameplay
    {
        // Le jeu retire ces effets et remet la vie a la base du role a chaque changement de role,
        // donc Apply n'a besoin d'aucun nettoyage de son cote.
        public static void Apply(Player player, float height, Config config)
        {
            if (!config.EnableSizeGameplay || !player.IsHuman)
                return;

            player.DisableEffect<MovementBoost>();
            player.DisableEffect<Slowness>();

            float tenths = (height - 1f) * 10f;

            ApplySpeed(player, -tenths * config.SpeedPercentPerTenth, config.MaxSpeedPercent);
            ApplyHealth(player, tenths * config.HealthPercentPerTenth, config.MaxHealthPercent);
        }

        public static void Clear(Player player, Config config)
        {
            if (!config.EnableSizeGameplay || !player.IsHuman)
                return;

            player.DisableEffect<MovementBoost>();
            player.DisableEffect<Slowness>();
            ApplyHealth(player, 0f, 0f);
        }

        private static void ApplySpeed(Player player, float percent, float limit)
        {
            int rounded = Mathf.RoundToInt(Mathf.Clamp(percent, -limit, limit));

            if (rounded > 0)
                player.EnableEffect<MovementBoost>((byte)rounded);
            else if (rounded < 0)
                player.EnableEffect<Slowness>((byte)-rounded);
        }

        private static void ApplyHealth(Player player, float percent, float limit)
        {
            if (!(player.RoleBase is IHealthbarRole role) || player.MaxHealth <= 0f)
                return;

            float ratio = player.Health / player.MaxHealth;
            float change = Mathf.Clamp(percent, -limit, limit);

            player.MaxHealth = role.MaxHealth * (1f + (change / 100f));
            player.Health = player.MaxHealth * ratio;
        }
    }
}
