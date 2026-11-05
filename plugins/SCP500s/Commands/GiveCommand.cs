using System;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCP500s.API;

namespace SCP500s.Commands
{
    public sealed class GiveCommand : StaffCommand
    {
        private const int MaxAmount = 8;

        public override string Command => "give";

        public override string[] Aliases => new[] { "g" };

        public override string Description => "Donne une pilule SCP-500 a un joueur. Usage : scp500 give <joueur|*> <cle> [quantite]";

        public override string Permission => Scp500Command.PermissionNode;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.Instance is null)
            {
                response = "Plugin indisponible.";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage : scp500 give <joueur|*> <cle> [quantite]";
                return false;
            }

            string rawTarget = arguments.At(0);
            string rawKey = arguments.At(1);

            if (rawTarget.Length > 64 || rawKey.Length > 64)
            {
                response = "Argument trop long.";
                return false;
            }

            PillBase pill = FindPill(rawKey);

            if (pill is null)
            {
                response = $"Cle \"{rawKey}\" inconnue. Utilisez scp500 list pour voir les cles disponibles.";
                return false;
            }

            int amount = 1;

            if (arguments.Count >= 3)
            {
                if (!int.TryParse(arguments.At(2), out amount) || amount < 1 || amount > MaxAmount)
                {
                    response = $"Quantite invalide. Attendu un entier entre 1 et {MaxAmount}.";
                    return false;
                }
            }

            if (rawTarget == "*")
                return GiveToEveryone(sender, pill, amount, out response);

            if (!TryFindPlayer(rawTarget, out Player target, out string error))
            {
                response = error;
                return false;
            }

            int given = Give(target, pill, amount);

            if (given == 0)
            {
                response = $"{target.Nickname} n'a aucun emplacement libre.";
                return false;
            }

            Audit(sender, Plugin.Instance.Name, $"a donne {given}x {pill.Key} a {target.Nickname} ({target.UserId})");

            response = given == amount
                ? $"{given}x {pill.Name} donne(s) a {target.Nickname}."
                : $"{given}/{amount} {pill.Name} donne(s) a {target.Nickname}, inventaire plein.";

            return true;
        }

        private static bool GiveToEveryone(ICommandSender sender, PillBase pill, int amount, out string response)
        {
            int players = 0;
            int total = 0;

            foreach (Player player in Player.ReadyList)
            {
                if (player is null || player.IsDestroyed || !player.IsAlive)
                    continue;

                int given = Give(player, pill, amount);

                if (given == 0)
                    continue;

                players++;
                total += given;
            }

            if (players == 0)
            {
                response = "Aucun joueur vivant n'a pu recevoir la pilule.";
                return false;
            }

            Audit(sender, Plugin.Instance.Name, $"a donne {total}x {pill.Key} a {players} joueur(s)");

            response = $"{total}x {pill.Name} distribue(s) a {players} joueur(s).";
            return true;
        }

        private static int Give(Player player, PillBase pill, int amount)
        {
            int given = 0;

            for (int i = 0; i < amount; i++)
            {
                if (player.Inventory.UserInventory.Items.Count >= Plugin.Instance.Config.MaxInventorySlots)
                    break;

                if (!pill.Give(player))
                    break;

                given++;
            }

            return given;
        }

        private static PillBase FindPill(string key)
        {
            foreach (PillBase pill in Plugin.Instance.Registry.Pills)
            {
                if (string.Equals(pill.Key, key, StringComparison.OrdinalIgnoreCase))
                    return pill;
            }

            if (uint.TryParse(key, out uint id))
            {
                foreach (PillBase pill in Plugin.Instance.Registry.Pills)
                {
                    if (pill.Id == id)
                        return pill;
                }
            }

            return null;
        }
    }
}
