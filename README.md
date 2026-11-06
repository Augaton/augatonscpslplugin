# Augaton SCP:SL Plugins

Trente et un plugins et une bibliothèque partagée pour **SCP: Secret Laboratory**, conçus pour tourner ensemble sur le même serveur. Ils sont développés pour la communauté francophone **Zone-Shilari** et construits sur [LabAPI](https://github.com/northwood-studios/LabAPI), le framework officiel du jeu.

[![CI](https://github.com/Augaton/augatonscpslplugin/actions/workflows/ci.yml/badge.svg)](https://github.com/Augaton/augatonscpslplugin/actions/workflows/ci.yml)
![LabAPI 1.1.7](https://img.shields.io/badge/LabAPI-1.1.7-blue)
![.NET Framework 4.8](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4)

> [!WARNING]
> **Pré-version.** La collection vient d'être portée d'EXILED vers LabAPI. Elle compile contre le build public du jeu et ses tests automatisés passent, mais elle n'a pas encore été validée sur un serveur en conditions réelles. La liste des vérifications restantes est dans [TESTS-EN-JEU.md](TESTS-EN-JEU.md).

## Sommaire

- [Points forts](#points-forts)
- [Plugins](#plugins)
- [Plugins tiers](#plugins-tiers)
- [Installation](#installation)
- [Configuration](#configuration)
- [Commandes](#commandes)
- [Architecture](#architecture)
- [Développement](#développement)
- [Migration depuis EXILED](#migration-depuis-exiled)
- [Crédits et licences](#crédits-et-licences)
- [Contribuer](#contribuer)

## Points forts

- **Une collection cohérente.** Des plugins qui touchent à la même ressource peuvent se contredire : l'un éteint les lumières pendant qu'un autre les rallume. Ici, les ressources partagées passent par des arbitres communs.
- **À la carte.** Chaque plugin fonctionne seul avec AugatonLib. Installés ensemble, ils se complètent ; absent, un plugin ne désactive que la synergie concernée.
- **100 % LabAPI.** Aucune dépendance à EXILED. Seuls AntiSLCrush et FairScpSelection utilisent des patchs Harmony.
- **Sobre en ressources.** Le serveur tourne sur une boucle Unity à un seul thread, donc le code évite les allocations et les boucles coûteuses dans les chemins fréquents.
- **Configurable.** Chaque plugin a sa config YAML commentée, ses textes séparés dans une traduction et un rechargement à chaud.
- **Sécurisé.** Permissions vérifiées en premier, arguments bornés, actions du staff tracées, et texte des joueurs assaini avant affichage.

## Plugins

| Plugin | Version | Rôle |
|---|---|---|
| [AugatonLib](AugatonLib) | 2.4.0 | Bibliothèque partagée, **obligatoire** |
| [AmbientCassie](plugins/AmbientCassie) | 1.0.0 | Annonces d'ambiance de C.A.S.S.I.E pendant le round |
| [AntiSLCrush](plugins/AntiSLCrush) | 4.0.0 | Protège le serveur contre des bugs du jeu. **Licence non commerciale** |
| [ArcadeShop](plugins/ArcadeShop) | 1.0.0 | Des points gagnés pendant le round, à dépenser dans une boutique |
| [ChaosDirector](plugins/ChaosDirector) | 1.2.0 | Niveau de chaos commun, nourri par les événements du round |
| [AutoFFToggle](plugins/AutoFFToggle) | 3.0.0 | Coupe le tir allié pendant le round et le réactive à la fin |
| [BetterCoinflipsRewritten](plugins/BetterCoinflipsRewritten) | 3.2.0 | Pile ou face aux effets positifs et négatifs, avec des pièces qui reviennent en cours de round |
| [Contracts](plugins/Contracts) | 1.0.0 | Un objectif secret par joueur, récompensé s'il le remplit |
| [DiscordAudit](plugins/DiscordAudit) | 1.0.0 | Envoie les actions du staff vers un webhook Discord |
| [MicroHidDock](plugins/MicroHidDock) | 1.0.0 | Repose le Micro-HID sur son socle, où il se recharge |
| [PocketCorpses](plugins/PocketCorpses) | 1.0.0 | Fait retomber les corps de la dimension de poche |
| [RandomGamemode](plugins/RandomGamemode) | 4.2.0 | 25 modes de jeu spéciaux, avec vote des joueurs |
| [RoundMutators](plugins/RoundMutators) | 1.1.0 | Une règle légère sur certains rounds normaux |
| [RoundRecap](plugins/RoundRecap) | 1.3.0 | Bilan de fin de round |
| [RealisticSizes](plugins/RealisticSizes) | 5.0.1 | Taille variable par joueur, avec effets de gameplay en option |
| [RemoteKeycard](plugins/RemoteKeycard) | 5.0.0 | Utilise une carte d'accès sans la sortir de l'inventaire |
| [Replacer](plugins/Replacer) | 3.1.0 | Remplace un joueur parti par un spectateur, avec son état complet |
| [Scp261](plugins/Scp261) | 2.0.1 | Machine distributrice : une pièce contre un résultat au hasard |
| [Scp914Recipes](plugins/Scp914Recipes) | 1.0.0 | Recettes de SCP-914 réglables en config |
| [SCP1162](plugins/SCP1162) | 3.1.0 | Le trou dans le mur : échange un objet contre un autre |
| [SCP500s](plugins/SCP500s) | 7.1.0 | 44 pilules personnalisées, dont une roulette, renouvelées de plus en plus vite au fil du round |
| [ScpLevels](plugins/ScpLevels) | 1.0.0 | Les SCP montent de niveau en tuant |
| [ScpProximityChat](plugins/ScpProximityChat) | 3.1.0 | Permet aux SCP de parler aux joueurs proches |
| [SCPReplacer](plugins/SCPReplacer) | 3.1.0 | Loterie de volontaires pour reprendre un SCP parti |
| [ScpScaling](plugins/ScpScaling) | 1.0.0 | Adapte les PV des SCP au nombre de joueurs |
| [SiteIncidents](plugins/SiteIncidents) | 1.2.0 | Pannes, alertes, portes bloquées, fuites de gaz, surcharges Tesla et coupures de caméras |
| [SpectatorVote](plugins/SpectatorVote) | 1.0.0 | Les spectateurs votent pour le prochain incident |
| [Streaks](plugins/Streaks) | 1.0.0 | Séries de kills récompensées et primes sur les meilleurs joueurs |
| [SupplyDrop](plugins/SupplyDrop) | 5.1.0 | Largages de ravitaillement, sur minuteur ou événement |
| [TickGuard](plugins/TickGuard) | 1.0.0 | Surveille les TPS et suspend les événements si le serveur souffre |
| [TeamGenocide](plugins/TeamGenocide) | 4.0.0 | Annonces C.A.S.S.I.E à l'extinction d'une équipe |
| [WarmupLobby](plugins/WarmupLobby) | 1.0.0 | Fait combattre les joueurs en attendant le round |

Chaque plugin a son propre README, avec le détail de sa configuration.

## Plugins tiers

Ces plugins viennent d'autres auteurs. Ils restent optionnels, et la bannière de démarrage signale ceux qui sont installés.

| Plugin | Rôle | Installation |
|---|---|---|
| [FairScpSelection](https://github.com/CosmosZvezdo4kin/FairScpSelection) | Choix des SCP selon les préférences des joueurs | **Inclus**, voir [third_party](third_party/FairScpSelection/NOTICE.md) |
| [Better3114Spawn](https://github.com/tayjay/Better3114Spawn) | Fait démarrer SCP-3114 déguisé parmi les humains | À installer depuis ses releases. Lié à l'événement d'Halloween du jeu |
| [ProjectMER](https://github.com/Michal78900/ProjectMER/) | Donne son vrai modèle à SCP-261 | Optionnel, à installer depuis ses releases |

Better3114Spawn n'a pas de licence : son code n'est donc pas copié ici et reste chez son auteur.

PocketCorpses et MicroHidDock reprennent les idées de [CorpsesFallPocketDimension](https://github.com/DentyTxR/CorpsesFallPocketDimension) et [MicroHIDReturn](https://github.com/EngineerLND/MicroHIDReturn), deux plugins sans licence. Ils ont été réécrits sans lire leur code. N'installe pas un original avec sa réécriture : l'effet serait appliqué deux fois.

FairScpSelection remplace le choix des SCP du jeu : sa cohabitation avec Better3114Spawn n'a pas été vérifiée.

## Installation

### Prérequis

- Un serveur dédié SCP: Secret Laboratory. LabAPI est inclus avec le jeu.
- [HintServiceMeow](https://github.com/MeowServer/HintServiceMeow) et Harmony, **obligatoires** et inclus dans l'archive. HintServiceMeow affiche tous les hints de la collection sans qu'ils s'écrasent.

### Compiler

Il faut le [SDK .NET 10](https://dot.net), ainsi que `curl` et `unzip` sous Linux.

```bash
git clone https://github.com/Augaton/augatonscpslplugin.git
cd augatonscpslplugin
./tools/fetch-references.sh
dotnet build -c Release
./tools/package.sh
```

`fetch-references.sh` télécharge les assemblies du jeu dans `.references/`. Elles servent uniquement à la compilation et ne sont ni versionnées ni déployées. Pour compiler contre votre propre serveur, définissez plutôt `SL_REFERENCES` vers son dossier `SCPSL_Data/Managed`. FairScpSelection a besoin de `Assembly-CSharp-Publicized.dll`, présent uniquement dans `.references/` : retirez-le de la solution si vous compilez contre un serveur.

Les fichiers compilés arrivent dans `out/`, et `package.sh` les assemble dans `out/zone-shilari-labapi.zip`. Quand une version sera publiée, la même archive sera disponible dans [Releases](https://github.com/Augaton/augatonscpslplugin/releases).

### Déployer

Le dossier `LabAPI` se trouve ici :

| Système | Emplacement |
|---|---|
| Linux | `~/.config/SCP Secret Laboratory/LabAPI/` |
| Windows | `%appdata%\SCP Secret Laboratory\LabAPI\` |
| Panel (Pelican, Pterodactyl) | en général `/home/container/.config/SCP Secret Laboratory/LabAPI/` |

1. Extrayez `zone-shilari-labapi.zip` dans ce dossier. Vous obtenez :

   ```
   LabAPI/
   ├── plugins/global/*.dll                  plugins et HintServiceMeow
   ├── dependencies/global/AugatonLib.dll
   ├── dependencies/global/0Harmony.dll
   └── licenses/                             licences des composants tiers
   ```

2. Redémarrez le serveur. La console affiche une bannière AugatonLib avec les plugins enregistrés, les intégrations détectées et les éventuels conflits.

Pour viser un seul serveur, remplacez `global` par son port.

### Installer à la carte

Rien n'oblige à tout prendre. Chaque plugin fonctionne seul avec ses trois dépendances obligatoires : `AugatonLib.dll` et `0Harmony.dll` dans `dependencies/`, et `HintServiceMeow.dll` dans `plugins/`. Gardez uniquement les DLL qui vous intéressent, ou téléchargez-les une par une depuis une release. Un plugin absent ne désactive que la synergie concernée. Par exemple, SupplyDrop continue de larguer sans SCP500s, simplement sans pilules.

## Configuration

Les fichiers sont créés au premier démarrage, dans `LabAPI/configs/<port>/<Plugin>/` :

| Fichier | Contenu |
|---|---|
| `config.yml` | Réglages du plugin, avec une description pour chaque clé |
| `translation.yml` | Textes affichés aux joueurs, pour les plugins qui en ont |
| `properties.yml` | Activation ou désactivation du plugin, géré par LabAPI |

Appliquez vos changements à chaud avec `labapi reload configs`. Les permissions du staff se règlent dans `LabAPI/configs/permissions.yml`.

## Commandes

### Joueurs

À taper dans la console du jeu (touche `~`).

| Commande | Plugin | Effet |
|---|---|---|
| `.noreplace` (`.nr`) | Replacer | Refuse ou accepte d'être choisi comme remplaçant pendant le round |
| `.votemode [numéro]` (`.vm`) | RandomGamemode | Affiche les choix ou vote pour le prochain mode de jeu |
| `.volunteer [numéro]` (`.v`) | SCPReplacer | Se porte volontaire pour reprendre un SCP parti |
| `.human` | SCPReplacer | Renonce à son rôle de SCP en début de round |
| `.shop [nom]` (`.boutique`) | ArcadeShop | Affiche la boutique ou achète un objet |
| `.contrat` (`.contract`) | Contracts | Rappelle votre contrat secret et son avancement |
| `.voteincident [numéro]` (`.vi`) | SpectatorVote | Affiche les choix ou vote pour le prochain incident, réservé aux spectateurs |

### Staff

Disponibles dans le Remote Admin et la console du serveur. Sans argument, une commande liste ses sous-commandes.

| Commande | Alias | Permission | Sous-commandes principales |
|---|---|---|---|
| `ambient` | `amb` | `ambientcassie.manage` | `status`, `play` |
| `anticrash` | `asc` | `anticrash.manage` | `status` |
| `arcadeshop` | `ashop` | `arcadeshop.manage` | `status`, `give` |
| `autoff` | `ff` | `autofftoggle.manage` | `status`, `set` |
| `chaos` | `chaosdirector` | `chaosdirector.manage` | `status` |
| `coinflip` | `bcf` | `bettercoinflips.manage` | `status` |
| `gamemode` | `gm` | `randomgamemode.manage` | `list`, `force`, `start`, `stop` |
| `discordaudit` | `daudit` | `discordaudit.manage` | `status`, `test` |
| `incidents` | `inc` | `siteincidents.manage` | `status`, `trigger` |
| `microdock` | `mdock` | `microhiddock.manage` | `status`, `test` |
| `contracts` | `ctr` | `contracts.manage` | `status` |
| `mutators` | `mutator`, `mut` | `roundmutators.manage` | `status`, `force`, `stop` |
| `pocketcorpses` | `corpses` | `pocketcorpses.manage` | `status`, `test` |
| `sizes` | `rsize` | `realisticsizes.manage` | `status`, `reset` |
| `remotekeycard` | `rk` | `remotekeycard.manage` | `status` |
| `replacer` | `rep` | `replacer.manage` | `status` |
| `scp1162` | `1162` | `scp1162.manage` | `status`, `spawn`, `despawn` |
| `scp261` | `261` | `scp261.manage` | `status` |
| `scp500` | `pills` | `scp500s.give` | `list`, `give` |
| `scplevels` | `levels` | `scplevels.manage` | `status`, `add` |
| `scpproximity` | `prox` | `scpproximitychat.manage` | `status` |
| `scpreplacer` | `scprep` | `scpreplacer.manage` | `status` |
| `scpscaling` | `scaling` | `scpscaling.manage` | `status` |
| `spectatorvote` | `svote` | `spectatorvote.manage` | `status`, `open` |
| `streaks` | `streak` | `streaks.manage` | `status`, `add` |
| `supplydrop` | `sd` | `supplydrop.call` | `list`, `call` |
| `teamgenocide` | `tg` | `teamgenocide.manage` | `status` |
| `tickguard` | `tps` | `tickguard.manage` | `status` |
| `warmup` | `lobby` | `warmuplobby.manage` | `status` |

La sous-commande `collection` affiche l'état de toute la collection : plugins chargés, intégrations et arbitres.

`collection check` vérifie sur le vrai serveur ce que la compilation ne voit pas : prefabs, salles, mots de C.A.S.S.I.E, codec et dépendances. Lancez-la pendant un round, par exemple avec `chaos collection check`.

## Architecture

```mermaid
flowchart TD
    P["Plugins"] --> A["AugatonLib"]
    A --> L["LabAPI"]
    A -. hints .-> H["HintServiceMeow"]
    L --> G["Serveur SCP:SL"]
```

Les plugins ne se référencent jamais entre eux. Ils partagent AugatonLib, qui fournit :

| Service | Rôle |
|---|---|
| Arbitres | Départagent les plugins qui écrivent la même ressource |
| `PluginBus` | Fait communiquer les plugins sans dépendance directe |
| `PlayerDeparture` | Signale un départ tant que l'état du joueur est encore lisible |
| `HintChannel` | Donne à chaque plugin son canal de hints |
| `Escalation`, `LootSpots` | Font monter le chaos par paliers et réapparaître des objets à des endroits sûrs |
| `StaffCommand` | Socle commun : permission, bornage des arguments, audit |
| `SafeText` | Assainit tout texte venant d'un joueur |

Six ressources sont arbitrées, et une pause commune suspend largages, pilules, pièces et incidents pendant les modes de type arène :

| Ressource | Arbitre |
|---|---|
| Tir allié | `FriendlyFireArbiter` |
| Taille du joueur | `ScaleArbiter` |
| Lumières et teintes | `LightArbiter` |
| Portes verrouillées | `DoorLockRegistry` |
| Départ d'un joueur | `DepartureArbiter` |
| Annonces d'extinction | `GenocideArbiter` |

Le bus sert par exemple à SupplyDrop pour demander des pilules à SCP500s, ou à SCPReplacer pour faire patienter TeamGenocide : le dernier SCP qui part n'est pas annoncé comme éliminé tant qu'un volontaire peut le reprendre.

| Si... | Alors |
|---|---|
| RandomGamemode est présent | Les modes arène suspendent largages, pilules, pièces, incidents, SCP-1162, remplacements et chaos |
| RandomGamemode et SCPReplacer sont présents | `.human` et `.volunteer` sont coupés pendant un mode |
| Replacer ou SCPReplacer, et RealisticSizes, sont présents | Le remplaçant garde la taille du joueur parti |
| SCP500s et SupplyDrop sont présents | Les largages contiennent des pilules |
| SCP500s et RandomGamemode sont présents | Soirée pilules distribue les vraies pilules |
| SCP500s et Scp261 sont présents | La machine distribue les vraies pilules |
| BetterCoinflips et Scp261 sont présents | Les pièces renouvelées alimentent la machine |
| ChaosDirector est présent | Les pilules, pièces et incidents s'accélèrent avec l'agitation du round |
| RoundRecap est présent | Le bilan compte ce que font les autres plugins |
| TickGuard est présent | Une baisse durable de TPS suspend les mêmes événements qu'un mode arène |
| DiscordAudit est présent | Les actions tracées par chaque plugin arrivent sur Discord |
| Scp914Recipes et SCP500s sont présents | Les recettes de SCP-914 donnent de vraies pilules |
| Streaks ou ArcadeShop, et SCP500s, sont présents | Les récompenses et achats `SCP500` sont de vraies pilules |
| Streaks et ChaosDirector sont présents | Les paliers de série font monter le chaos |
| Streaks et RoundRecap sont présents | Le bilan nomme la meilleure série de kills |
| Contracts et SCP500s sont présents | La récompense `SCP500` d'un contrat est une vraie pilule |
| Contracts et ArcadeShop sont présents | Un contrat rempli rapporte des points de boutique |
| SpectatorVote et SiteIncidents sont présents | Les spectateurs votent pour le prochain incident. Sans SiteIncidents, aucun vote ne s'ouvre |
| ScpScaling et ScpLevels sont présents | Les PV de niveau s'ajoutent à des PV déjà adaptés au nombre de joueurs |
| Un plugin manque | Seule la synergie concernée disparaît |

Une CI vérifie qu'aucun plugin n'en référence un autre.

Le détail est dans le [README d'AugatonLib](AugatonLib/README.md).

## Développement

### Structure

```
AugatonLib/            bibliothèque partagée
plugins/<Nom>/         un dossier par plugin
third_party/           plugins d'autres auteurs, code inchangé
tests/                 tests automatisés
tools/                 récupération des références et assemblage de l'archive
```

`Directory.Build.props` et `Directory.Build.targets` portent toute la configuration commune : un `.csproj` de plugin ne déclare que son espace de noms et sa version.

### Compiler et tester

```bash
dotnet build -c Release
dotnet test tests/AugatonLib.Tests -c Release
```

Les tests tournent sans le jeu : ils couvrent la logique indépendante d'Unity, comme `SafeText`, `PluginBus`, l'urne de vote ou les paliers de chaos. Le reste se valide sur un serveur.

### Ajouter un plugin

1. Créez `plugins/<Nom>/<Nom>.csproj`, avec seulement `RootNamespace` et `Version`.
2. Créez une classe qui hérite de `AugatonPlugin<Config, Translation>`, ou de `AugatonPlugin<Config>` s'il n'a aucun texte.
3. Déclarez ses capacités avec `PluginDirectory.Register`, et retirez-les dans `Disable`.
4. Ajoutez-le au catalogue dans `AugatonLib/Runtime/KnownPlugin.cs`. Un test vérifie que chaque dossier de plugin y figure.
5. Ajoutez le projet à `augatonscpslplugin.slnx`.

Les conventions du projet sont dans [CLAUDE.md](CLAUDE.md).

### Intégration continue

| Workflow | Déclencheur | Effet |
|---|---|---|
| `ci` | push sur `main`, pull request | Compile, teste, produit l'archive et cherche des secrets avec gitleaks |
| `release` | tag `v*` | Compile, teste et publie l'archive dans une release GitHub |

## Migration depuis EXILED

Les configs EXILED ne se reprennent pas telles quelles :

- Le dossier change : elles vivent maintenant dans `LabAPI/configs/<port>/<Plugin>/`.
- `is_enabled` disparaît au profit de `properties.yml`.
- Les valeurs `ZoneType`, `RoomType` et `EffectType` changent de noms.

Pensez à retirer les anciennes DLL de `.config/EXILED/Plugins/`, sinon les deux versions tourneraient en parallèle. Les changements propres à chaque plugin figurent dans son README.

## Crédits et licences

Les plugins portés sont des réécritures. Leur conception et leurs mécaniques reviennent à leurs auteurs d'origine, cités en jeu dans le champ `Author` de chaque plugin.

| Composant | Auteur(s) d'origine | Licence |
|---|---|---|
| AugatonLib, tests et outillage | Augaton | [MIT](LICENSE) |
| AmbientCassie, ArcadeShop, ChaosDirector, Contracts, DiscordAudit, RoundMutators, RoundRecap, Scp914Recipes, ScpLevels, ScpScaling, SiteIncidents, SpectatorVote, Streaks, TickGuard, WarmupLobby | Augaton | [MIT](plugins/ChaosDirector/LICENSE) |
| MicroHidDock | Augaton, d'après une idée d'EngineerLND | [MIT](plugins/MicroHidDock/LICENSE) |
| PocketCorpses | Augaton, d'après une idée de DentyTxR | [MIT](plugins/PocketCorpses/LICENSE) |
| AntiSLCrush | ui_2506, version modifiée | [Non commerciale](plugins/AntiSLCrush/LICENSE.md) |
| AutoFFToggle | Kognity | [MIT](plugins/AutoFFToggle/LICENSE) |
| BetterCoinflipsRewritten | Mikihero, Matisiowy | [Non précisée](plugins/BetterCoinflipsRewritten/NOTICE.md) |
| RandomGamemode | OPGman | [MIT](plugins/RandomGamemode/LICENSE) |
| RealisticSizes | JesusQC | [Non précisée](plugins/RealisticSizes/NOTICE.md) |
| RemoteKeycard | Beryl | [Non précisée](plugins/RemoteKeycard/NOTICE.md) |
| Replacer | DGvagabond | [Non précisée](plugins/Replacer/NOTICE.md) |
| SCP1162 | [MrAfitol](https://github.com/MrAfitol/SCP-1162), d'après le [plugin Synapse](https://github.com/SynapseSL/Scp1162) | [Non précisée](plugins/SCP1162/NOTICE.md) |
| Scp261 | MedveMarci, version modifiée | [MIT](plugins/Scp261/LICENSE) |
| SCP500s | VividZap, Lumi | [MIT](plugins/SCP500s/LICENSE) |
| ScpProximityChat | Bolton | [Non précisée](plugins/ScpProximityChat/NOTICE.md) |
| SCPReplacer | Jon M | [Non précisée](plugins/SCPReplacer/NOTICE.md) |
| SupplyDrop | Wafel, KadeDev, JesusQC | [Non précisée](plugins/SupplyDrop/NOTICE.md) |
| TeamGenocide | Heisenberg3666 | [MIT](plugins/TeamGenocide/LICENSE) |
| FairScpSelection | CosmosZvezdochkin | [Apache 2.0](third_party/FairScpSelection/LICENSE.txt) |
| HintServiceMeow, inclus dans l'archive | MeowServer | [MIT](third_party/licenses/HintServiceMeow-LICENSE.txt) |
| Harmony, inclus dans l'archive | Andreas Pardeike | [MIT](third_party/licenses/Harmony-LICENSE.txt) |

« Non précisée » signifie qu'aucun fichier de licence n'accompagnait les sources d'origine. Ce dépôt n'accorde alors aucun droit supplémentaire sur ces plugins : ils sont publiés comme travaux dérivés à des fins communautaires, avec attribution. La licence MIT du dépôt ne s'y applique pas.

**Auteurs d'origine :** si vous souhaitez qu'un plugin soit retiré, renommé ou placé sous une autre licence, ouvrez une [issue](https://github.com/Augaton/augatonscpslplugin/issues). Nous donnerons suite sans discussion.

Ce projet n'est pas affilié à Northwood Studios.

## Contribuer

Les retours sont les bienvenus, en particulier sur les points de [TESTS-EN-JEU.md](TESTS-EN-JEU.md).

- **Bug :** ouvrez une [issue](https://github.com/Augaton/augatonscpslplugin/issues) avec la version du jeu, le plugin concerné et l'erreur de la console.
- **Code :** ouvrez une pull request. La CI doit passer, et tout `+=` sur un event dans `Enable` a besoin de son `-=` dans `Disable`.
