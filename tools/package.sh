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

cp "$root/third_party/FairScpSelection/LICENSE.txt" "$stage/licenses/FairScpSelection-LICENSE.txt"
cp "$root"/third_party/licenses/*.txt "$stage/licenses/"
cp "$root/plugins/AntiSLCrush/LICENSE.md" "$stage/licenses/AntiSLCrush-LICENSE.md"

rm -f "$root/out/zone-shilari-labapi.zip"
(cd "$stage" && zip -qr "$root/out/zone-shilari-labapi.zip" .)
echo "Archive creee : $root/out/zone-shilari-labapi.zip"
