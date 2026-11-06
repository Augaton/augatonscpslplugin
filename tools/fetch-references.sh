#!/usr/bin/env bash
# Telecharge les assemblies du jeu dans .references/ et verifie leur empreinte.
# Elles servent uniquement a la compilation et ne sont ni commitees ni deployees.
#
# Dev.zip suit les mises a jour du jeu : apres l'une d'elles, le telechargement est refuse. Verifiez alors
# que le depot compile, et acceptez la nouvelle empreinte avec --update, a committer avec ce changement.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source_url="https://exmod-team.github.io/SL-References/Dev.zip"
pin_file="$root/tools/references.sha256"
target="$root/.references"
stamp="$target/.sha256"
update=false

case "${1:-}" in
    "") ;;
    --update) update=true ;;
    *)
        echo "Usage : $0 [--update]" >&2
        exit 2
        ;;
esac

expected="$(cat "$pin_file")"

if [ "$update" = false ] && [ -f "$stamp" ] && [ "$(cat "$stamp")" = "$expected" ]; then
    echo "References deja a jour dans $target"
    exit 0
fi

# Sur le meme disque que la cible, et rien n'est remplace avant la fin : un echec laisse .references/ intact.
work="$(mktemp -d "$root/.references.XXXXXX")"
trap 'rm -rf "$work"' EXIT

curl -fsSL --proto '=https' --tlsv1.2 --retry 3 --connect-timeout 20 --max-time 300 \
    -o "$work/references.zip" "$source_url"

actual="$(sha256sum "$work/references.zip" | cut -d ' ' -f 1)"

if [ "$update" = false ] && [ "$actual" != "$expected" ]; then
    {
        echo "Empreinte inattendue pour $source_url"
        echo "  attendue : $expected"
        echo "  recue    : $actual"
        echo "Le jeu a peut-etre change. Si c'est bien le cas, relancez avec --update."
    } >&2
    exit 1
fi

unzip -qo "$work/references.zip" -d "$work/extracted"

if [ ! -f "$work/extracted/Assembly-CSharp.dll" ]; then
    echo "Archive inattendue : Assembly-CSharp.dll est absent." >&2
    exit 1
fi

echo "$actual" > "$work/extracted/.sha256"
echo "$actual" > "$pin_file"

rm -rf "$target"
mv "$work/extracted" "$target"
echo "References installees dans $target"
