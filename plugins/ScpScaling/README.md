# ScpScaling

Adapte les PV des SCP au nombre de joueurs connectes : un SCP ne doit ni ecraser une petite partie, ni fondre
dans une grande. Le facteur se calcule a l'apparition du SCP, et les PV partent pleins.

Exemple avec les reglages par defaut : 8 joueurs donnent 80 % des PV, 16 joueurs 100 %, 24 joueurs 120 %.

SCP-079 et SCP-049-2 sont laisses a leurs PV normaux. Un SCP repris par Replacer garde les PV du joueur parti.
Rien ne change pendant un mode de jeu.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `reference_players` | `16` | Joueurs pour lesquels un SCP garde ses PV normaux |
| `percent_per_player` | `2.5` | PV gagnes ou perdus par joueur d'ecart, en pourcentage |
| `min_factor`, `max_factor` | `0.6`, `1.5` | Bornes du facteur |
| `excluded_roles` | `Scp079`, `Scp0492` | Roles laisses a leurs PV normaux |
| `pause_during_gamemodes` | `true` | Ne change rien pendant un mode de jeu |

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `scpscaling status` | `scpscaling.manage` | Etat du plugin et facteur actuel |

Alias `scaling`.
