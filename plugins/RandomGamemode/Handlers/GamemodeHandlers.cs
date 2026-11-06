using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using RandomGamemode.API;

namespace RandomGamemode.Handlers
{
    public sealed class GamemodeHandlers
    {
        private readonly GamemodeRegistry registry;

        public GamemodeHandlers(GamemodeRegistry registry) => this.registry = registry;

        public void OnRoundStarted() => Guard(() => registry.OnRoundStarted());

        public void OnRoundEnded(RoundEndedEventArgs ev) => Guard(() => registry.End(true));

        public void OnRoundRestarted() => Guard(() => registry.End(false));

        public void OnWaitingForPlayers() => Guard(() =>
        {
            registry.End(false);
            Plugin.Instance?.Bridge?.Refresh();
            registry.PrepareVote();
        });

        public void OnJoined(PlayerJoinedEventArgs ev) => Guard(() => registry.ShowVote(ev?.Player));

        public void OnLeft(PlayerLeftEventArgs ev)
        {
            string userId = ev?.Player?.UserId;

            Guard(() => registry.Vote.Retract(userId));

            if (userId is not null)
                Dispatch((g, e) => g.OnPlayerLeft(e), userId);
        }

        public void OnPlayerDied(PlayerDeathEventArgs ev) => Dispatch((g, e) => g.OnPlayerDied(e), ev);

        // ChangedRole et non Spawned : Spawned ignore les roles sans point d'apparition, comme SCP-079,
        // et part avant la remise de l'inventaire.
        public void OnPlayerSpawned(PlayerChangedRoleEventArgs ev)
        {
            Guard(() => registry.EnforceHumansOnly(ev?.Player));
            Dispatch((g, e) => g.OnPlayerSpawned(e), ev);
        }

        public void OnPlayerHurting(PlayerHurtingEventArgs ev) => Dispatch((g, e) => g.OnPlayerHurting(e), ev);

        public void OnPlayerEscaped(PlayerEscapedEventArgs ev) => Dispatch((g, e) => g.OnPlayerEscaped(e), ev);

        public void OnThrownProjectile(PlayerThrewProjectileEventArgs ev) => Dispatch((g, e) => g.OnThrownProjectile(e), ev);

        public void OnDroppingItem(PlayerDroppingItemEventArgs ev) => Dispatch((g, e) => g.OnDroppingItem(e), ev);

        public void OnStoppingGenerator(PlayerDeactivatingGeneratorEventArgs ev) => Dispatch((g, e) => g.OnStoppingGenerator(e), ev);

        public void OnInteractingDoor(PlayerInteractingDoorEventArgs ev) => Dispatch((g, e) => g.OnInteractingDoor(e), ev);

        public void OnDecontaminating(LczDecontaminationStartingEventArgs ev) => Dispatch((g, e) => g.OnDecontaminating(e), ev);

        public void OnRespawningTeam(WaveRespawningEventArgs ev) => Dispatch((g, e) => g.OnRespawningTeam(e), ev);

        public void OnEndingRound(RoundEndingConditionsCheckEventArgs ev)
        {
            Dispatch((g, e) => g.OnEndingRound(e), ev);
            registry.ApplyForcedEnd(ev);
        }

        private void Dispatch<T>(Action<GamemodeBase, T> action, T ev)
        {
            GamemodeBase active = registry.Active;

            if (active is null)
                return;

            try
            {
                action(active, ev);
            }
            catch (Exception e)
            {
                Logger.Error($"Mode {active.Key} : {e}");
            }
        }

        private static void Guard(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error($"GamemodeHandlers: {e}");
            }
        }
    }
}
