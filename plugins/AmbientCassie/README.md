# AmbientCassie

C.A.S.S.I.E parle de temps en temps pendant le round, pour nourrir l'ambiance RP : rappels au personnel,
verifications des systemes, puis messages plus inquietants quand le round s'etire.

Une annonce d'ambiance cede toujours la place. Elle n'est pas jouee si C.A.S.S.I.E parle deja, pendant le
compte a rebours de l'ogive, ou pendant un mode arene. Elle ne coupe donc jamais une annonce d'extinction
de TeamGenocide ni un incident de SiteIncidents.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `first_announcement_seconds` | `240` | Delai avant la premiere annonce |
| `min_interval_seconds`, `max_interval_seconds` | `300`, `600` | Delai entre deux annonces, 60 au plus bas |
| `late_round_seconds` | `600` | A partir de la, les annonces `Late` remplacent les annonces `Early` |
| `min_players` | `4` | Joueurs requis |
| `pause_during_gamemodes` | `true` | Suspend les annonces pendant un mode arene |

Les annonces se reglent dans `translation.yml`, chacune avec `cassie`, `subtitles`, `phase` (`Any`, `Early`
ou `Late`) et `weight`. C.A.S.S.I.E ne connait qu'une liste de mots : `collection check` signale un mot
inconnu.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `ambient status` | `ambientcassie.manage` | Etat du plugin |
| `ambient play` | `ambientcassie.manage` | Joue tout de suite une annonce |

Alias `amb`. Chaque annonce forcee est tracee.
