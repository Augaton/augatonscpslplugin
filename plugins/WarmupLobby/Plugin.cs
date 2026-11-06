using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;

namespace WarmupLobby
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public static Plugin Instance { get; private set; }

        public WarmupService Service { get; private set; }

        public override string Name => "WarmupLobby";

        public override string Description => "Fait combattre les joueurs en attendant le debut du round.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new WarmupService(this);

            ServerEvents.WaitingForPlayers += Service.Start;
            ServerEvents.RoundStarted += Service.Finish;
            ServerEvents.RoundRestarted += Service.Stop;
            PlayerEvents.Dying += Service.OnDying;
            PlayerEvents.DroppingItem += Service.OnDroppingItem;
            PlayerEvents.DroppingAmmo += Service.OnDroppingAmmo;
            PlayerEvents.ThrowingProjectile += Service.OnThrowingProjectile;
            PlayerEvents.SearchingPickup += Service.OnSearchingPickup;
            PlayerEvents.InteractingDoor += Service.OnInteractingDoor;
            PlayerEvents.InteractingElevator += Service.OnInteractingElevator;
            PlayerEvents.InteractingLocker += Service.OnInteractingLocker;
            PlayerEvents.Cuffing += Service.OnCuffing;
            ObjectiveEvents.Completing += Service.OnObjectiveCompleting;

            SelfCheck.Register(Name, "point d'apparition de l'arene", Service.CheckArena);

            PluginDirectory.Register(this);
        }

        public override void Disable()
        {
            ServerEvents.WaitingForPlayers -= Service.Start;
            ServerEvents.RoundStarted -= Service.Finish;
            ServerEvents.RoundRestarted -= Service.Stop;
            PlayerEvents.Dying -= Service.OnDying;
            PlayerEvents.DroppingItem -= Service.OnDroppingItem;
            PlayerEvents.DroppingAmmo -= Service.OnDroppingAmmo;
            PlayerEvents.ThrowingProjectile -= Service.OnThrowingProjectile;
            PlayerEvents.SearchingPickup -= Service.OnSearchingPickup;
            PlayerEvents.InteractingDoor -= Service.OnInteractingDoor;
            PlayerEvents.InteractingElevator -= Service.OnInteractingElevator;
            PlayerEvents.InteractingLocker -= Service.OnInteractingLocker;
            PlayerEvents.Cuffing -= Service.OnCuffing;
            ObjectiveEvents.Completing -= Service.OnObjectiveCompleting;

            Service.Stop();
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.RespawnSeconds = AtLeast("respawn_seconds", Config.RespawnSeconds, 1f);

            if (Config.FirstRole.IsHuman() && Config.SecondRole.IsHuman() && Config.FirstRole.GetFaction() != Config.SecondRole.GetFaction())
                return;

            Logger.Warn("first_role et second_role doivent etre deux roles humains ennemis, valeurs par defaut remises.");

            Config.FirstRole = RoleTypeId.NtfPrivate;
            Config.SecondRole = RoleTypeId.ChaosConscript;
        }
    }
}
