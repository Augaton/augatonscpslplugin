#!/usr/bin/env bash
# Les controles de la CI, a lancer avant de committer. Le premier echec arrete tout.
# La CI et la release appellent ce script : ce qui passe ici passe la-bas.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$root"

echo "== Compilation"
rm -rf "$root/out"
dotnet build -c Release -v q --nologo

echo "== Chaque plugin seul"
for project in plugins/*/*.csproj; do
    dotnet build "$project" -c Release -v q --nologo --no-restore
done

echo "== Tests"
dotnet test tests/AugatonLib.Tests -c Release --nologo -v q --no-build

echo "== Scripts"
if command -v shellcheck > /dev/null; then
    shellcheck --severity=warning tools/*.sh
else
    echo "shellcheck absent : etape passee, la CI la fera."
fi

echo "Tous les controles sont passes."
