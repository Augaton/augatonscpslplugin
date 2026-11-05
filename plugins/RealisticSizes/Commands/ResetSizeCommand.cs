using System;
using AugatonLib.Arbitration;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace RealisticSizes.Commands
{
    public sealed class ResetSizeCommand : StaffCommand
    {
        public override string Command => "reset";

        public override string[] Aliases => Array.Empty<string>();

        public override string Description => "Remet la taille normale a un joueur, ou a tous avec *.";

        public override string Permission => "realisticsizes.manage";

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Usage : sizes reset <joueur|*>";
                return false;
            }

            string target = arguments.At(0);

            if (target == "*")
            {
                int count = 0;

                foreach (Player player in Player.ReadyList)
                {
                    // Player.Scale vaut zero hors role FPC : un spectateur serait compte a tort.
                    if (player.RoleBase is not IFpcRole || player.Scale == Vector3.one)
                        continue;

                    ScaleArbiter.Reset(player, Vector3.one);
                    count++;
                }

                Audit(sender, "RealisticSizes", $"a remis a zero la taille de {count} joueur(s)");

                response = $"{count} joueur(s) remis a la taille normale.";
                return true;
            }

            if (!TryFindPlayer(target, out Player found, out string error))
            {
                response = error;
                return false;
            }

            ScaleArbiter.Reset(found, Vector3.one);
            Audit(sender, "RealisticSizes", $"a remis a zero la taille de {found.Nickname} ({found.UserId})");

            response = $"{found.Nickname} est revenu a sa taille normale.";
            return true;
        }
    }
}
