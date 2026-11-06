#!/usr/bin/env bash
# Assemble out/zone-shilari-labapi.zip, a extraire dans le dossier LabAPI du serveur.
# Le dossier global/ est charge quel que soit le port.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
stage="$(mktemp -d)"
packages="${NUGET_PACKAGES:-$HOME/.nuget/packages}"
trap 'rm -rf "$stage"' EXIT

mkdir -p "$stage/plugins/global" "$stage/dependencies/global" "$stage/licenses"
cp "$root"/out/plugins/*.dll "$stage/plugins/global/"
cp "$root"/out/dependencies/AugatonLib.dll "$stage/dependencies/global/"

# Les dependances obligatoires sont livrees avec la collection, aux versions utilisees pour compiler.
cp "$packages/hintservicemeow/5.5.0/lib/net48/HintServiceMeow.dll" "$stage/plugins/global/"
cp "$packages/lib.harmony/2.4.1/lib/net48/0Harmony.dll" "$stage/dependencies/global/"

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

    if [ "$found" = false ]; then
        echo "package.sh : $name n'a ni licence ni attribution." >&2
        exit 1
    fi
done

rm -f "$root/out/zone-shilari-labapi.zip"
(cd "$stage" && zip -qr "$root/out/zone-shilari-labapi.zip" .)
echo "Archive creee : $root/out/zone-shilari-labapi.zip"
