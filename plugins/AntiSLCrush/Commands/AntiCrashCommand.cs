// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using AugatonLib.Commands;
using CommandSystem;

namespace AntiSLCrush.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class AntiCrashCommand : StaffParentCommand
    {
        public AntiCrashCommand() => LoadGeneratedCommands();

        public override string Command => "anticrash";

        public override string[] Aliases => new[] { "asc" };

        public override string Description => "Etat des protections anti-crash.";

        public override string Permission => "anticrash.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));

            RegisterCommand(new StatusCommand(
                "AntiSLCrush",
                typeof(Plugin),
                Permission,
                builder =>
                {
                    builder.Append(Protections.Describe());
                    return true;
                }));
        }
    }
}
