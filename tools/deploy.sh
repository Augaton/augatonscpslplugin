#!/usr/bin/env bash
# Envoie out/zone-shilari-labapi.zip sur un serveur par SFTP, par exemple un panel Pelican.
# Les reglages vivent dans .env, jamais committe : voir .env.example.
#
# Arretez le serveur avant l'envoi : ecraser une DLL deja chargee peut le faire planter.
#
#   ./tools/deploy.sh             envoie l'archive
#   ./tools/deploy.sh --dry-run   affiche les commandes sans rien envoyer
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
archive="$root/out/zone-shilari-labapi.zip"
env_file="$root/.env"
dry_run=false

fail() {
    echo "deploy.sh : $1" >&2
    exit 1
}

case "${1:-}" in
    "") ;;
    --dry-run) dry_run=true ;;
    *) fail "usage : $0 [--dry-run]" ;;
esac

[ -f "$env_file" ] || fail "copiez .env.example en .env et remplissez-le"
[ -f "$archive" ] || fail "archive introuvable : lancez tools/check.sh puis tools/package.sh"

# shellcheck source=/dev/null
. "$env_file"

host="${SFTP_HOST:-}"
port="${SFTP_PORT:-2022}"
user="${SFTP_USER:-}"
path="${LABAPI_PATH:-.config/SCP Secret Laboratory/LabAPI}"

[ -n "$host" ] && [ -n "$user" ] || fail "SFTP_HOST et SFTP_USER sont obligatoires dans .env"
[[ "$path" != *'"'* ]] || fail "LABAPI_PATH ne doit pas contenir de guillemet"

stage="$(mktemp -d)"
trap 'rm -rf "$stage"' EXIT

unzip -q "$archive" -d "$stage/files"

# Le tiret laisse passer une erreur de mkdir : le dossier existe souvent deja.
{
    for folder in plugins plugins/global dependencies dependencies/global licenses; do
        echo "-mkdir \"$path/$folder\""
    done

    echo "put \"$stage/files/plugins/global/*\" \"$path/plugins/global/\""
    echo "put \"$stage/files/dependencies/global/*\" \"$path/dependencies/global/\""
    echo "put \"$stage/files/licenses/*\" \"$path/licenses/\""
} > "$stage/batch"

if [ "$dry_run" = true ]; then
    echo "Destination : $user@$host, port $port"
    cat "$stage/batch"
    exit 0
fi

# -b arrete l'envoi a la premiere commande en echec, et BatchMode=no laisse sftp demander le mot de passe.
sftp -o BatchMode=no -P "$port" -b "$stage/batch" "$user@$host"

echo "Envoi termine. Redemarrez le serveur depuis le panel."
