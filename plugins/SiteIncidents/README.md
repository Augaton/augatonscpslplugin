# SiteIncidents

Provoque des incidents aleatoires dans le complexe pendant le round : panne d'eclairage d'une zone,
alerte lumineuse rouge, portes bloquees, fuite de gaz dans une salle, surcharge des portes Tesla, arret
des ascenseurs ou coupure de cameras pour SCP-079. Chaque incident est annonce par C.A.S.S.I.E, avec des sous-titres
en francais.

La fuite de gaz vise une salle ou se trouve un humain : elle s'eclaire en vert et empoisonne les humains
qui y restent. La surcharge fait decharger toutes les portes Tesla a intervalle regulier. La coupure de
cameras n'est tiree que si SCP-079 est en jeu.

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
| `min_event_gap_seconds` | `60` | Ecart minimal avec un largage de SupplyDrop. L'incident retente 20 s plus tard |
| `pause_during_gamemodes` | `true` | Suspend les incidents pendant un mode de type arene |
| `interval_per_chaos_level` | `20` | Secondes retirees par niveau du Directeur du chaos, s'il est installe |
| `blackout_weight`, `alert_weight`, `lockdown_weight` | `10` | Probabilite de chaque incident. `0` = jamais |
| `blackout_duration`, `alert_duration`, `lockdown_duration` | `20`, `30`, `15` | Durees en secondes |
| `alert_color` | `#FF0000` | Couleur de l'alerte |
| `lockdown_doors` | `10` | Portes bloquees dans la zone |
| `elevator_outage_weight` | `6` | Probabilite de l'arret des ascenseurs. `0` = jamais |
| `elevator_outage_duration` | `20` | Duree de l'arret, 5 au plus bas. Il n'a pas lieu pendant l'ogive ni dans la derniere minute avant la decontamination, et l'ogive le leve |
| `gas_leak_weight`, `tesla_surge_weight`, `camera_outage_weight` | `6` | Probabilite de chaque nouvel incident. `0` = jamais |
| `gas_leak_duration`, `tesla_surge_duration`, `camera_outage_duration` | `20`, `15`, `15` | Durees en secondes |
| `gas_leak_color` | `#33FF66` | Couleur de la salle touchee |
| `tesla_surge_interval` | `3` | Secondes entre deux decharges, 2 au plus bas |

Les annonces et les noms de zones se reglent dans `translation.yml`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `incidents status` | `siteincidents.manage` | Etat du plugin |
| `incidents trigger <type>` | `siteincidents.manage` | Declenche `Blackout`, `Alert`, `Lockdown`, `GasLeak`, `TeslaSurge`, `CameraOutage` ou `ElevatorOutage` |

Alias `inc`. Chaque declenchement manuel est trace.

## Bus

Un autre plugin peut demander un incident en passant son nom a `PluginBus.Request` sur
`BusTopics.IncidentRequested`, comme le font SpectatorVote, les pieces et les pilules. La reponse dit si l'incident a
bien lieu. Un nom inconnu produit un avertissement, et la demande est refusee hors d'un round, pendant une pause, ou
apres l'ogive.
