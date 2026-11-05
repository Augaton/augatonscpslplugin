using CommandSystem;
using AugatonLib.Commands;

namespace BetterCoinflipsRewritten.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class CoinflipCommand : StaffParentCommand
    {
        public CoinflipCommand() => LoadGeneratedCommands();

        public override string Command => "coinflip";

        public override string[] Aliases => new[] { "bcf" };

        public override string Description => "Etat du pile ou face.";

        public override string Permission => "bettercoinflips.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "BetterCoinflipsRewritten",
                typeof(Plugin), Permission, builder =>
            {
                Config config = Plugin.Instance.Config;
                builder.AppendLine($"  piece consommee au lancer : {(config.ConsumeCoinOnFlip ? "oui" : "non")}");
                builder.AppendLine($"  cooldown par joueur : {config.CoinCooldown:0.#}s");
                builder.AppendLine($"  cooldown des effets globaux : {config.FacilityEffectCooldown:0.#}s");
                builder.AppendLine($"  pieces ajoutees au loot : {(config.AddBonusCoinsToLoot ? config.BonusCoinAmount : 0)}");
                builder.AppendLine($"  pieces posees sur la carte : {(config.SpawnCoinsOnMap ? config.MapCoinAmount : 0)}");
                return true;
            }));
        }
    }
}
