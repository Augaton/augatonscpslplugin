# SiteIncidents

Provoque des incidents aleatoires dans le complexe pendant le round : panne d'eclairage d'une zone,
alerte lumineuse rouge, ou portes bloquees quelques secondes. Chaque incident est annonce par
C.A.S.S.I.E, avec des sous-titres en francais.

Les lumieres passent par `LightArbiter`, donc les incidents cohabitent avec les pannes des autres
plugins. Le verrouillage utilise son propre drapeau : il ne touche jamais une porte bloquee par le
staff, et il ignore les portes deja verrouillees.

Aucun incident n'a lieu si la zone est perdue : pas de zone legere pendant la decontamination, et
plus aucun incident apres l'explosion de l'ogive.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `first_incident_seconds` | `300` | Delai avant le premier incident |
| `min_interval_seconds` | `180` | Delai minimal entre deux incidents, 30 au plus bas |
| `max_interval_seconds` | `360` | Delai maximal entre deux incidents |
| `min_players` | `4` | Joueurs requis |
| `pause_during_gamemodes` | `true` | Suspend les incidents pendant un mode de type arene |
| `interval_per_chaos_level` | `20` | Secondes retirees par niveau du Directeur du chaos, s'il est installe |
| `blackout_weight`, `alert_weight`, `lockdown_weight` | `10` | Probabilite de chaque incident. `0` = jamais |
| `blackout_duration`, `alert_duration`, `lockdown_duration` | `20`, `30`, `15` | Durees en secondes |
| `alert_color` | `#FF0000` | Couleur de l'alerte |
| `lockdown_doors` | `10` | Portes bloquees dans la zone |

Les annonces et les noms de zones se reglent dans `translation.yml`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `incidents status` | `siteincidents.manage` | Etat du plugin |
| `incidents trigger <type>` | `siteincidents.manage` | Declenche `Blackout`, `Alert` ou `Lockdown` |

Alias `inc`. Chaque declenchement manuel est trace.
