using System.Collections.Generic;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using RandomGamemode.API;

namespace RandomGamemode.Gamemodes
{
    public sealed class NightOfTheLivingNerd : GamemodeBase
    {
        private readonly List<string> boosted = new List<string>(4);

        public override string Key => "livingnerd";

        public override string DisplayName => "La nuit du scientifique fou";

        public override IEnumerator<float> Run()
        {
            yield return Timing.WaitForSeconds(3f);

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed)
                    continue;

                if (player.IsSCP)
                {
                    player.SetRole(RoleTypeId.Scientist);
                    player.ClearInventory();
                    player.AddItem(ItemType.GunLogicer);
                    player.AddItem(ItemType.Flashlight);
                    player.AddItem(ItemType.KeycardFacilityManager);
                    player.SetAmmo(ItemType.Ammo762x39, 1000);
                    MoveToSpawn(player, RoleTypeId.Scp939);
                    player.EnableEffect<MovementBoost>(20, 0f);
                    boosted.Add(player.UserId);
                }
                else
                {
                    player.SetRole(RoleTypeId.ClassD);
                    player.AddItem(ItemType.Flashlight);
                    player.AddItem(ItemType.SCP268);
                }
            }

            KillLights(Config.BlackoutDuration);

            yield return Timing.WaitForSeconds(3f);
            Announce(Settings.Description);
        }

        public override void Stop()
        {
            RestoreLights();

            foreach (string userId in boosted)
            {
                Player target = Player.Get(userId);

                if (target is not null && !target.IsDestroyed)
                    target.DisableEffect<MovementBoost>();
            }

            boosted.Clear();
        }
    }
}
