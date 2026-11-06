# ChaosDirector

Fait monter un niveau de chaos commun pendant le round. Pilules avalees, pieces lancees, morts,
largages, incidents et equipes eliminees rapportent des points, et chaque tranche de points ajoute un
niveau.

Ce niveau s'ajoute aux paliers lies au temps de SCP500s et de BetterCoinflips, et raccourcit l'attente
entre deux incidents de SiteIncidents : plus la partie est agitee, plus elle s'emballe. Sans ce plugin,
chacun garde sa montee en puissance au fil du temps.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `pause_during_gamemodes` | `true` | Ne compte rien pendant un mode arene |
| `points_per_pill` | `1` | Points par pilule consommee |
| `points_per_coin_flip` | `1` | Points par piece lancee |
| `points_per_death` | `1` | Points par mort |
| `points_per_drop` | `3` | Points par largage |
| `points_per_incident` | `3` | Points par incident de site |
| `points_per_team_wipe` | `10` | Points par equipe eliminee |
| `points_per_vending` | `1` | Points par utilisation de SCP-261 |
| `points_per_exchange` | `1` | Points par echange a SCP-1162 |
| `points_per_contract` | `1` | Points par contrat rempli |
| `points_per_scp_level` | `3` | Points par niveau gagne par un SCP, avec ScpLevels |
| `points_per_streak` | `2` | Points par palier atteint dans une serie de kills, avec Streaks |
| `points_per_level` | `30` | Points necessaires pour un niveau |
| `max_bonus_level` | `5` | Niveau maximal |
| `apogee_incidents` | 4 incidents | Incidents tires au niveau maximal, une fois par round, avec SiteIncidents |
| `apogee_drops` | `mtf`, `chaos` | Largages tires au niveau maximal, une fois par round, avec SupplyDrop |
| `announce_levels` | `true` | Annonce chaque nouveau niveau |
| `broadcast_duration` | `4` | Duree de l'annonce, en secondes |

Le niveau repart de zero a chaque round.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `chaos status` | `chaosdirector.manage` | Niveau et points du round |
