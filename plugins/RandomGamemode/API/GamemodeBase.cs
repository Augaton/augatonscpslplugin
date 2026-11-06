using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Integrations;
using AugatonLib.Spawning;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace RandomGamemode.API
{
    public abstract class GamemodeBase
    {
        private readonly List<CoroutineHandle> handles = new List<CoroutineHandle>(8);
        private readonly HashSet<string> boostedHealth = new HashSet<string>();

        public abstract string Key { get; }

        public abstract string DisplayName { get; }

        // Relu a chaque usage : LoadConfigs remplace l'objet Config au rechargement.
        public GamemodeSettings Settings
        {
            get
            {
                Dictionary<string, GamemodeSettings> entries = Config.Gamemodes;

                if (entries.TryGetValue(Key, out GamemodeSettings settings) && settings is not null)
                    return settings;

                // Un config.yml plus ancien que le mode ne le connait pas : il herite de ses vrais reglages.
                if (!new Config().Gamemodes.TryGetValue(Key, out settings))
                    settings = new GamemodeSettings { Description = DisplayName };

                entries[Key] = settings;
                return settings;
            }
        }

        protected static Config Config => Plugin.Instance.Config;

        protected static Translation Text => Plugin.Instance.Translation;

        protected static UncomplicatedBridge Bridge => Plugin.Instance.Bridge;

        // Vrai quand le mode decide seul de la fin du round : le jeu la declarerait des qu'un camp reste seul.
        protected bool KeepRoundOpen { get; set; }

        public virtual bool CanStart(int playerCount) => true;

        public virtual bool HumansOnly => false;

        public virtual bool PausesWorldEvents => HumansOnly;

        protected virtual RoleTypeId HumanRole => RoleTypeId.ClassD;

        public abstract IEnumerator<float> Run();

        public virtual void Stop()
        {
        }

        public virtual void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
        }

        public virtual void OnPlayerEscaped(PlayerEscapedEventArgs ev)
        {
        }

        public virtual void OnPlayerDied(PlayerDeathEventArgs ev)
        {
        }

        public virtual void OnPlayerSpawned(PlayerChangedRoleEventArgs ev)
        {
        }

        public virtual void OnThrownProjectile(PlayerThrewProjectileEventArgs ev)
        {
        }

        public virtual void OnDroppingItem(PlayerDroppingItemEventArgs ev)
        {
        }

        public virtual void OnStoppingGenerator(PlayerDeactivatingGeneratorEventArgs ev)
        {
        }

        public virtual void OnInteractingDoor(PlayerInteractingDoorEventArgs ev)
        {
        }

        public virtual void OnDecontaminating(LczDecontaminationStartingEventArgs ev)
        {
        }

        public virtual void OnRespawningTeam(WaveRespawningEventArgs ev)
        {
        }

        public virtual void OnEndingRound(RoundEndingConditionsCheckEventArgs ev)
        {
            if (ev is not null && KeepRoundOpen)
                ev.CanEnd = false;
        }

        protected void ConvertScpsToHumans()
        {
            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsSCP)
                    continue;

                player.SetRole(HumanRole);
            }
        }

        internal void EnforceHumanRole(Player player)
        {
            if (player is null || player.IsDestroyed || !player.IsSCP)
                return;

            string userId = player.UserId;

            if (string.IsNullOrEmpty(userId))
                return;

            Delay(0.5f, () =>
            {
                Player target = Player.Get(userId);

                if (target is null || target.IsDestroyed || !target.IsSCP)
                    return;

                target.SetRole(HumanRole);
            });
        }

        protected void Track(CoroutineHandle handle) => handles.Add(handle);

        protected CoroutineHandle Delay(float seconds, System.Action action)
        {
            CoroutineHandle handle = Timing.CallDelayed(seconds, () =>
            {
                try
                {
                    action();
                }
                catch (System.Exception e)
                {
                    Logger.Error($"Mode {Key} : {e}");
                }
            });

            handles.Add(handle);
            return handle;
        }

        internal void KillTrackedCoroutines()
        {
            KeepRoundOpen = false;

            foreach (CoroutineHandle handle in handles)
                Timing.KillCoroutines(handle);

            handles.Clear();
        }

        protected void BoostHealth(Player player, float health)
        {
            boostedHealth.Add(player.UserId);
            player.MaxHealth = health;
            player.Health = health;
        }

        internal void RestoreHealth()
        {
            foreach (string userId in boostedHealth)
            {
                Player player = Player.Get(userId);

                if (player is null || player.IsDestroyed || !(player.RoleBase is IHealthbarRole role))
                    continue;

                player.MaxHealth = role.MaxHealth;
            }

            boostedHealth.Clear();
        }

        protected static List<Player> ReadyPlayers()
        {
            List<Player> players = new List<Player>(32);

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && !player.IsDestroyed)
                    players.Add(player);
            }

            return players;
        }

        protected static int CountAlive()
        {
            int alive = 0;

            foreach (Player player in Player.ReadyList)
            {
                if (player is not null && player.IsAlive)
                    alive++;
            }

            return alive;
        }

        protected static void EndRound(string message)
        {
            Announce(message);
            Plugin.Instance.Registry.ForceRoundEnd();
        }

        protected static void MoveToSpawn(Player player, RoleTypeId role)
        {
            if (RoleSpawn.TryGetPosition(role, out Vector3 position))
                player.Position = position;
            else
                Logger.Debug($"Aucun point d'apparition pour {role}, {player.Nickname} reste sur place.", Config.Debug);
        }

        protected static void KillLights(float duration)
        {
            LightArbiter.Blackout(Interop.Owner, duration, LightArbiter.AllZones);
        }

        protected static void RestoreLights()
        {
            LightArbiter.Restore(Interop.Owner);
        }

        protected static void Announce(string message, ushort duration = 15)
        {
            if (string.IsNullOrEmpty(message))
                return;

            Server.SendBroadcast(message, duration);
        }
    }
}
