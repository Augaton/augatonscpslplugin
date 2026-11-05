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
        });

        public void OnPlayerDied(PlayerDeathEventArgs ev) => Dispatch(g => g.OnPlayerDied(ev));

        // ChangedRole et non Spawned : Spawned ignore les roles sans point d'apparition, comme SCP-079,
        // et part avant la remise de l'inventaire.
        public void OnPlayerSpawned(PlayerChangedRoleEventArgs ev)
        {
            Guard(() => registry.EnforceHumansOnly(ev?.Player));
            Dispatch(g => g.OnPlayerSpawned(ev));
        }

        public void OnThrownProjectile(PlayerThrewProjectileEventArgs ev) => Dispatch(g => g.OnThrownProjectile(ev));

        public void OnDroppingItem(PlayerDroppingItemEventArgs ev) => Dispatch(g => g.OnDroppingItem(ev));

        public void OnStoppingGenerator(PlayerDeactivatingGeneratorEventArgs ev) => Dispatch(g => g.OnStoppingGenerator(ev));

        public void OnInteractingDoor(PlayerInteractingDoorEventArgs ev) => Dispatch(g => g.OnInteractingDoor(ev));

        public void OnDecontaminating(LczDecontaminationStartingEventArgs ev) => Dispatch(g => g.OnDecontaminating(ev));

        public void OnRespawningTeam(WaveRespawningEventArgs ev) => Dispatch(g => g.OnRespawningTeam(ev));

        public void OnEndingRound(RoundEndingConditionsCheckEventArgs ev) => Dispatch(g => g.OnEndingRound(ev));

        private void Dispatch(Action<GamemodeBase> action)
        {
            GamemodeBase active = registry.Active;

            if (active is null)
                return;

            try
            {
                action(active);
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
