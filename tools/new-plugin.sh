#!/usr/bin/env bash
# Cree un plugin pret a compiler, et l'inscrit dans le catalogue d'AugatonLib et la solution.
#
#   ./tools/new-plugin.sh MonPlugin "role en quelques mots"
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
name="${1:-}"
role="${2:-}"
folder="$root/plugins/$name"

fail() {
    echo "new-plugin.sh : $1" >&2
    exit 1
}

# Insere une ligne a sa place alphabetique parmi celles qui contiennent le marqueur.
insert_sorted() {
    local file="$1" marker="$2" line="$3" sorted

    sorted="$(mktemp)"

    awk -v marker="$marker" -v line="$line" '
        index($0, marker) {
            seen = 1

            if (!placed && tolower($0) > tolower(line)) {
                print line
                placed = 1
            }

            print
            next
        }

        seen && !placed {
            print line
            placed = 1
        }

        { print }
    ' "$file" > "$sorted"

    cat "$sorted" > "$file"
    rm -f "$sorted"
}

[[ "$name" =~ ^[A-Z][A-Za-z0-9]+$ ]] || fail "le nom doit etre en PascalCase. Usage : $0 MonPlugin \"role en quelques mots\""
[[ "$role" =~ ^[a-zA-Z0-9\ \'-]+$ ]] || fail "le role est obligatoire, sans accent ni guillemet"
[ -z "$(find "$root/plugins" -maxdepth 1 -iname "$name")" ] || fail "plugins/$name existe deja"

lower="$(echo "$name" | tr '[:upper:]' '[:lower:]')"

mkdir -p "$folder/Commands"
cp "$root/plugins/TickGuard/LICENSE" "$folder/LICENSE"

cat > "$folder/$name.csproj" <<PROJECT
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <RootNamespace>$name</RootNamespace>
    <Version>1.0.0</Version>
  </PropertyGroup>

</Project>
PROJECT

cat > "$folder/Config.cs" <<CONFIG
using System.ComponentModel;

namespace $name
{
    public sealed class Config
    {
        [Description("Suspend le plugin pendant un mode de jeu.")]
        public bool PauseDuringGamemodes { get; set; } = true;
    }
}
CONFIG

cat > "$folder/Plugin.cs" <<PLUGIN
using AugatonLib.Runtime;

namespace $name
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "$name";

        public override string Description => "$role";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;

            PluginDirectory.Register(this);
        }

        public override void Disable()
        {
            PluginDirectory.Unregister(this);

            Instance = null;
        }
    }
}
PLUGIN

cat > "$folder/Commands/${name}Command.cs" <<COMMAND
using System;
using AugatonLib.Commands;
using CommandSystem;

namespace $name.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ${name}Command : StaffParentCommand
    {
        public ${name}Command() => LoadGeneratedCommands();

        public override string Command => "$lower";

        public override string[] Aliases => Array.Empty<string>();

        public override string Description => "$role";

        public override string Permission => "$lower.manage";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CollectionCommand(Permission));
            RegisterCommand(new StatusCommand("$name", typeof(Plugin), Permission));
        }
    }
}
COMMAND

cat > "$folder/README.md" <<README
# $name

$role.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| \`pause_during_gamemodes\` | \`true\` | Suspend le plugin pendant un mode de jeu |

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| \`$lower status\` | \`$lower.manage\` | Etat du plugin |
README

insert_sorted "$root/augatonscpslplugin.slnx" 'Path="plugins/' "    <Project Path=\"plugins/$name/$name.csproj\" />"
insert_sorted "$root/AugatonLib/Runtime/KnownPlugin.cs" 'new KnownPlugin("' "            new KnownPlugin(\"$name\", \"$name\", \"$role\"),"

echo "plugins/$name cree. Reste a ecrire sa logique, puis a lancer ./tools/check.sh."
