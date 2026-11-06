#!/usr/bin/env bash
# Assemble out/zone-shilari-labapi.zip, a extraire dans le dossier LabAPI du serveur, et out/SHA256SUMS.
# Le dossier global/ est charge quel que soit le port. Le moindre controle en echec arrete le script :
# une archive incomplete ne part jamais.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
stage="$(mktemp -d)"
packages="${NUGET_PACKAGES:-$HOME/.nuget/packages}"
archive="$root/out/zone-shilari-labapi.zip"
trap 'rm -rf "$stage"' EXIT

fail() {
    echo "package.sh : $1" >&2
    exit 1
}

# La version vient du projet qui reference le paquet : elle n'est ainsi tenue qu'a un seul endroit.
package_dll() {
    local project="$1" package="$2" dll="$3" version id

    version="$(sed -n "s/.*PackageReference Include=\"$package\" Version=\"\([^\"]*\)\".*/\1/p" "$root/$project" | head -n 1)"
    [ -n "$version" ] || fail "version de $package introuvable dans $project"

    id="$(echo "$package" | tr '[:upper:]' '[:lower:]')"
    [ -f "$packages/$id/$version/lib/net48/$dll" ] || fail "$dll introuvable : lancez d'abord dotnet build"

    echo "$packages/$id/$version/lib/net48/$dll"
}

# out/ survit d'un build a l'autre : une DLL sans projet vient d'un plugin renomme ou retire.
for dll in "$root"/out/plugins/*.dll; do
    name="$(basename "$dll" .dll)"

    [ -d "$root/plugins/$name" ] || [ -d "$root/third_party/$name" ] \
        || fail "$name.dll n'a plus de projet : videz out/ et recompilez"
done

for project in "$root"/plugins/*/*.csproj "$root"/third_party/*/*.csproj; do
    name="$(basename "$project" .csproj)"

    [ -f "$root/out/plugins/$name.dll" ] || fail "$name.dll manque : lancez d'abord dotnet build -c Release"
done

[ -f "$root/out/dependencies/AugatonLib.dll" ] || fail "AugatonLib.dll manque : lancez d'abord dotnet build -c Release"

mkdir -p "$stage/plugins/global" "$stage/dependencies/global" "$stage/licenses"
cp "$root"/out/plugins/*.dll "$stage/plugins/global/"
cp "$root"/out/dependencies/AugatonLib.dll "$stage/dependencies/global/"

# Les dependances obligatoires sont livrees avec la collection, aux versions utilisees pour compiler.
hint_service="$(package_dll AugatonLib/AugatonLib.csproj HintServiceMeow HintServiceMeow.dll)"
harmony="$(package_dll plugins/AntiSLCrush/AntiSLCrush.csproj Lib.Harmony 0Harmony.dll)"

cp "$hint_service" "$stage/plugins/global/"
cp "$harmony" "$stage/dependencies/global/"

cp "$root/LICENSE" "$stage/licenses/AugatonLib-LICENSE.txt"
cp "$root/third_party/FairScpSelection/LICENSE.txt" "$stage/licenses/FairScpSelection-LICENSE.txt"
cp "$root"/third_party/licenses/*.txt "$stage/licenses/"

# La MIT exige que la licence accompagne le code : aucun plugin ne part sans la sienne, ou sans son
# attribution quand l'auteur d'origine n'en a pas publie.
for folder in "$root"/plugins/*/; do
    name="$(basename "$folder")"
    found=false

    for file in LICENSE LICENSE.md NOTICE.md; do
        [ -f "$folder$file" ] || continue

        target="$name-$file"
        [ "$file" != LICENSE ] || target="$target.txt"

        cp "$folder$file" "$stage/licenses/$target"
        found=true
    done

    [ "$found" = true ] || fail "$name n'a ni licence ni attribution"
done

rm -f "$archive"
(cd "$stage" && zip -qrX "$archive" .)

# Noms sans dossier : une release publie tous ces fichiers cote a cote.
{
    (cd "$root/out" && sha256sum "$(basename "$archive")")
    (cd "$root/out/plugins" && sha256sum ./*.dll)
    (cd "$root/out/dependencies" && sha256sum AugatonLib.dll)
} > "$root/out/SHA256SUMS"

echo "Archive creee : $archive"
