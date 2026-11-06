# RoundMutators

Ajoute parfois une regle legere a un round normal : les modes de RandomGamemode remplacent le round, un mutateur
le pimente sans le changer.

Le plugin reste volontairement discret : au plus un mutateur par round, une chance sur quatre, au moins un round
normal entre deux mutateurs, des valeurs moderees, et un seul hint d'annonce. Seul `SwiftScps` touche les SCP. Aucun
mutateur pendant un mode de jeu, et un mode qui demarre retire le mutateur en cours. RoundRecap affiche le mutateur
du round.

## Mutateurs

| Nom | Effet par defaut |
|---|---|
| `LowGravity` | Gravite a 80 % |
| `Swift` | 8 % de vitesse en plus |
| `Tiny` | Taille a 90 % |
| `Tireless` | Endurance sans fin |
| `NightFall` | Lumieres coupees tout le round, une lampe pour chaque humain |
| `ScarceAmmo` | 60 % des munitions de la carte disparaissent |
| `ScarceMedical` | 60 % des soins de la carte disparaissent |
| `SwiftScps` | 15 % de vitesse en plus pour les SCP |
| `FragileHumans` | Vie maximale des humains a 80 % |

Le mutateur suit les renforts et les changements de role. `Tiny` passe par `ScaleArbiter` et cede la place a une
pilule ou a SCP-261 qui changent la taille d'un joueur.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `chance_percent` | `25` | Chance qu'un round recoive un mutateur |
| `rounds_between` | `1` | Rounds normaux garantis apres un mutateur |
| `min_players` | `6` | Joueurs requis |
| `start_delay_seconds` | `10` | Delai avant le tirage, 3 au plus bas |
| `low_gravity_weight`, `swift_weight`, `tiny_weight`, `tireless_weight` | `10` | Poids de chaque mutateur. `0` = jamais |
| `night_fall_weight`, `scarce_ammo_weight`, `scarce_medical_weight`, `swift_scps_weight`, `fragile_humans_weight` | `10` | Idem |
| `scarce_percent` | `60` | Part des objets retiree de la carte |
| `swift_scp_percent` | `15` | Vitesse ajoutee aux SCP, 100 au plus |
| `fragile_health_percent` | `80` | Vie maximale des humains, 10 au plus bas |
| `gravity_factor` | `0.8` | Gravite avec `LowGravity`, 0.2 au plus bas |
| `speed_percent` | `8` | Vitesse ajoutee avec `Swift`, 100 au plus |
| `size_factor` | `0.9` | Taille avec `Tiny`, 0.5 au plus bas |
| `pause_during_gamemodes` | `true` | Aucun mutateur pendant un mode de jeu |
| `hint_duration` | `6` | Duree du hint d'annonce, en secondes |

Les noms affiches se reglent dans `translation.yml`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `mutators status` | `roundmutators.manage` | Etat du plugin et mutateur en cours |
| `mutators force <nom>` | `roundmutators.manage` | Lance un mutateur tout de suite |
| `mutators stop` | `roundmutators.manage` | Arrete le mutateur en cours |

Alias `mutator`, `mut`. Chaque action est tracee.
