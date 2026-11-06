# ScpLevels

Chaque kill d'humain rapproche un SCP du niveau suivant. Un niveau donne des PV maximum, soignes aussitot, de la
vitesse et une regeneration. Tout le serveur voit le SCP monter : la menace grandit pendant le round.

Les niveaux disparaissent avec le role. SCP-079 et SCP-049-2 n'en prennent pas, et rien ne compte pendant un mode
de jeu. Avec ScpScaling, les PV bonus s'ajoutent a des PV deja adaptes au nombre de joueurs.

Une mort par decomposition dans la dimension de poche compte pour SCP-106, et un kill de SCP-049-2 pour SCP-049. Un remplacant de Replacer ou de SCPReplacer
reprend le niveau du joueur parti. Chaque niveau gagne alimente ChaosDirector, RoundRecap et AmbientCassie.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `levels` | 2, 4, 7 et 11 kills | Niveaux : `kills`, `movement_boost`, `bonus_health`, `regen_per_second` |
| `excluded_roles` | `Scp079`, `Scp0492` | Roles qui ne montent pas de niveau |
| `announce_levels` | `true` | Annonce chaque niveau a tous. Sinon, seul le SCP le voit |
| `pause_during_gamemodes` | `true` | Ne compte rien pendant un mode de jeu |
| `hint_duration` | `4` | Duree des hints, en secondes |

`bonus_health` s'additionne d'un niveau a l'autre. `movement_boost` et `regen_per_second` remplacent la valeur du
niveau precedent, et `movement_boost` est plafonne a 100.

Les valeurs par defaut sont un point de depart : a equilibrer en jeu.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `scplevels status` | `scplevels.manage` | Etat du plugin |
| `scplevels add <joueur>` | `scplevels.manage` | Compte un kill, pour tester seul les niveaux |

Alias `levels`. Chaque ajout est trace.
