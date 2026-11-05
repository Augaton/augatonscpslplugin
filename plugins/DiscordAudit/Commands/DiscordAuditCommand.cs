using System;
using AugatonLib.Commands;
using CommandSystem;

namespace DiscordAudit.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class DiscordAuditCommand : StaffParentCommand
    {
        internal const string Node = "discordaudit.manage";

        public DiscordAuditCommand() => LoadGeneratedCommands();

        public override string Command => "discordaudit";

        public override string[] Aliases => new[] { "daudit" };

        public override string Description => "Journal staff vers Discord.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("DiscordAudit", typeof(Plugin), Permission, builder =>
            {
                WebhookSender sender = Plugin.Instance.Sender;

                builder.AppendLine($"  webhook : {(sender.IsConfigured ? "configure" : "absent ou invalide")}");
                builder.AppendLine($"  actions en attente : {sender.QueuedLines}");
                builder.AppendLine($"  dernier echec : {sender.LastFailure ?? "aucun"}");
                return true;
            }));
            RegisterCommand(new TestCommand());
        }
    }

    public sealed class TestCommand : StaffCommand
    {
        public override string Command => "test";

        public override string[] Aliases => new[] { "t" };

        public override string Description => "Envoie une ligne de test vers Discord.";

        public override string Permission => DiscordAuditCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Sender.IsConfigured)
            {
                response = "Aucun webhook valide dans la config.";
                return false;
            }

            Audit(sender, "DiscordAudit", "a envoye un message de test");

            response = "Ligne de test mise en file.";
            return true;
        }
    }
}
