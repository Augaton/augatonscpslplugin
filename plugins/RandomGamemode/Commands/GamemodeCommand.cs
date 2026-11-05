using System;
using System.Text;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using RandomGamemode.API;

namespace RandomGamemode.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class GamemodeCommand : StaffParentCommand
    {
        internal const string Node = "randomgamemode.manage";

        public GamemodeCommand() => LoadGeneratedCommands();

        public override string Command => "gamemode";

        public override string[] Aliases => new[] { "gm", "forcegamemode" };

        public override string Description => "Gestion des modes de jeu speciaux.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new ListSubCommand());
            RegisterCommand(new ForceSubCommand());
            RegisterCommand(new StartSubCommand());
            RegisterCommand(new StopSubCommand());
        }
    }

    public abstract class GamemodeSubCommand : StaffCommand
    {
        public override string Permission => GamemodeCommand.Node;

        protected abstract bool Run(Plugin plugin, ArraySegment<string> arguments, ICommandSender sender, out string response);

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null)
            {
                response = "Plugin indisponible.";
                return false;
            }

            return Run(plugin, arguments, sender, out response);
        }
    }

    public sealed class ListSubCommand : GamemodeSubCommand
    {
        public override string Command => "list";

        public override string[] Aliases => new[] { "l" };

        public override string Description => "Liste les modes disponibles, leur poids et leur etat.";

        protected override bool Run(Plugin plugin, ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder builder = StringBuilderPool.Shared.Rent();

            try
            {
                builder.AppendLine("Modes de jeu :");

                int players = Player.Count;

                foreach (GamemodeBase gamemode in plugin.Registry.All)
                {
                    GamemodeSettings settings = gamemode.Settings;
                    string state = settings.IsEnabled ? "actif" : "desactive";
                    string usable = gamemode.CanStart(players) ? string.Empty : "  [prerequis manquants]";

                    builder.AppendLine(
                        $"  {gamemode.Key}  [{state}]  poids {settings.Weight}, " +
                        $"min {settings.MinPlayers} joueurs  -  {gamemode.DisplayName}{usable}");
                }

                builder.AppendLine();
                builder.AppendLine(
                    $"Uncomplicated : {plugin.Bridge.Roles.Count} roles, {plugin.Bridge.Items.Count} objets, " +
                    $"{plugin.Bridge.Teams.Count} equipes au catalogue.");

                builder.AppendLine(
                    $"  application - roles {(plugin.Bridge.RolesAvailable ? "OK" : "indisponible")}, " +
                    $"objets {(plugin.Bridge.ItemsAvailable ? "OK" : "indisponible")}, " +
                    $"equipes {(plugin.Bridge.TeamsAvailable ? "OK" : "indisponible")}");

                response = builder.ToString();
                return true;
            }
            finally
            {
                StringBuilderPool.Shared.Return(builder);
            }
        }
    }

    public sealed class ForceSubCommand : GamemodeSubCommand
    {
        public override string Command => "force";

        public override string[] Aliases => new[] { "f" };

        public override string Description => "Force un mode pour le prochain round. Usage : gamemode force <cle|none>";

        protected override bool Run(Plugin plugin, ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage : gamemode force <cle|none>";
                return false;
            }

            string key = arguments.At(0);

            if (string.Equals(key, "none", StringComparison.OrdinalIgnoreCase))
            {
                plugin.Registry.Forced = null;
                Audit(sender, plugin.Name, "a annule le forcage de mode");

                response = plugin.Translation.GamemodeCleared;
                return true;
            }

            GamemodeBase gamemode = plugin.Registry.Find(key);

            if (gamemode is null)
            {
                response = plugin.Translation.UnknownGamemode.Replace("%KEY%", key);
                return false;
            }

            plugin.Registry.Forced = gamemode.Key;
            Audit(sender, plugin.Name, $"a force le mode \"{gamemode.Key}\" pour le prochain round");

            response = plugin.Translation.GamemodeForced.Replace("%KEY%", gamemode.Key);
            return true;
        }
    }

    public sealed class StartSubCommand : GamemodeSubCommand
    {
        public override string Command => "start";

        public override string[] Aliases => new[] { "s" };

        public override string Description => "Demarre un mode immediatement. Usage : gamemode start <cle>";

        protected override bool Run(Plugin plugin, ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage : gamemode start <cle>";
                return false;
            }

            if (!Round.IsRoundInProgress)
            {
                response = plugin.Translation.RoundNotStarted;
                return false;
            }

            if (plugin.Registry.Active is not null)
            {
                response = plugin.Translation.AlreadyRunning;
                return false;
            }

            string key = arguments.At(0);
            GamemodeBase gamemode = plugin.Registry.Find(key);

            if (gamemode is null)
            {
                response = plugin.Translation.UnknownGamemode.Replace("%KEY%", key);
                return false;
            }

            if (!gamemode.CanStart(Player.Count))
            {
                response = $"Prerequis manquants pour le mode \"{gamemode.Key}\", voir gamemode list.";
                return false;
            }

            plugin.Registry.Begin(gamemode);
            Audit(sender, plugin.Name, $"a demarre le mode \"{gamemode.Key}\" en cours de round");

            response = plugin.Translation.GamemodeStarted.Replace("%KEY%", gamemode.Key);
            return true;
        }
    }

    public sealed class StopSubCommand : GamemodeSubCommand
    {
        public override string Command => "stop";

        public override string Description => "Arrete le mode en cours.";

        protected override bool Run(Plugin plugin, ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (plugin.Registry.Active is null)
            {
                response = plugin.Translation.NoActiveGamemode;
                return false;
            }

            Audit(sender, plugin.Name, $"a arrete le mode \"{plugin.Registry.Active.Key}\"");
            plugin.Registry.End(true);

            response = plugin.Translation.GamemodeStopped;
            return true;
        }
    }
}
