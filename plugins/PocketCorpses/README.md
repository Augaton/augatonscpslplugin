# PocketCorpses

Un joueur mort dans la dimension de poche ne disparait plus pour de bon : son corps retombe dans le
complexe, comme les objets que SCP-106 recrache. Le corps tombe du plafond au-dessus d'un joueur vivant
pris au hasard. L'alerte sonore est celle du jeu, et un gouffre peut s'ouvrir au sol juste avant.

Le corps garde son nom, son role et sa cause de mort. SCP-049 peut donc le trouver et le ranimer.

Tant qu'aucun joueur n'est hors de la dimension, le corps reste en attente.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `return_chance` | `100` | Chance en pourcentage qu'un corps retombe |
| `min_delay_seconds`, `max_delay_seconds` | `30`, `90` | Delai avant la chute. Le jeu rend les objets entre 90 et 240 secondes |
| `warning_seconds` | `3` | Secondes entre l'alerte et la chute |
| `sinkhole_under_corpses` | `true` | Ouvre un gouffre avant la chute d'un corps |
| `sinkhole_under_items` | `false` | Ouvre un gouffre avant la chute d'un objet |
| `sinkhole_seconds` | `8` | Duree de vie d'un gouffre |

Un gouffre ralentit les humains qui marchent dessus, jamais les SCP.

## Attribution

D'apres une idee de DentyTxR, reecrit sans reprendre son code : voir `NOTICE.md`.
