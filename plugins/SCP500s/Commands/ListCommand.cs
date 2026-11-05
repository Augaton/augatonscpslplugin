using System;
using System.Text;
using AugatonLib.Commands;
using CommandSystem;
using NorthwoodLib.Pools;
using SCP500s.API;

namespace SCP500s.Commands
{
    public sealed class ListCommand : StaffCommand
    {
        public override string Command => "list";

        public override string[] Aliases => new[] { "l" };

        public override string Description => "Liste les pilules enregistrees. Un filtre optionnel restreint sur la cle ou le nom.";

        public override string Permission => Scp500Command.PermissionNode;

        protected override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.Instance is null)
            {
                response = "Plugin indisponible.";
                return false;
            }

            string filter = arguments.Count > 0 ? arguments.At(0).ToLowerInvariant() : null;

            if (filter != null && filter.Length > 32)
            {
                response = "Filtre trop long.";
                return false;
            }

            string randomKey = Plugin.Instance.Config.RandomPillKey;
            StringBuilder builder = StringBuilderPool.Shared.Rent();
            int shown = 0;

            try
            {
                builder.AppendLine("Pilules enregistrees :");

                foreach (PillBase pill in Plugin.Instance.Registry.Pills)
                {
                    if (filter != null
                        && pill.Key.ToLowerInvariant().IndexOf(filter, StringComparison.Ordinal) < 0
                        && pill.Name.ToLowerInvariant().IndexOf(filter, StringComparison.Ordinal) < 0)
                    {
                        continue;
                    }

                    shown++;

                    string spawn = string.Equals(pill.Key, randomKey, StringComparison.OrdinalIgnoreCase)
                        ? $"part fixe {Plugin.Instance.Config.RandomPillSharePercent} %"
                        : pill.IsRandomOnly
                            ? "roulette uniquement"
                            : $"poids {pill.SpawnWeight}  max/round {pill.MaxPerRound}";

                    builder.AppendLine($"  {pill.Key}  (id {pill.Id})  {pill.Name}  {spawn}");
                }

                if (shown == 0)
                {
                    response = filter is null
                        ? "Aucune pilule enregistree."
                        : $"Aucune pilule ne correspond a \"{filter}\".";

                    return false;
                }

                builder.AppendLine($"Total : {shown} pilule(s).");
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
