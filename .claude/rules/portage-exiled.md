---
paths:
  - "plugins/**"
  - "AugatonLib/**"
---

# Portage EXILED vers LabAPI

But : même comportement en jeu, écrit en LabAPI natif. `plugins/Replacer` est le port de référence.

## Toujours vérifier le point de déclenchement

Un event de même nom peut partir à un autre moment du flux de jeu. C'est le risque principal du
portage : le code compile et ne marche plus. Quand un handler lit un état (rôle, inventaire, position),
comparer où EXILED déclenche son event et où le jeu appelle l'event LabAPI.

Exemple : `Exiled.Player.Left` part au début de `CustomNetworkManager.OnServerDisconnect`, alors que
`PlayerEvents.Left` part dans `ReferenceHub.OnDestroy`, après le kill de `disconnect_drop` et le passage
du rôle à `Destroyed`.
`AugatonLib.Runtime.PlayerDeparture.Departing` rétablit la sémantique d'EXILED.

Exemple : `Exiled.Server.RoundStarted` écoute `CharacterClassManager.OnRoundStarted`, alors que
`ServerEvents.RoundStarted` part avant l'attribution des rôles. Un handler de début de round qui lit
un rôle, une équipe ou un joueur en jeu s'abonne à `CharacterClassManager.OnRoundStarted`.

`LoadConfigs` remplace l'objet `Config` au rechargement, là où EXILED copiait dedans : un handler lit
`plugin.Config` au moment de l'usage et n'en garde aucune référence. `LoadConfigs` tourne à l'amorçage :
pas de `GetTeam()` ni de `TryGetRoleTemplate` dans `ValidateConfig`, qui chargeraient les prefabs.

## Plugin

| EXILED | LabAPI |
|---|---|
| `Plugin<Config, Translation>` | `AugatonPlugin<Config, Translation>`, ou `AugatonPlugin<Config>` sans texte |
| `OnEnabled` + `base.OnEnabled()` | `Enable()` sans appel de base |
| `OnDisabled` + `base.OnDisabled()` | `Disable()` sans appel de base |
| `Prefix`, `RequiredExiledVersion`, `Version` | à retirer ; ajouter `Description` |
| `ServerEvents.ReloadedConfigs` + `ValidateConfig()` | `protected override void ValidateConfig()` |
| `IConfig`, `IsEnabled`, `ITranslation` | à retirer |

## Events

`LabApi.Events.Handlers.PlayerEvents` / `ServerEvents`, arguments dans
`LabApi.Events.Arguments.PlayerEvents` / `.ServerEvents`. Abonnement direct en `+=`.

