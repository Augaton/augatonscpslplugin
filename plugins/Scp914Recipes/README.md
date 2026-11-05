# Scp914Recipes

Ajoute a SCP-914 des recettes reglables en config, pour les objets poses dans la machine comme pour ceux
que portent les joueurs. Quand une recette echoue a son jet, la machine agit normalement.

| Entree | Reglage | Sortie | Chance |
|---|---|---|---|
| `Coin` | `Fine` | `SCP500` | 25 % |
| `Coin` | `VeryFine` | `Medkit` | 50 % |
| `Painkillers` | `Rough` | `Coin` | 100 % |
| `Medkit` | `Rough` | `Coin` | 100 % |

Le plugin fonctionne seul. Avec SCP500s, une sortie `SCP500` donne une vraie pilule ; sans lui, une SCP-500
ordinaire. Avec BetterCoinflips, les pieces obtenues sont des pieces normales.

Une pilule de SCP500s ne peut pas servir d'entree : ce plugin la protege de la machine.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `pause_during_gamemodes` | `true` | Rend a la machine son fonctionnement normal pendant un mode arene |
| `recipes` | voir plus haut | Recettes, testees dans l'ordre |

Chaque recette se compose de `input`, `setting`, `output` et `chance`. Les reglages sont `Rough`, `Coarse`,
`OneToOne`, `Fine` et `VeryFine`. `collection check` signale un objet de sortie inconnu.
