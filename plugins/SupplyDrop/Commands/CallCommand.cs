using System;
using AugatonLib.Commands;
using CommandSystem;
using LabApi.Features.Wrappers;
using SupplyDrop.API;
using UnityEngine;

namespace SupplyDrop.Commands
{
    public sealed class CallCommand : StaffCommand
    {
        public override string Command => "call";

        public override string[] Aliases => new[] { "c" };

        public override string Description => "Declenche un largage immediatement. Usage : supplydrop call <profil> [silencieux] [ici]";

        public override string Permission => SupplyDropCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null)
            {
                response = "Plugin indisponible.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage : supplydrop call <profil> [silencieux] [ici]";
                return false;
            }

            if (!Round.IsRoundInProgress)
            {
                response = plugin.Translation.RoundNotStarted;
                return false;
            }

            string key = arguments.At(0);
            DropProfile profile = plugin.Service.Find(key);

            if (profile is null)
            {
                response = plugin.Translation.UnknownProfile.Replace("%KEY%", key);
                return false;
            }

            bool announce = true;
            Vector3? forcedPosition = null;
            Player caller = Player.Get(sender);

            for (int i = 1; i < arguments.Count; i++)
            {
                string mode = arguments.At(i).ToLowerInvariant();

                if (mode == "silencieux" || mode == "silent" || mode == "quiet")
                {
                    announce = false;
                    continue;
                }

                if (mode != "ici" && mode != "here")
                    continue;

                if (caller is null)
                {
                    response = "Le mode \"ici\" n'est disponible que depuis un client en jeu.";
                    return false;
                }

                forcedPosition = caller.Position;
            }

            plugin.Service.Trigger(profile, announce, forcedPosition);

            Audit(
                sender,
                plugin.Name,
                $"a declenche le largage \"{profile.Key}\" " +
                $"(annonce : {announce}, position forcee : {forcedPosition?.ToString() ?? "non"})");

            response = plugin.Translation.DropCalled.Replace("%KEY%", profile.Key);
            return true;
        }
    }
}
