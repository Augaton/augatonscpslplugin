using CommandSystem;
using AugatonLib.Commands;

namespace SCP1162.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class Scp1162Command : StaffParentCommand
    {
        public Scp1162Command() => LoadGeneratedCommands();

        public override string Command => "scp1162";

        public override string[] Aliases => new[] { "1162" };

        public override string Description => "Gestion de l'anomalie SCP-1162.";

        public override string Permission => "scp1162.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "SCP1162",
                typeof(Plugin), Permission, builder =>
            {
                Config config = Plugin.Instance.Config;
                API.Scp1162Service service = Plugin.Instance.Service;

                builder.AppendLine($"  anomalie : {(service is not null && service.IsSpawned ? $"{service.ActiveCount} active(s)" : "absente")}");
                builder.AppendLine($"  portee : {config.UseDistance} m, cooldown : {config.CooldownSeconds} s");
                builder.AppendLine($"  mains arrachees : {config.CutHandsChance}%, objet avale : {config.DeleteChance}%");
                builder.AppendLine($"  emplacements : {config.SpawnCount}/{config.Locations.Count}, apparition : {config.SpawnChance}%");
                builder.AppendLine($"  recompenses : {config.Rewards.Count}");
                return true;
            }));

            RegisterCommand(new SpawnCommand());
            RegisterCommand(new DespawnCommand());
        }
    }
}
