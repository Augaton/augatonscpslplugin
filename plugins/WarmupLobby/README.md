# WarmupLobby

Pendant l'attente de joueurs, tout le monde apparait au meme endroit, reparti en deux camps ennemis, et se bat
jusqu'au lancement du round. Un mort revient aussitot, dans le camp le moins nombreux.

Au lancement, chacun repasse spectateur juste avant que le jeu attribue les roles : le round se deroule normalement,
sans trace de l'echauffement. Les corps sont retires et le compteur de morts repart de zero.

L'echauffement ne touche pas a la carte : pendant ce temps, les joueurs ne peuvent ni lacher ou lancer d'objet, ni
ramasser, ni menotter, ni utiliser une porte, un casier ou un ascenseur. Un mort ne laisse rien au sol. Ses kills ne
rapportent ni influence ni temps de vague au vrai round.

## Reglage du serveur

La duree du lobby vient du jeu : `lobby_waiting_time` dans `config_gameplay.txt`, 20 secondes par defaut. Augmentez-la
si l'echauffement vous semble trop court. L'echauffement ne prolonge pas le lobby.

Gardez-la sous `afk_time`, 90 secondes par defaut : le jeu expulse aussi pendant le lobby un joueur vivant qui ne bouge
pas.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `first_role`, `second_role` | `NtfPrivate`, `ChaosConscript` | Les deux camps, forcement humains et ennemis |
| `arena_spawn_role` | `ChaosConscript` | Role dont le point d'apparition sert d'arene |
| `respawn_seconds` | `3` | Secondes entre deux reapparitions, 1 au plus bas |
| `min_players` | `2` | Joueurs requis pour lancer l'echauffement |
| `hint_duration` | `4` | Duree des hints, en secondes |

`collection check` signale un `arena_spawn_role` sans point d'apparition.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `warmup status` | `warmuplobby.manage` | Etat du plugin |

Alias `lobby`.
