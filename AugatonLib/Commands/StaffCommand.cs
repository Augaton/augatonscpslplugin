using System;
using AugatonLib.Bus;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace AugatonLib.Commands
{
    public abstract class StaffCommand : ICommand
    {
        public abstract string Command { get; }

        public virtual string[] Aliases => Array.Empty<string>();

        public abstract string Description { get; }

        public abstract string Permission { get; }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.HasPermissions(Permission))
                {
                    response = $"Permission refusee. Noeud requis : {Permission}";
                    return false;
                }

                foreach (string argument in arguments)
                {
                    if (argument is not null && argument.Length > 96)
                    {
                        response = "Argument trop long.";
                        return false;
                    }
                }

                return OnExecute(arguments, sender, out response);
            }
            catch (Exception e)
            {
                Logger.Error($"{Command}: {e}");
                response = "Erreur interne, voir la console serveur.";
                return false;
            }
        }

        protected abstract bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response);

        protected static string Author(ICommandSender sender)
        {
            return Player.Get(sender) is Player player
                ? $"{player.Nickname} ({player.UserId})"
                : sender.LogName;
        }

        protected static void Audit(ICommandSender sender, string plugin, string action)
        {
            StaffAction entry = new StaffAction(plugin, Author(sender), action);

            Logger.Info(entry.ToString());
            PluginBus.Publish(BusTopics.StaffAction, plugin, entry);
        }

        protected static bool TryFindPlayer(string query, out Player player, out string error)
        {
            player = null;
            error = null;

            if (string.IsNullOrEmpty(query))
            {
                error = "Joueur non specifie.";
                return false;
            }

            player = Find(query);

            if (player is null)
            {
                error = $"Joueur \"{query}\" introuvable.";
                return false;
            }

            return true;
        }

        private static Player Find(string query)
        {
            if (int.TryParse(query, out int playerId) && Player.TryGet(playerId, out Player byId))
                return byId;

            return Player.Get(query) ?? Player.GetByDisplayName(query) ?? Player.GetByNickname(query);
        }
    }
}
