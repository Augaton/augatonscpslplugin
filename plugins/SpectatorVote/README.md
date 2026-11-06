# SpectatorVote

Les morts ne font plus qu'attendre : a intervalle regulier, les spectateurs votent pour le prochain incident du
site parmi trois choix tires au hasard. Une egalite se tranche au hasard, et un vote sans bulletin ne declenche
rien.

Ce plugin a besoin de **SiteIncidents**, qui joue l'incident gagnant. Sans lui, le plugin se charge mais aucun vote
ne s'ouvre, et `collection check` le signale. Les votes sont suspendus pendant un mode de jeu.

## Joueurs

| Commande | Effet |
|---|---|
| `.vi` | Affiche les choix du vote en cours |
| `.vi <numero>` | Vote, et peut changer son vote tant que le scrutin est ouvert |

Alias de `.voteincident`. Seuls les spectateurs votent.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `first_vote_seconds` | `240` | Delai avant le premier vote |
| `vote_interval_seconds` | `240` | Delai entre deux votes, au moins la duree d'un vote plus 30 |
| `vote_duration_seconds` | `25` | Duree d'un vote, 10 au plus bas |
| `option_count` | `3` | Choix proposes, 2 au plus bas |
| `min_spectators` | `2` | Spectateurs requis pour ouvrir un vote |
| `vote_command_cooldown_seconds` | `1` | Delai entre deux `.vi`, 1 au plus bas |
| `options` | 5 incidents | Choix possibles : `incident`, `label`, `weight` |
| `pause_during_gamemodes` | `true` | Suspend les votes pendant un mode de jeu |
| `hint_duration` | `5` | Duree du hint de resultat, en secondes |

`incident` prend un nom de SiteIncidents : `Blackout`, `Alert`, `Lockdown`, `GasLeak`, `TeslaSurge` ou `CameraOutage`.
Un nom inconnu produit un avertissement dans la console de SiteIncidents. `CameraOutage` n'est pas propose par defaut :
sans SCP-079, il ne ferait rien, et le vote serait perdu.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `spectatorvote status` | `spectatorvote.manage` | Etat du plugin |
| `spectatorvote open` | `spectatorvote.manage` | Ouvre un vote tout de suite, meme avec peu de spectateurs |

Alias `svote`. Chaque ouverture forcee est tracee.
