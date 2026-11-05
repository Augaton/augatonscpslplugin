# DiscordAudit

Envoie vers un salon Discord les actions du staff que les plugins de la collection tracent deja en
console : `gamemode force`, `scp500 give`, `supplydrop call`, `incidents trigger` et les autres. Sans ce
plugin, ou sans adresse de webhook, rien ne change et rien ne sort du serveur.

Les envois partent hors du thread principal : un Discord lent ne ralentit pas le serveur. Les actions sont
regroupees, la limite de debit de Discord est respectee, et une action qui ne part pas est retentee deux
fois avant d'etre abandonnee.

## Securite

- L'adresse du webhook est un **secret** : quiconque la possede peut ecrire dans le salon. Elle reste dans
  `config.yml` sur le serveur, et n'apparait dans aucun log ni dans aucune commande.
- Seules les adresses `https` de Discord sont acceptees.
- Le texte part dans un bloc de code, et les mentions sont coupees : un pseudo contenant `@everyone` ne
  sonne personne.
- Les lignes contiennent le pseudo et le `UserId` de l'auteur, et souvent du joueur vise. Reserver le salon
  au staff.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `webhook_url` | vide | Adresse du webhook. Vide = aucun envoi |
| `send_interval_seconds` | `5` | Secondes entre deux envois, 2 au plus bas |
| `max_queued_lines` | `200` | Actions gardees en attente. Au-dela, les plus anciennes sont perdues |

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `discordaudit status` | `discordaudit.manage` | Webhook configure ou non, file d'attente, dernier echec |
| `discordaudit test` | `discordaudit.manage` | Envoie une ligne de test |

Alias `daudit`.
