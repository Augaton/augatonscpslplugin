#!/usr/bin/env bash
# Assemble out/zone-shilari-labapi.zip, a extraire dans le dossier LabAPI du serveur.
# Le dossier global/ est charge quel que soit le port.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
stage="$(mktemp -d)"
trap 'rm -rf "$stage"' EXIT

mkdir -p "$stage/plugins/global" "$stage/dependencies/global" "$stage/licenses"
cp "$root"/out/plugins/*.dll "$stage/plugins/global/"
cp "$root"/out/dependencies/AugatonLib.dll "$stage/dependencies/global/"

cp "$root/third_party/FairScpSelection/LICENSE.txt" "$stage/licenses/FairScpSelection-LICENSE.txt"

rm -f "$root/out/zone-shilari-labapi.zip"
(cd "$stage" && zip -qr "$root/out/zone-shilari-labapi.zip" .)
echo "Archive creee : $root/out/zone-shilari-labapi.zip"
