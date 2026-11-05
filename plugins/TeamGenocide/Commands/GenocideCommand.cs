using CommandSystem;
using AugatonLib.Commands;

namespace TeamGenocide.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class GenocideCommand : StaffParentCommand
    {
        public GenocideCommand() => LoadGeneratedCommands();

        public override string Command => "teamgenocide";

        public override string[] Aliases => new[] { "tg" };

        public override string Description => "Etat des annonces d'extinction d'equipe.";

        public override string Permission => "teamgenocide.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "TeamGenocide",
                typeof(Plugin), Permission, builder =>
            {
                Config config = Plugin.Instance.Config;
                builder.AppendLine($"  equipes couvertes : {config.Announcements.Count}");
                builder.AppendLine($"  delai d'activation : {config.ActivationDelay:0.#}s");
                builder.AppendLine($"  une annonce par equipe : {(config.AnnounceOncePerTeam ? "oui" : "non")}");
                return true;
            }));
        }
    }
}
