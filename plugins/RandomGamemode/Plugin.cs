using System;
using System.Collections.Generic;
using AugatonLib.Caching;
using AugatonLib.Integrations;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using RandomGamemode.API;
using RandomGamemode.Handlers;

namespace RandomGamemode
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public override string Name => "RandomGamemode";

        public override string Description => "Modes de jeu speciaux tires au hasard au debut d'un round.";

        public override string Author => "Zone-Shilari (base: OPGman)";

        public static Plugin Instance { get; private set; }

        public GamemodeRegistry Registry { get; private set; }

        public CooldownTracker VoteCooldowns { get; } = new CooldownTracker();

        public UncomplicatedBridge Bridge { get; private set; }

        private GamemodeHandlers gamemodeHandlers;

        public override void Enable()
        {
            Instance = this;


            Bridge = new UncomplicatedBridge();
            Bridge.Initialise();

            Registry = new GamemodeRegistry(this);
            Registry.Build();

            gamemodeHandlers = new GamemodeHandlers(Registry);

            // ServerEvents.RoundStarted part avant l'attribution des roles.
            CharacterClassManager.OnRoundStarted += gamemodeHandlers.OnRoundStarted;
            ServerEvents.RoundEnded += gamemodeHandlers.OnRoundEnded;
            ServerEvents.RoundRestarted += gamemodeHandlers.OnRoundRestarted;
            ServerEvents.WaitingForPlayers += gamemodeHandlers.OnWaitingForPlayers;
            ServerEvents.RoundEndingConditionsCheck += gamemodeHandlers.OnEndingRound;
            ServerEvents.WaveRespawning += gamemodeHandlers.OnRespawningTeam;
            ServerEvents.LczDecontaminationStarting += gamemodeHandlers.OnDecontaminating;

            PlayerEvents.Joined += gamemodeHandlers.OnJoined;
            PlayerEvents.Left += gamemodeHandlers.OnLeft;
            PlayerEvents.Death += gamemodeHandlers.OnPlayerDied;
            PlayerEvents.ChangedRole += gamemodeHandlers.OnPlayerSpawned;
            PlayerEvents.ThrewProjectile += gamemodeHandlers.OnThrownProjectile;
            PlayerEvents.DroppingItem += gamemodeHandlers.OnDroppingItem;
            PlayerEvents.DeactivatingGenerator += gamemodeHandlers.OnStoppingGenerator;
            PlayerEvents.InteractingDoor += gamemodeHandlers.OnInteractingDoor;

            PluginDirectory.Register(
                this,
                Capability.Hints,
                Capability.FriendlyFire,
                Capability.Scale,
                Capability.Light,
                Capability.DoorLock,
                Capability.Genocide,
                Capability.Bus);
        }

        public override void Disable()
        {
            if (gamemodeHandlers is not null)
            {
                CharacterClassManager.OnRoundStarted -= gamemodeHandlers.OnRoundStarted;
                ServerEvents.RoundEnded -= gamemodeHandlers.OnRoundEnded;
                ServerEvents.RoundRestarted -= gamemodeHandlers.OnRoundRestarted;
                ServerEvents.WaitingForPlayers -= gamemodeHandlers.OnWaitingForPlayers;
                ServerEvents.RoundEndingConditionsCheck -= gamemodeHandlers.OnEndingRound;
                ServerEvents.WaveRespawning -= gamemodeHandlers.OnRespawningTeam;
                ServerEvents.LczDecontaminationStarting -= gamemodeHandlers.OnDecontaminating;

                PlayerEvents.Joined -= gamemodeHandlers.OnJoined;
                PlayerEvents.Left -= gamemodeHandlers.OnLeft;
                PlayerEvents.Death -= gamemodeHandlers.OnPlayerDied;
                PlayerEvents.ChangedRole -= gamemodeHandlers.OnPlayerSpawned;
                PlayerEvents.ThrewProjectile -= gamemodeHandlers.OnThrownProjectile;
                PlayerEvents.DroppingItem -= gamemodeHandlers.OnDroppingItem;
                PlayerEvents.DeactivatingGenerator -= gamemodeHandlers.OnStoppingGenerator;
                PlayerEvents.InteractingDoor -= gamemodeHandlers.OnInteractingDoor;
            }

            Registry?.End(false);
            Hints.Clear();

            PluginDirectory.Unregister(this);

            gamemodeHandlers = null;
            Registry = null;
            Bridge = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.VoteCommandCooldownSeconds = AtLeast("vote_command_cooldown_seconds", Config.VoteCommandCooldownSeconds, 0.5f);

            if (Config.GamemodeChance < 0 || Config.GamemodeChance > 100)
            {
                Logger.Warn($"GamemodeChance ({Config.GamemodeChance}) hors bornes 0-100, remis a 10.");
                Config.GamemodeChance = 10;
            }

            if (!IsValidFormat(Config.StartText))
            {
                Logger.Warn("StartText n'est pas un format valide ({0} attendu), remis par defaut.");
                Config.StartText = new Config().StartText;
            }

            if (!IsValidFormat(Config.EndText))
            {
                Logger.Warn("EndText n'est pas un format valide ({0} attendu), remis par defaut.");
                Config.EndText = new Config().EndText;
            }

            if (Config.Gamemodes is null)
            {
                Logger.Warn("Gamemodes est vide, reglages par defaut appliques.");
                Config.Gamemodes = new Config().Gamemodes;
            }

            if (Config.BlackoutDuration < 5f)
            {
                Logger.Warn($"BlackoutDuration ({Config.BlackoutDuration}) trop bas, remis a 60.");
                Config.BlackoutDuration = 60f;
            }

            if (Config.RandomizerItemCount < 0)
            {
                Logger.Warn($"RandomizerItemCount ({Config.RandomizerItemCount}) negatif, ramene a 0.");
                Config.RandomizerItemCount = 0;
            }

            if (Config.RandomizerItemCount > 7)
            {
                Logger.Warn($"RandomizerItemCount ({Config.RandomizerItemCount}) depasse la taille d'inventaire, ramene a 7.");
                Config.RandomizerItemCount = 7;
            }

            if (Config.GunGameLadder is null)
                Config.GunGameLadder = new List<ItemType>();

            if (Config.GunGameLadder.Count == 0)
                Logger.Warn("GunGameLadder est vide, le mode Gun Game ne pourra pas demarrer.");

            if (Config.RandomizerItemPool is null)
                Config.RandomizerItemPool = new List<ItemType>();

            if (Config.RandomizerItemPool.Count == 0)
                Logger.Warn("RandomizerItemPool est vide, le mode Randomiseur ne donnera aucun objet.");

            if (Config.PillPartyCustomChance < 0 || Config.PillPartyCustomChance > 100)
            {
                Logger.Warn($"PillPartyCustomChance ({Config.PillPartyCustomChance}) hors bornes 0-100, remis a 70.");
                Config.PillPartyCustomChance = 70;
            }

            Config.HideAndSeekHideSeconds = AtLeast("hide_and_seek_hide_seconds", Config.HideAndSeekHideSeconds, 5f);
            Config.HideAndSeekHuntSeconds = AtLeast("hide_and_seek_hunt_seconds", Config.HideAndSeekHuntSeconds, 30f);
            Config.EvacuationDelaySeconds = AtLeast("evacuation_delay_seconds", Config.EvacuationDelaySeconds, 5f);

            if (Config.JuggernautHealth < 1f)
            {
                Logger.Warn($"JuggernautHealth ({Config.JuggernautHealth}) trop bas, remis a 2500.");
                Config.JuggernautHealth = 2500f;
            }
        }

        private static bool IsValidFormat(string text)
        {
            if (text is null)
                return false;

            try
            {
                _ = string.Format(text, string.Empty);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
