# Streaks

Enchainer les kills sans mourir rapporte des objets. Au-dela d'un seuil, tout le serveur apprend qu'une prime
pese sur le joueur, et celui qui l'abat recoit la prime.

Seuls comptent les kills d'un humain sur un ennemi : ni le tir allie, ni le suicide, ni le role Tutorial. Mourir
remet la serie a zero. Les SCP ont leur propre progression avec ScpLevels. Rien ne compte pendant un mode de jeu.

Avec SCP500s, la recompense `SCP500` est une vraie pilule. Un inventaire plein laisse l'objet aux pieds du joueur.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `rewards` | 3, 5, 8 et 12 kills | Paliers : `kills`, `item`, `quantity`, `label` |
| `bounty_kills` | `5` | Kills d'affilee qui mettent une prime. `0` = jamais |
| `bounty_item`, `bounty_quantity` | `Adrenaline`, `1` | Objet donne a celui qui abat la cible |
| `kills_for_gamemode_win` | `2` | Kills d'avance au round suivant pour le vainqueur d'un mode de jeu. `0` = aucun |
| `pause_during_gamemodes` | `true` | Ne compte rien pendant un mode de jeu |
| `hint_duration` | `4` | Duree des hints, en secondes |

Les messages se reglent dans `translation.yml`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `streaks status` | `streaks.manage` | Etat du plugin |
| `streaks add <joueur>` | `streaks.manage` | Compte un kill, pour tester seul les paliers et la prime |

Alias `streak`. Chaque ajout est trace.

## Synergies

ChaosDirector compte les paliers atteints, et RoundRecap affiche la meilleure serie du round.
