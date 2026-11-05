using System;
using System.Text;
using AugatonLib.Commands;
using CommandSystem;
using NorthwoodLib.Pools;
using SupplyDrop.API;

namespace SupplyDrop.Commands
{
    public sealed class ListCommand : StaffCommand
    {
        public override string Command => "list";

        public override string[] Aliases => new[] { "l" };

        public override string Description => "Liste les profils de largage configures.";

        public override string Permission => SupplyDropCommand.Node;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin plugin = Plugin.Instance;

            if (plugin is null)
            {
                response = "Plugin indisponible.";
                return false;
            }

            if (plugin.Config.Profiles is null || plugin.Config.Profiles.Count == 0)
            {
                response = plugin.Translation.NoProfile;
                return false;
            }

            StringBuilder builder = StringBuilderPool.Shared.Rent();

            try
            {
                builder.AppendLine("Profils de largage :");

                foreach (DropProfile profile in plugin.Config.Profiles)
                {
                    if (profile is null)
                        continue;

                    string state = profile.IsEnabled ? "actif" : "desactive";
                    int items = profile.Items?.Count ?? 0;
                    string limit = profile.MaxDropsPerRound > 0 ? profile.MaxDropsPerRound.ToString() : "illimite";

                    builder.AppendLine(
                        $"  {profile.Key}  [{state}]  premier a {profile.FirstDropSeconds:0}s, " +
                        $"puis toutes les {profile.IntervalSeconds:0}s, max {limit}, {items} entree(s) d'objets");
                }

                response = builder.ToString();
                return true;
            }
            finally
            {
                StringBuilderPool.Shared.Return(builder);
            }
        }
    }
}
