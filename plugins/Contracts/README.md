# Contracts

Chaque humain recoit un contrat secret en debut de round : eliminer des ennemis, activer un generateur, s'evader,
se soigner, ou rester en vie un certain temps. Le remplir donne une recompense. Personne d'autre ne voit le contrat.
Les SCP en recoivent aussi : eliminer des humains ou tenir un certain temps, contre de la vie. SCP-079 et SCP-049-2
n'en ont pas.

Un contrat rempli en amene un autre, plus dur et mieux paye, jusqu'a `max_contracts_per_round` par joueur et par round.
Quitter le serveur et revenir ne donne pas de contrat de plus. Un joueur revenu par une vague recoit le
sien s'il n'en avait pas. Rien n'est stocke : tout repart de zero au round suivant. Le contrat d'evasion ne va qu'aux
Classe-D et aux scientifiques, les seuls que le jeu laisse s'evader.

Un remplacant de Replacer ou de SCPReplacer qui n'a pas de contrat reprend celui en cours du joueur parti. S'il en a
deja un, il le garde. Un joueur dont le contrat a ete repris n'en recoit pas de nouveau s'il revient dans le round.

Seuls comptent les kills d'un ennemi, ni le tir allie ni le suicide. Rien n'est distribue ni compte pendant un mode
de jeu, ou quand TickGuard a suspendu les evenements. Avec SCP500s, la recompense `SCP500` est une vraie pilule, et
un inventaire plein laisse l'objet aux pieds du joueur. RoundRecap compte les contrats remplis. ArcadeShop ajoute des
points, et ChaosDirector fait monter le niveau de chaos.

Quatre contrats ne sont proposes que si le plugin concerne est installe : lancer des pieces avec
BetterCoinflipsRewritten, avaler une pilule avec SCP500s, utiliser SCP-261 avec Scp261, et echanger a SCP-1162 avec
SCP1162. Un `config.yml` deja cree ne les recoit pas seul : les ajouter a `offers` avec les `kind` `FlipCoins`,
`SwallowPills`, `UseVending` et `ExchangeItem`, ou le supprimer pour le regenerer.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `pause_during_gamemodes` | `true` | Coupe les contrats pendant un mode de jeu |
| `first_offer_seconds` | `15` | Delai avant les premiers contrats |
| `command_cooldown_seconds` | `3` | Cooldown de `.contrat`, 1 au plus bas |
| `excluded_scps` | SCP-079, SCP-049-2 | Roles de SCP sans contrat |
| `max_contracts_per_round` | `2` | Contrats remplis au plus par joueur et par round. `1` = pas de contrat suivant |
| `follow_up_delay_seconds` | `20` | Delai entre un contrat rempli et le suivant, 5 au plus bas |
| `follow_up_offers` | 6 contrats | Contrats suivants, au meme format que `offers` |
| `offers` | 11 contrats | Contrats possibles : `kind`, `for_scps`, `target`, `weight`, `reward`, `reward_quantity`, `reward_health`, `reward_label` |
| `hint_duration`, `hint_y_coordinate`, `hint_font_size` | `8`, `950`, `20` | Affichage des messages |

Les textes se reglent dans `translation.yml`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `.contrat` | aucune | Rappelle votre contrat et son avancement |
| `contracts status` | `contracts.manage` | Nombre de contrats en cours |

Alias `.contract` et `ctr`.