| EXILED | LabAPI |
|---|---|
| `Player.Left` (lit l'état du joueur) | `PlayerDeparture.Departing` (`Action<Player>`) |
| `Player.Left` (nettoyage seul) | `PlayerEvents.Left` |
| `Server.RoundStarted` (lit les rôles) | `CharacterClassManager.OnRoundStarted` |
| `Server.RoundStarted` (remise à zéro seule) | `ServerEvents.RoundStarted` |
| `Player.Verified` | `PlayerEvents.Joined` |
| `Player.Died` | `PlayerEvents.Death` |
| `Player.ThrownProjectile` | `PlayerEvents.ThrewProjectile` (part après le spawn réseau) |
| `Player.TogglingNoClip` | `PlayerEvents.TogglingNoclip` |
| `Player.VoiceChatting` | `PlayerEvents.SendingVoiceMessage` |
| `Player.ActivatingWarheadPanel` | `PlayerEvents.UnlockingWarheadButton` |
| `Player.StoppingGenerator` | `PlayerEvents.DeactivatingGenerator` |
| `Server.RestartingRound` | `ServerEvents.RoundRestarted` |
| `Server.EndingRound` | `ServerEvents.RoundEndingConditionsCheck` (annuler `RoundEnding` laisse `IsRoundEnded` à vrai) |
| `Server.RespawningTeam` | `ServerEvents.WaveRespawning` |
| `Player.Spawned` | `PlayerEvents.ChangedRole` (`Spawned` ne part que pour les rôles FPC avec point d'apparition) |
| `Map.PickupAdded` / `PickupDestroyed` | `ItemPickupBase.OnPickupAdded` / `OnPickupDestroyed` (`PickupCreated` ne part pas si le wrapper existe déjà) |
| `Map.Decontaminating` | `ServerEvents.LczDecontaminationStarting` |

## API courante

`using LabApi.Features.Wrappers;` donne `Player`, `Item`, `Pickup`, `Room`, `Door`, `Map`, `Round`, `Server`,
`Cassie`, `Warhead`. `Logger` vient de `LabApi.Features.Console` ; si le fichier importe aussi `UnityEngine`,
ajouter `using Logger = LabApi.Features.Console.Logger;`.

| EXILED | LabAPI |
|---|---|
| `Log.Info/Warn/Error` | `Logger.Info/Warn/Error` |
| `Log.Debug(x)` | `Logger.Debug(x, Config.Debug)` |
| `Player.List` | `Player.ReadyList` (exclut l'hôte et les non authentifiés) |
| `Player.List.Count` | `Player.Count` |
| `player.IsConnected` | `!player.IsDestroyed` (`IsOffline` est obsolète) |
| `player.IsVerified` | `player.IsReady` |
| `player.IsScp` | `player.IsSCP` |
| `player.Role.Type` | `player.Role` |
| `player.Role.Team` | `player.Team` |
| `player.Role.Set(r, reason, flags)` | `player.SetRole(r, RoleChangeReason.X, flags)` |
| `player.Role is Scp079Role r` | `player.RoleBase is Scp079Role r` (type du jeu, via `SubroutineModule`) |
| `player.CurrentRoom` | `player.Room` |
| `player.ShowHint(t, d)` | `plugin.Hints.Show(player, t, d)`, sinon `player.SendHint(t, d)` |
| `player.Broadcast(d, t)` | `player.SendBroadcast(t, d)` |
| `Map.Broadcast(d, t)` | `Server.SendBroadcast(t, d)` |
| `player.EnableEffect(EffectType.X, i, d)` | `player.EnableEffect<X>(i, d)` (`CustomPlayerEffects`) |
| `EffectType` en config | nom de classe en `string`, via `AugatonLib.Effects.EffectLookup.TryGet`, jamais `player.TryGetEffect` qui compare avec `StartsWith` |
| `Round.IsStarted`, `IsEnded`, `ElapsedTime` | `Round.IsRoundStarted`, `IsRoundEnded`, `Duration` |
| `Map.IsLczDecontaminated` | `Decontamination.IsDecontaminating` |
| `ZoneType` | `MapGeneration.FacilityZone` (enum simple, pas de drapeaux ; poche = `Other`) |
| `RoomType` | `room.Name` (`MapGeneration.RoomName`) et `room.Zone`. Attention : les couloirs sont tous `Unnamed` |
| `DoorType` | `door.DoorName` (`LabApi.Features.Enums.DoorName`) |
| `KeycardPermissions` | `Interactables.Interobjects.DoorUtils.DoorPermissionFlags` |
| `AmmoType` | `ItemType` (`Ammo556x45`, ...) |
| `SpawnReason` | `PlayerRoles.RoleChangeReason` |
| `role.GetFullName()` | `AugatonLib.Text.RoleNames.Full(role)` |
| `role.GetRandomSpawnLocation()` | `AugatonLib.Spawning.RoleSpawn.TryGet(...)` |
| `sender.CheckPermission(p)` | `sender.HasPermissions(p)` (`LabApi.Features.Permissions`) |
| `Player.Get(string)` | commande : `StaffCommand.TryFindPlayer`. Sinon `Player.Get(userId)` |

## Dépendances sans équivalent

- `Exiled.CustomItems` : LabAPI n'a pas d'objets personnalisés. Suivre soi-même les numéros de série
  (`item.Serial`, `pickup.Serial`) ; un numéro de série survit au passage objet / pickup.
- `Exiled.CustomRoles` : pas d'équivalent ; retirer l'intégration optionnelle.
- Accès publicisés : on compile contre le vrai `Assembly-CSharp.dll`. Un membre privé du jeu n'est pas
  accessible. Passer par l'API publique, jamais par la réflexion sans nécessité.

## Déploiement

| Fichier | Destination |
|---|---|
| `out/plugins/*.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livré avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

Les configs deviennent `LabAPI/configs/<port>/<Plugin>/config.yml` et `translation.yml`. Elles sont à
recréer, car les clés `ZoneType`, `RoomType` et `EffectType` ont changé de valeurs.
