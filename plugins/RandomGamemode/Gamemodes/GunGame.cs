using System.Collections.Generic;
using AugatonLib.Text;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class GunGame : GamemodeBase
    {
        private readonly Dictionary<string, int> tiers = new Dictionary<string, int>();

        public override string Key => "gungame";

        public override string DisplayName => "Gun Game";

        public override bool CanStart(int playerCount) => Config.GunGameLadder.Count > 0;

        public override bool HumansOnly => true;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            tiers.Clear();

            int changed = 0;

            foreach (Player player in ReadyPlayers())
            {
                if (player is null || player.IsDestroyed)
                    continue;

                player.SetRole(RoleTypeId.ClassD);

                if (++changed % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            yield return Timing.WaitForSeconds(1.5f);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive)
                    continue;

                tiers[player.UserId] = 0;
                Equip(player, 0);
            }

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }

        public override void Stop() => tiers.Clear();

        public override void OnPlayerSpawned(PlayerChangedRoleEventArgs ev)
        {
            if (ev?.Player is null || !ev.Player.IsAlive || ev.Player.IsSCP)
                return;

            if (!tiers.TryGetValue(ev.Player.UserId, out int tier))
                return;

            string userId = ev.Player.UserId;

            Delay(0.6f, () =>
            {
                Player target = Player.Get(userId);

                if (target is null || target.IsDestroyed || !target.IsAlive)
                    return;

                Equip(target, tier);
            });
        }

        public override void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev?.Attacker is null || ev.Attacker == ev.Player)
                return;

            string userId = ev.Attacker.UserId;

            if (string.IsNullOrEmpty(userId) || !tiers.TryGetValue(userId, out int tier))
                return;

            int next = tier + 1;

            if (next >= Config.GunGameLadder.Count)
            {
                Announce(Text.GunGameWinner.Replace("%PLAYER%", SafeText.Nickname(ev.Attacker)));
                Plugin.Instance.Registry.ForceRoundEnd();
                return;
            }

            tiers[userId] = next;

            Delay(0.4f, () =>
            {
                Player target = Player.Get(userId);

                if (target is null || target.IsDestroyed || !target.IsAlive)
                    return;

                Equip(target, next);
                Plugin.Instance.Hints.Show(target, Text.GunGameTier.Replace("%TIER%", (next + 1).ToString()), 4f);
            });
        }

        private void Equip(Player player, int tier)
        {
            List<ItemType> ladder = Config.GunGameLadder;

            if (ladder.Count == 0)
                return;

            ItemType weapon = ladder[UnityEngine.Mathf.Clamp(tier, 0, ladder.Count - 1)];

            player.ClearInventory();
            player.AddItem(weapon);
            player.AddItem(ItemType.Medkit);
            player.SetAmmo(ItemType.Ammo556x45, 300);
            player.SetAmmo(ItemType.Ammo762x39, 300);
            player.SetAmmo(ItemType.Ammo9x19, 300);
            BoostHealth(player, Config.GunGameHealth);
        }
    }
}
