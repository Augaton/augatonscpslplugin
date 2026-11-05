# CLAUDE.md — Plugins LabAPI (SCP: Secret Laboratory)

Plugins serveur pour **Zone-Shilari** (FR, RP). Ce dépôt regroupe AugatonLib et les plugins de la
collection, portés depuis EXILED vers **LabAPI**, le framework officiel. Aucun plugin ne dépend plus
d'EXILED.

Le serveur tourne sur une **boucle Unity mono-thread**. Tout code lourd dans un handler bloque le
tick pour tous les joueurs. Cette contrainte prime sur toutes les autres.

## Stack

| Élément | Valeur |
|---|---|
| Langage, cible | C# 9, `net48` |
| Framework | `Northwood.LabAPI` 1.1.7 (NuGet) |
| Assemblies du jeu | `.references/` (`tools/fetch-references.sh`), ou `SL_REFERENCES` vers `SCPSL_Data/Managed` |
| Hints | HintServiceMeow 5.5, obligatoire, via `plugin.Hints` (`AugatonLib.Hints.HintChannel`) |
| Build | `dotnet build -c Release` à la racine |
| Tests | `dotnet test tests/AugatonLib.Tests -c Release` |
| Sortie | `out/plugins/`, `out/dependencies/`, et `tools/package.sh` pour l'archive |

Les DLL du jeu ne sont jamais committées. La configuration commune vit dans
`Directory.Build.props` et `Directory.Build.targets` : un `.csproj` de plugin ne déclare que son
`RootNamespace` et sa `Version`. Tout projet sauf AugatonLib référence AugatonLib automatiquement.

## Structure

```
AugatonLib/            bibliotheque partagee, deployee dans dependencies/
plugins/<Nom>/         un plugin : Plugin.cs, Config.cs, Translation.cs, Handlers/, Commands/, API/
third_party/<Nom>/     plugin d'un autre auteur, code inchange, hors des regles ci-dessous
```

Un fichier, une responsabilité. Chaque plugin garde son `README.md` et son `NOTICE.md` d'attribution.

## Cycle de vie

Un plugin hérite de `AugatonPlugin<Config, Translation>`, ou de `AugatonPlugin<Config>` s'il n'a aucun
texte à traduire, et fournit `Name`, `Description`, `Author`,
`Enable()` et `Disable()`. `LoadConfigs` est appelé avant `Enable` et par `labapi reload configs` ; il
charge `config.yml` et `translation.yml`, puis appelle `ValidateConfig()`, que l'on surcharge pour
borner les valeurs avec un `Logger.Warn` ou `AtLeast(cle, valeur, minimum)`. Une config qui implémente `IHintSettings` règle `plugin.Hints`
à chaque chargement. L'activation du plugin se règle dans `properties.yml`, créé
par LabAPI : il n'y a pas de clé `IsEnabled` dans la config.

- Tout `+=` dans `Enable` a son `-=` dans `Disable`.
- `Disable` nettoie tout état statique, tout dictionnaire et toute coroutine.
- Pas d'I/O bloquante dans `Enable`.

## Events

- Aucune exception ne remonte d'un handler : `try` / `catch` avec `Logger.Error`.
- `ev.Player` et `ev.Attacker` peuvent être `null` selon le contexte.
- Une logique de jeu qui doit lire un joueur qui part s'abonne à `PlayerDeparture.Departing`, jamais à
  `PlayerEvents.Left`. À `Left`, le rôle vaut `Destroyed` et le joueur a pu être tué par
  `disconnect_drop`. `Left` convient au nettoyage d'état, par `UserId`.

## Coroutines (MEC)

Stocker le `CoroutineHandle`, le tuer dans `Disable` et en fin de round. Intervalle minimal raisonnable :
`0.1f`. Une coroutine par système, pas une par joueur. Il n'existe pas de `OnUpdate`.

## Harmony

Pas de patch Harmony dans un plugin, sauf AntiSLCrush, dont c'est le role. Aucun fichier de ce plugin ne
perd son en-tete de copyright : sa licence l'interdit.

## Performance

