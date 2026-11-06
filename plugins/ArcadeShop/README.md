# ArcadeShop

Les joueurs gagnent des points pendant le round et les depensent dans une boutique, depuis la console du jeu. Les
points ne survivent pas au round : rien n'est stocke, et un joueur qui part perd son solde.

Rapportent des points : tuer un ennemi, tuer un SCP, lancer un generateur et s'evader. Le tir allie ne rapporte
rien, et un generateur ne paie qu'une fois, meme s'il est coupe puis relance. La boutique ferme pendant un mode de
jeu.

Seul un humain vivant et sans menottes peut acheter. Un inventaire plein laisse l'objet a ses pieds, et la recompense
`SCP500` est une vraie pilule avec SCP500s.

## Joueurs

| Commande | Effet |
|---|---|
| `.shop` | Affiche le solde et le catalogue |
| `.shop <nom>` | Achete un objet, par exemple `.shop soin` |

Alias `.boutique`.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `points_per_kill` | `10` | Points pour un ennemi humain tue |
| `points_per_scp_kill` | `40` | Points pour un SCP tue |
| `points_per_generator` | `15` | Points pour lancer un generateur, une fois par generateur |
| `points_per_escape` | `30` | Points pour une evasion |
| `points_per_contract` | `15` | Points par contrat rempli, si Contracts est installe |
| `max_balance` | `500` | Solde maximal d'un joueur |
| `buy_cooldown_seconds` | `2` | Delai entre deux achats, 1 au plus bas |
| `items` | 9 objets | Catalogue : `key`, `label`, `item`, `quantity`, `price`, `limit` |
| `pause_during_gamemodes` | `true` | Ferme la boutique pendant un mode de jeu |
| `hint_duration` | `3` | Duree des hints, en secondes |

`limit` compte les achats par joueur et par round, `0` voulant dire sans limite. `quantity` compte les munitions pour
une boite. Un objet sans `key`, sans `item` ou a prix negatif est ignore au chargement, avec un avertissement.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `arcadeshop status` | `arcadeshop.manage` | Etat du plugin |
| `arcadeshop give <joueur> <points>` | `arcadeshop.manage` | Donne des points, de 1 a 10000 |

Alias `ashop`. Chaque don est trace.
