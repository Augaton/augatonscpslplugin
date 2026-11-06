# Profils de serveur

La collection compte 31 plugins, et ils sont tous actifs une fois installés. Zone-Shilari, un serveur arcade, les utilise tous. Un serveur plus posé n'en voudra qu'une partie : ce guide propose trois sélections. Rien n'y est obligatoire, et chaque plugin fonctionne seul avec AugatonLib.

## Désactiver un plugin

Au premier démarrage, LabAPI crée un fichier `properties.yml` par plugin, dans `LabAPI/configs/<port>/<Plugin>/`. Pour couper un plugin, mettez-y :

```yaml
is_enabled: false
```

Redémarrez ensuite le serveur. La bannière de démarrage liste les plugins actifs.

## Les trois profils

- **Calme** : le jeu de base, en plus stable. Aucun objet ni règle ne s'ajoute à la partie. Adapté à un serveur RP.
- **Vivant** : le site réagit et réserve des surprises, sans changer les règles du jeu.
- **Arcade** : tout est actif. C'est le choix de Zone-Shilari.

| Plugin | Calme | Vivant | Arcade |
|---|---|---|---|
| **Protection et confort** | | | |
| AntiSLCrush | ✅ | ✅ | ✅ |
| TickGuard | ✅ | ✅ | ✅ |
| DiscordAudit | ✅ | ✅ | ✅ |
| AutoFFToggle | ✅ | ✅ | ✅ |
| FairScpSelection | ✅ | ✅ | ✅ |
| Replacer | ✅ | ✅ | ✅ |
| SCPReplacer | ✅ | ✅ | ✅ |
| RemoteKeycard | ✅ | ✅ | ✅ |
| ScpProximityChat | ✅ | ✅ | ✅ |
| ScpScaling | ✅ | ✅ | ✅ |
| TeamGenocide | ✅ | ✅ | ✅ |
| RoundRecap | ✅ | ✅ | ✅ |
| **Ambiance du site** | | | |
| AmbientCassie | ✅ | ✅ | ✅ |
| RealisticSizes | ✅ | ✅ | ✅ |
| MicroHidDock | ✅ | ✅ | ✅ |
| PocketCorpses | ✅ | ✅ | ✅ |
| SiteIncidents | — | ✅ | ✅ |
| SupplyDrop | — | ✅ | ✅ |
| **Objets et anomalies** | | | |
| SCP1162 | — | ✅ | ✅ |
| Scp914Recipes | — | ✅ | ✅ |
| Scp261 | — | ✅ | ✅ |
| SCP500s | — | ✅ | ✅ |
| BetterCoinflipsRewritten | — | ✅ | ✅ |
| Contracts | — | ✅ | ✅ |
| **Rythme de partie** | | | |
| RoundMutators | — | — | ✅ |
| RandomGamemode | — | — | ✅ |
| ChaosDirector | — | — | ✅ |
| SpectatorVote | — | — | ✅ |
| **Arcade** | | | |
| ArcadeShop | — | — | ✅ |
| Streaks | — | — | ✅ |
| ScpLevels | — | — | ✅ |
| WarmupLobby | — | — | ✅ |

`WarmupLobby` reste un bon ajout à n'importe quel profil : il ne joue qu'avant le round et ne laisse aucune trace.

## Réglages à ajuster

Pour un profil Vivant :

| Plugin | Réglage | Conseil |
|---|---|---|
| RandomGamemode | `gamemode_chance` | Si vous l'activez, gardez 5 ou moins pour que les modes restent rares |
| RoundMutators | `chance_percent` | Si vous l'activez, gardez 15 ou moins |
| SiteIncidents | `min_interval_seconds` | Montez à 240 pour un round plus posé |
| SiteIncidents, SupplyDrop | `min_event_gap_seconds` | Montez à 120 si les événements s'enchaînent trop |
| SupplyDrop | `trap_chance_percent` | Mettez 0 pour des largages toujours sûrs |

Ces valeurs sont un point de départ et n'ont pas encore été testées en jeu. Pendant une partie, `tickguard status` affiche les TPS les plus bas du round.
