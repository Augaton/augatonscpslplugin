using System;
using System.Collections.Generic;
using System.Reflection;
using AugatonLib.Spawning;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace WarmupLobby
{
    public sealed class WarmupService
    {
        // Le compteur de morts du jeu n'a pas de mutateur public, et il n'est remis a zero qu'au chargement
        // de la scene : sans cela, les morts de l'echauffement entreraient dans le bilan du round.
        private static readonly PropertyInfo RoundDeaths = typeof(RoundSummary).GetProperty(nameof(RoundSummary.Kills));

        private readonly Plugin plugin;
        private readonly List<Player> waiting = new List<Player>(16);

        private CoroutineHandle loop;

        public WarmupService(Plugin plugin) => this.plugin = plugin;

        public bool IsRunning { get; private set; }

        private Config Config => plugin.Config;

        public void Start()
        {
            Stop();

            IsRunning = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public void Stop()
        {
            if (IsRunning)
                Timing.KillCoroutines(loop);

            IsRunning = false;
            waiting.Clear();
        }

        // ServerEvents.RoundStarted part avant RoleAssigner, qui ecarte tout joueur vivant : un joueur encore
        // en vie ici ne recevrait aucun role. Chacun repasse donc spectateur avant l'attribution.
        public void Finish()
        {
            if (!IsRunning)
                return;

            Stop();

            foreach (Player player in Player.ReadyList)
                Dismiss(player);

            foreach (Ragdoll ragdoll in new List<Ragdoll>(Ragdoll.List))
                ragdoll.Destroy();

            ResetDeathCounter();
        }

        public string CheckArena()
        {
            return RoleSpawn.TryGetPosition(Config.ArenaSpawnRole, out _) ? null : $"pas de point d'apparition pour {Config.ArenaSpawnRole}";
        }

        // Un objet lache ou une porte ouverte resteraient pour le vrai round : l'echauffement ne touche a rien.
        public void OnDying(PlayerDyingEventArgs ev)
        {
            try
            {
                if (IsRunning && ev?.Player is not null)
                    ev.Player.ClearInventory();
            }
            catch (Exception e)
            {
                Logger.Error($"OnDying: {e}");
            }
        }

        public void OnDroppingItem(PlayerDroppingItemEventArgs ev) => Deny(ev);

        public void OnDroppingAmmo(PlayerDroppingAmmoEventArgs ev) => Deny(ev);

        public void OnThrowingProjectile(PlayerThrowingProjectileEventArgs ev) => Deny(ev);

        public void OnSearchingPickup(PlayerSearchingPickupEventArgs ev) => Deny(ev);

        public void OnInteractingDoor(PlayerInteractingDoorEventArgs ev) => Deny(ev);

        public void OnInteractingElevator(PlayerInteractingElevatorEventArgs ev) => Deny(ev);

        private void Deny(ICancellableEvent ev)
        {
            if (IsRunning && ev is not null)
                ev.IsAllowed = false;
        }

        private IEnumerator<float> Loop()
        {
            while (IsRunning && !Round.IsRoundStarted)
            {
                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"WarmupService.Tick: {e}");
                }

                yield return Timing.WaitForSeconds(Config.RespawnSeconds);
            }
        }

        private void Tick()
        {
            if (Player.Count < Config.MinPlayers)
                return;

            int first = 0;
            int second = 0;

            waiting.Clear();

            foreach (Player player in Player.ReadyList)
            {
                if (player.IsDummy)
                    continue;

                if (player.Role == Config.FirstRole)
                    first++;
                else if (player.Role == Config.SecondRole)
                    second++;
                else if (player.Role == RoleTypeId.Spectator || player.Role == RoleTypeId.None)
                    waiting.Add(player);
            }

            foreach (Player player in waiting)
            {
                bool joinsFirst = first <= second;

                Spawn(player, joinsFirst ? Config.FirstRole : Config.SecondRole);

                if (joinsFirst)
                    first++;
                else
                    second++;
            }
        }

        private void Spawn(Player player, RoleTypeId role)
        {
            player.SetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.All);

            if (RoleSpawn.TryGetPosition(Config.ArenaSpawnRole, out Vector3 position))
                player.Position = position;

            plugin.Hints.Show(player, plugin.Translation.Spawned, Config.HintDuration);
        }

        // Un joueur en echec ne doit pas priver les suivants de leur role.
        private void Dismiss(Player player)
        {
            try
            {
                if (player.Role != Config.FirstRole && player.Role != Config.SecondRole)
                    return;

                player.ClearInventory();
                player.SetRole(RoleTypeId.Spectator, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.All);
            }
            catch (Exception e)
            {
                Logger.Error($"Finish ({player.Nickname}): {e}");
            }
        }

        private static void ResetDeathCounter()
        {
            try
            {
                RoundDeaths.SetValue(null, 0);
            }
            catch (Exception e)
            {
                Logger.Error($"ResetDeathCounter: {e}");
            }
        }
    }
}
