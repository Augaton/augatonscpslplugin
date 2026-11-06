# SpectatorVote

Les morts ne font plus qu'attendre : a intervalle regulier, les spectateurs votent pour la suite du round parmi
trois choix tires au hasard. Une egalite se tranche au hasard, et un vote sans bulletin ne declenche rien.

Un choix peut etre un incident de **SiteIncidents**, un largage de **SupplyDrop**, un mutateur de **RoundMutators**,
ou un geste direct des morts : soigner les vivants ou aveugler les SCP. Ces deux derniers ne demandent aucun autre plugin.
Chaque choix ne sort que si son plugin est charge, et il faut au moins deux choix pour ouvrir un vote : `collection
check` signale le cas contraire. Un mutateur n'est plus propose quand le round en a deja un. Les votes sont suspendus
pendant un mode de jeu.

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
| `options` | 13 choix | Choix possibles : `kind`, `incident`, `label`, `weight` |
| `blessing_heal` | `30` | Vie rendue aux humains par `HealHumans` |
| `blessing_blind_seconds` | `5` | Aveuglement des SCP par `BlindScps`, de 1 a 25 secondes |
| `pause_during_gamemodes` | `true` | Suspend les votes pendant un mode de jeu |
| `hint_duration` | `5` | Duree du hint de resultat, en secondes |

`kind` vaut `Incident` par defaut, `SupplyDrop`, `Mutator` ou `Blessing`. `incident` nomme la cible :

- avec `Incident`, `Blackout`, `Alert`, `Lockdown`, `GasLeak`, `TeslaSurge`, `CameraOutage` ou `ElevatorOutage` ;
- avec `SupplyDrop`, la cle d'un profil de largage, par exemple `mtf`, `chaos` ou `neutre` ;
- avec `Mutator`, le nom d'un mutateur de RoundMutators ;
- avec `Blessing`, `HealHumans` ou `BlindScps`, qui ne demandent aucun autre plugin.

Un nom inconnu produit un avertissement dans la console du plugin vise. `CameraOutage` n'est pas propose par defaut :
sans SCP-079, il ne ferait rien, et le vote serait perdu. Un `config.yml` deja cree garde ses choix : y ajouter les
largages et les mutateurs a la main, ou le supprimer pour le regenerer.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `spectatorvote status` | `spectatorvote.manage` | Etat du plugin |
| `spectatorvote open` | `spectatorvote.manage` | Ouvre un vote tout de suite, meme avec peu de spectateurs |

Alias `svote`. Chaque ouverture forcee est tracee.
