# Contracts

Chaque humain recoit un contrat secret en debut de round : eliminer des ennemis, activer un generateur, s'evader,
se soigner, ou rester en vie un certain temps. Le remplir donne une recompense. Personne d'autre ne voit le contrat.

Un joueur a un seul contrat par round, meme s'il se deconnecte puis revient. Un joueur revenu par une vague recoit le
sien s'il n'en avait pas. Rien n'est stocke : tout repart de zero au round suivant. Le contrat d'evasion ne va qu'aux
Classe-D et aux scientifiques, les seuls que le jeu laisse s'evader.

Seuls comptent les kills d'un ennemi, ni le tir allie ni le suicide. Rien n'est distribue ni compte pendant un mode
de jeu, ou quand TickGuard a suspendu les evenements. Avec SCP500s, la recompense `SCP500` est une vraie pilule, et
un inventaire plein laisse l'objet aux pieds du joueur. RoundRecap compte les contrats remplis. ArcadeShop ajoute des
points, et ChaosDirector fait monter le niveau de chaos.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `pause_during_gamemodes` | `true` | Coupe les contrats pendant un mode de jeu |
| `first_offer_seconds` | `15` | Delai avant les premiers contrats |
| `command_cooldown_seconds` | `3` | Cooldown de `.contrat`, 1 au plus bas |
| `offers` | 5 contrats | Contrats possibles : `kind`, `target`, `weight`, `reward`, `reward_quantity`, `reward_label` |
| `hint_duration`, `hint_y_coordinate`, `hint_font_size` | `8`, `950`, `20` | Affichage des messages |

Les textes se reglent dans `translation.yml`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `.contrat` | aucune | Rappelle votre contrat et son avancement |
| `contracts status` | `contracts.manage` | Nombre de contrats en cours |

Alias `.contract` et `ctr`.
