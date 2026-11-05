# Plugins LabAPI — Zone-Shilari

Bibliotheque partagee et plugins serveur SCP: Secret Laboratory du serveur francophone
**Zone-Shilari**, portes d'EXILED vers [LabAPI](https://github.com/northwood-studios/LabAPI),
le framework officiel. Aucun composant ne depend plus d'EXILED.

## Composants

| Composant | Version | Role |
|---|---|---|
| [AugatonLib](AugatonLib) | 2.0.0 | Bibliotheque partagee. **Dependance obligatoire** |
| [AutoFFToggle](plugins/AutoFFToggle) | 3.0.0 | Tir allie coupe pendant le round, actif a la fin |
| [BetterCoinflipsRewritten](plugins/BetterCoinflipsRewritten) | 3.0.0 | Pile ou face a effets |
| [RandomGamemode](plugins/RandomGamemode) | 4.0.0 | 17 modes de jeu speciaux |
| [RealisticSizes](plugins/RealisticSizes) | 5.0.0 | Taille variable par joueur |
| [RemoteKeycard](plugins/RemoteKeycard) | 5.0.0 | Ouverture sans sortir sa carte |
| [Replacer](plugins/Replacer) | 3.0.0 | Remplacement des joueurs partis |
| [SCP1162](plugins/SCP1162) | 3.0.0 | Le trou dans le mur, echange d'objets |
| [SCP500s](plugins/SCP500s) | 7.0.0 | 44 pilules personnalisees, dont une roulette |
| [ScpProximityChat](plugins/ScpProximityChat) | 3.0.0 | Chat de proximite pour les SCP |
| [SCPReplacer](plugins/SCPReplacer) | 3.0.0 | Volontariat pour remplacer un SCP parti |
| [SupplyDrop](plugins/SupplyDrop) | 5.0.0 | Largages de ravitaillement |
| [TeamGenocide](plugins/TeamGenocide) | 4.0.0 | Annonces d'extinction d'equipe |

## Compilation

Il faut le SDK .NET 8 ou plus recent, y compris sous Linux.

```bash
./tools/fetch-references.sh    # une fois, puis apres chaque mise a jour du jeu
dotnet build -c Release
```

Les tests se lancent avec `dotnet test tests/AugatonLib.Tests -c Release`, et `./tools/package.sh`
assemble `out/zone-shilari-labapi.zip`, a extraire dans le dossier `LabAPI` du serveur.

Le script telecharge les assemblies du jeu dans `.references/`. Elles ne servent qu'a la compilation
et ne sont ni committees ni deployees. Pour compiler contre votre propre serveur, definissez
`SL_REFERENCES` vers son dossier `SCPSL_Data/Managed`.

| Sortie | Destination serveur |
|---|---|
| `out/plugins/*.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |

Le dossier `LabAPI` se trouve dans `~/.config/SCP Secret Laboratory/` sous Linux et dans
`%appdata%\SCP Secret Laboratory\` sous Windows. `<port>` est le port du serveur, ou `global`.

## Installation

1. Deposez `AugatonLib.dll` dans `dependencies/<port>/`, jamais dans `plugins/`.
2. Deposez les plugins souhaites dans `plugins/<port>/`.
3. Installez [HintServiceMeow](https://github.com/MeowServer/HintServiceMeow/releases) :
   `HintServiceMeow-LabAPI.dll` dans `plugins/<port>/`, et `0Harmony.dll` dans `dependencies/<port>/`.
4. Redemarrez le serveur et surveillez la banniere AugatonLib dans la console.

Les configs sont creees dans `configs/<port>/<Plugin>/` : `config.yml`, `translation.yml` et
`properties.yml`, qui active ou desactive le plugin. Rechargement a chaud : `lapi reload configs`.

## Integration continue

| Workflow | Declencheur | Effet |
|---|---|---|
| `ci` | push sur `main`, pull request | Compile, teste, archive le zip, cherche des secrets avec gitleaks |
| `release` | tag `v*` | Compile, teste et publie le zip dans une release GitHub |

Publier revient a pousser un tag : `git tag v1.0.0 && git push origin v1.0.0`.

## Migration depuis EXILED

Les configs EXILED ne se reprennent pas telles quelles : le dossier change, `is_enabled` passe dans
`properties.yml`, et les valeurs de type `ZoneType`, `RoomType` ou `EffectType` changent de noms. Les
changements propres a chaque plugin figurent dans son README.
