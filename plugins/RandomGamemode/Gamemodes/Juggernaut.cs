using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Text;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;
using UnityEngine;

namespace RandomGamemode.Gamemodes
{
    public sealed class Juggernaut : GamemodeBase
    {
        private string juggernautUserId;

        public override string Key => "juggernaut";

        public override string DisplayName => "Juggernaut";

        public override bool CanStart(int playerCount) => playerCount >= 4;

        public override bool HumansOnly => true;

        protected override RoleTypeId HumanRole => RoleTypeId.NtfSergeant;

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            List<Player> candidates = new List<Player>(32);

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && !player.IsDestroyed)
                    candidates.Add(player);
            }

            if (candidates.Count < 2)
                yield break;

            Player chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            juggernautUserId = chosen.UserId;
            candidates.Remove(chosen);

            int changed = 0;

            foreach (Player hunter in candidates)
            {
                if (hunter.IsDestroyed)
                    continue;

                hunter.SetRole(RoleTypeId.NtfSergeant);
                hunter.ClearInventory();
                hunter.AddItem(ItemType.GunE11SR);
                hunter.AddItem(ItemType.Medkit);
                hunter.AddItem(ItemType.Flashlight);
                hunter.SetAmmo(ItemType.Ammo556x45, 500);

                if (++changed % RoleChangesPerFrame == 0)
                    yield return Timing.WaitForOneFrame;
            }

            chosen.SetRole(RoleTypeId.ChaosRepressor);

            yield return Timing.WaitForSeconds(1.5f);

            Player target = Player.Get(juggernautUserId);

            if (target is null || target.IsDestroyed)
                yield break;

            target.ClearInventory();
            target.AddItem(ItemType.GunLogicer);
            target.AddItem(ItemType.ArmorHeavy);
            target.SetAmmo(ItemType.Ammo762x39, 2000);
            BoostHealth(target, Config.JuggernautHealth);
            ScaleArbiter.Request(target, Interop.Owner, Vector3.one * Config.JuggernautScale);
            target.EnableEffect<DamageReduction>(Config.JuggernautDamageReduction, 0f);

            Announce(Text.JuggernautAnnouncement.Replace("%PLAYER%", SafeText.Nickname(target)));

            yield return Timing.WaitForSeconds(1f);
            Announce(Settings.Description);
        }

        public override void Stop()
        {
            Player target = Player.Get(juggernautUserId);

            if (target is not null && !target.IsDestroyed)
                target.DisableEffect<DamageReduction>();

            juggernautUserId = null;
        }

        public override void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev?.Player is null || string.IsNullOrEmpty(juggernautUserId))
                return;

            if (ev.Player.UserId != juggernautUserId)
                return;

            Announce(Text.JuggernautDefeated.Replace("%PLAYER%", SafeText.Nickname(ev.Player)));
            juggernautUserId = null;
        }
    }
}
