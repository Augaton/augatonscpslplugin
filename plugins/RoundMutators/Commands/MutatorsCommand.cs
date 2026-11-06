using System;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace RoundMutators.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class MutatorsCommand : StaffParentCommand
    {
        internal const string Node = "roundmutators.manage";

        public MutatorsCommand() => LoadGeneratedCommands();

        public override string Command => "mutators";

        public override string[] Aliases => new[] { "mutator", "mut" };

        public override string Description => "Mutateurs de round.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("RoundMutators", typeof(Plugin), Permission, builder =>
            {
                MutatorType? active = Plugin.Instance.Service.Active;

                builder.AppendLine($"  mutateur : {(active.HasValue ? active.Value.ToString() : "aucun")}");
                return true;
            }));
            RegisterCommand(new ForceCommand());
            RegisterCommand(new StopCommand());
        }
    }

    public sealed class ForceCommand : StaffCommand
    {
        public override string Command => "force";

        public override string Description => "Lance un mutateur tout de suite.";

        public override string Permission => MutatorsCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0
                || !Enum.TryParse(arguments.At(0), true, out MutatorType type)
                || !Enum.IsDefined(typeof(MutatorType), type))
            {
                response = $"Usage : mutators force <{string.Join("|", Enum.GetNames(typeof(MutatorType)))}>";
                return false;
            }

            if (!Round.IsRoundInProgress)
            {
                response = "Aucun round en cours.";
                return false;
            }

            Plugin.Instance.Service.Begin(type);
            Audit(sender, "RoundMutators", $"a lance le mutateur {type}");

            response = $"Mutateur lance : {type}.";
            return true;
        }
    }

    public sealed class StopCommand : StaffCommand
    {
        public override string Command => "stop";

        public override string Description => "Arrete le mutateur en cours.";

        public override string Permission => MutatorsCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin.Instance.Service.Stop();
            Audit(sender, "RoundMutators", "a arrete le mutateur");

            response = "Mutateur arrete.";
            return true;
        }
    }
}