En hot path (handler fréquent, coroutine sous la seconde), éviter la LINQ chaînée, `Player.Get` en
boucle, la concaténation de chaînes en boucle, `GameObject.Find`, et l'allocation d'une collection par
tick. Cacher les collections du round au `RoundStarted`. Retirer le joueur des dictionnaires à son
départ. `Logger.Debug` prend `Config.Debug` en second argument. Pas de log par joueur en production.

## Sécurité

- Le client est hostile. Valider type, longueur, plage et format de toute donnée joueur.
- Permissions vérifiées en première ligne : les commandes staff héritent de `StaffCommand` /
  `StaffParentCommand`, qui le font déjà. Nœuds nommés `monplugin.action`, jamais réutilisés.
- Texte joueur affiché à d'autres joueurs : passer par `SafeText`, qui retire le rich text et les
  caractères de contrôle.
- Aucun secret dans le code ni dans une config committée.
- Toute action déclenchable par un non-staff a un cooldown serveur, indexé sur `UserId`, borné par
  `AtLeast` : la config ne doit pas pouvoir le mettre à zéro.
- Toute action staff à impact est tracée avec `Audit(...)`.

## Config et traduction

Toute constante de gameplay va en config, avec `[Description]`. Les textes joueur vont dans
`Translation`. Une config est sérialisée par propriétés, pas par champs.

## Conventions

`PascalCase` public, `camelCase` privé, préfixe `_` interdit. Pas de commentaire, sauf pour expliquer
un *pourquoi* non évident. Early return plutôt qu'imbrication. `readonly` par défaut. `is null` plutôt
que `== null`. Pas de `catch { }` vide. Chaînes en français sans accents, comme dans le code existant.

## Ressources partagées entre plugins

Ces ressources sont arbitrées par AugatonLib et ne s'écrivent jamais en direct :

| Ressource | Arbitre | Interdit |
|---|---|---|
| Tir allié | `FriendlyFireArbiter` | `Server.FriendlyFire = ...` |
| Taille du joueur | `ScaleArbiter` | `player.Scale = ...` |
| Lumières | `LightArbiter` | `Map.TurnOffLights`, `Map.SetColorOfLights`, `OverrideLightsColor` |
| Portes verrouillées | `DoorLockRegistry` | `ev.IsAllowed` sur une porte sans consulter le registre |
| Départ d'un joueur | `DepartureArbiter` | agir sur un départ sans vérifier `IsOwner` |
| Annonces d'extinction | `GenocideArbiter` | annoncer sans vérifier `IsSuppressed` et `IsHeld(team)` |

Chaque plugin dépose une demande sous un nom de propriétaire unique et la libère dans `Disable` **et**
au redémarrage de round : AugatonLib vit dans `dependencies/`, une demande non libérée reste posée.
`GenocideArbiter.Hold` retient une seule équipe le temps qu'un remplaçant arrive ; celui qui retient
publie ensuite `BusTopics.TeamCheckRequested`. Déclarer ses capacités avec `PluginDirectory.Register(this, Capability.Scale, ...)` et
`PluginDirectory.Unregister(this)`. Les synergies optionnelles passent par `PluginBus`, jamais par une
référence directe entre plugins : chaque plugin doit fonctionner seul avec AugatonLib, ce qu'un test
et la CI vérifient. Un plugin absent ne doit changer que la synergie concernée.

Un plugin qui agit sur le monde ou sur les rôles respecte `WorldEvents.IsPaused`, posé par les modes
arène, avec une clé `pause_during_gamemodes`. Tous les modes ne posent pas cette pause : pour les voir
tous, écouter `GamemodeStarted` et `GamemodeEnded`, et remettre le drapeau à zéro en fin de round, jamais
au début, où il partirait sur le même événement que le mode.

## Tests

Ce qui dépend de la carte ou du jeu se déclare dans `SelfCheck.Register`, visible avec `collection check`.
Les règles qui se lisent dans le texte du code vont dans `RepositoryRulesTests`.

`tests/AugatonLib.Tests` tourne sur .NET 10, sans Unity : il ne couvre que le code qui ne touche ni au
jeu ni à `UnityEngine`. Une méthode qui mentionne un type Unity ne peut pas y être chargée, même si la
branche testée n'y passe pas. Le reste se valide en jeu avec `TESTS-EN-JEU.md`.
