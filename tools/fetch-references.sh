#!/usr/bin/env bash
# Telecharge les assemblies du jeu dans .references/.
# Elles servent uniquement a la compilation et ne sont ni commitees ni deployees.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
archive="$(mktemp --suffix=.zip)"
trap 'rm -f "$archive"' EXIT

curl -fsSL -o "$archive" https://exmod-team.github.io/SL-References/Dev.zip
rm -rf "$root/.references"
unzip -qo "$archive" -d "$root/.references"
echo "References installees dans $root/.references"
