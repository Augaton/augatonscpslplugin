using System;
using CommandSystem;
using AugatonLib.Commands;
using AugatonLib.FriendlyFire;

namespace AutoFFToggle.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class AutoFFCommand : StaffParentCommand
    {
        public AutoFFCommand() => LoadGeneratedCommands();

        public override string Command => "autoff";

        public override string[] Aliases => new[] { "ff" };

        public override string Description => "Gestion du tir allie automatique.";

        public override string Permission => "autofftoggle.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "AutoFFToggle",
                typeof(Plugin),
                Permission,
                builder =>
                {
                    builder.AppendLine($"  {FriendlyFireArbiter.Describe()}");
                    return true;
                }));

            RegisterCommand(new SetCommand());
        }
    }

    public sealed class SetCommand : StaffCommand
    {
        public override string Command => "set";

        public override string[] Aliases => Array.Empty<string>();

        public override string Description => "Force le tir allie : on, off ou auto pour rendre la main au plugin.";

        public override string Permission => "autofftoggle.manage";

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage : autoff set <on|off|auto>";
                return false;
            }

            const string owner = Handlers.ServerHandlers.ManualOwner;

            switch (arguments.At(0).ToLowerInvariant())
            {
                case "on":
                    FriendlyFireArbiter.RequestOn(owner);
                    break;

                case "off":
                    FriendlyFireArbiter.RequestOff(owner);
                    break;

                case "auto":
                    FriendlyFireArbiter.Release(owner);
                    break;

                default:
                    response = "Valeur attendue : on, off ou auto.";
                    return false;
            }

            Audit(sender, "AutoFFToggle", $"a regle le tir allie sur \"{arguments.At(0)}\"");

            response = FriendlyFireArbiter.Describe();
            return true;
        }
    }
}
