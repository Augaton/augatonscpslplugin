using System;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace MicroHidDock.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class MicroDockCommand : StaffParentCommand
    {
        internal const string Node = "microhiddock.manage";

        public MicroDockCommand() => LoadGeneratedCommands();

        public override string Command => "microdock";

        public override string[] Aliases => new[] { "mdock" };

        public override string Description => "Gestion du socle du Micro-HID.";

        public override string Permission => Node;

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("MicroHidDock", typeof(Plugin), Permission, builder =>
            {
                builder.AppendLine($"  socles : {MicroPedestal.List.Count}");
                builder.AppendLine($"  Micro-HID poses : {Plugin.Instance.Dock.Describe()}");
                return true;
            }));
            RegisterCommand(new TestCommand());
        }
    }

    public sealed class TestCommand : StaffCommand
    {
        private const float TestEnergy = 0.1f;

        public override string Command => "test";

        public override string[] Aliases => new[] { "t" };

        public override string Description => "Vous donne un Micro-HID presque vide, pour tester le depot et la recharge.";

        public override string Permission => MicroDockCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Player.Get(sender) is not Player player)
            {
                response = "Commande reservee a un joueur en jeu.";
                return false;
            }

            if (player.AddItem(ItemType.MicroHID) is not MicroHIDItem micro)
            {
                response = "Inventaire plein ou role sans inventaire.";
                return false;
            }

            micro.Energy = TestEnergy;
            Audit(sender, "MicroHidDock", "s'est donne un Micro-HID de test");

            response = $"Micro-HID donne, charge a {TestEnergy:P0}.";
            return true;
        }
    }
}
