# TickGuard

Mesure les TPS du serveur pendant le round. Quand ils restent trop bas, le plugin alerte la console et
suspend les evenements automatiques le temps que le serveur respire. C'est la meme pause que celle des
modes arene : largages, pilules, pieces, incidents, SCP-1162, SCP-261, remplacements et niveau de chaos.

Les TPS les plus bas du round restent consultables, ce qui donne une vraie mesure de la charge. Avec DiscordAudit,
l'alerte et le retour a la normale arrivent aussi sur le salon du webhook.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `min_tps` | `30` | Seuil de souffrance |
| `recovery_tps` | `45` | TPS a retrouver pour reprendre |
| `check_seconds` | `5` | Secondes entre deux mesures, 1 au plus bas |
| `checks_before_alert` | `3` | Mesures basses de suite avant l'alerte |
| `pause_world_events` | `true` | Suspend les evenements. Si `false`, le plugin ne fait qu'alerter |

Un plugin regle sur `pause_during_gamemodes: false` ignore aussi cette pause.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `tickguard status` | `tickguard.manage` | TPS actuels, plus bas du round et souffrances constatees |

Alias `tps`.
