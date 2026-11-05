# SCP-1162 3.0

> Portage LabAPI 1.1.7 d'un plugin de **MrAfitol**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Ajoute **SCP-1162**, le trou dans le mur. Un objet lache a proximite disparait
et le joueur recupere autre chose a la place — ou perd ses mains.

**LabAPI 1.1.7** — `dotnet build -c Release plugins/SCP1162/SCP1162.csproj`

## Fonctionnement

L'anomalie est un cylindre noir plaque contre un mur, place au debut du round
selon `spawn_chance`. Elle n'intercepte rien tant que le round n'a pas commence.

Quand un joueur lache ou lance un objet a moins de `use_distance` metres, le
plugin annule le drop et resout l'echange :

| Etape | Reglage | Effet |
|---|---|---|
| 1. Cooldown | `cooldown_seconds` | Sous le delai, le drop passe normalement |
| 2. Mains arrachees | `cut_hands_chance` | Effet `SeveredHands`, l'objet tombe quand meme |
| 3. Objet avale | `delete_chance` | L'objet disparait sans contrepartie |
| 4. Echange | `rewards` | L'objet est remplace par un tirage pondere |

`ignore_thrown_items` retire l'etape 2 pour les objets lances a distance : jeter
l'objet reste le moyen sur de tenter sa chance.

Les armes rendues arrivent chargeur plein (`fill_magazine`) et avec des
accessoires tires au hasard (`random_attachments`).

## Emplacements

`locations` accepte plusieurs emplacements ; `spawn_count` fixe combien sont
utilises simultanement, tires au hasard dans la liste.

```yml
locations:
- room: Lcz173
  offset:
    x: 17
    y: 13
    z: 3.59
  rotation:
    x: 90
    y: 0
    z: 0
  scale:
    x: 1.3
    y: 0.1
    z: 1.3
spawn_count: 1
```

`offset` et `rotation` sont exprimes dans le repere local de la salle. Si
plusieurs salles correspondent sur la carte, l'une est tiree au sort.
Une salle absente est signalee dans les logs et l'emplacement est ignore.

## Recompenses

`rewards` est une liste ponderee : le poids fixe la frequence relative de chaque
objet. Un poids nul ou negatif retire l'objet du tirage sans avoir a supprimer
la ligne.

```yml
rewards:
- item: Coin
  weight: 20
- item: SCP500
  weight: 1
```

`item_names` porte les noms affiches aux joueurs. Un objet absent de la liste
garde son nom technique.

## Exclusions

| Reglage | Defaut | Role |
|---|---|---|
| `pause_during_gamemodes` | `true` | Suspend l'anomalie pendant un mode arene |
| `ignored_roles` | `Scp3114` | Roles qui ne peuvent pas utiliser l'anomalie |
| `ignored_items` | `SCP330`, `MicroHID` | Objets que l'anomalie refuse d'avaler |

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `out/plugins/SCP1162.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` n'est pas un plugin : il va dans `dependencies/`, jamais dans
`plugins/`. Le mettre a jour en meme temps que ce plugin.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `scp1162 status` | `scp1162.manage` | Etat de l'anomalie et reglages actifs |
| `scp1162 spawn` | `scp1162.manage` | Place l'anomalie |
| `scp1162 despawn` | `scp1162.manage` | Retire l'anomalie |

Alias `1162`. Les deux actions a impact sont tracees avec l'auteur.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Installation

`dotnet build -c Release` a la racine du depot produit les DLL dans `out/`. Les
copier selon le tableau ci-dessus :

```
LabAPI/plugins/<port>/SCP1162.dll
LabAPI/dependencies/<port>/AugatonLib.dll
```

Si plusieurs plugins de la collection sont installes, garder la version
d'AugatonLib la plus recente : elle est partagee par tous.

Les fichiers de configuration sont crees au premier lancement :

```
LabAPI/configs/<port>/SCP1162/config.yml
LabAPI/configs/<port>/SCP1162/translation.yml
```

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la
cle `is_enabled` n'existe plus.

## Portage LabAPI

`room` change de format : la cle prend un `MapGeneration.RoomName`, et non plus
un `RoomType` d'EXILED. La valeur par defaut `Lcz173` reste valide. Une valeur
inconnue rend `config.yml` illisible, et LabAPI repart alors des valeurs par
defaut.

| EXILED | LabAPI |
|---|---|
| `LczGlassBox` | `LczGlassroom` |
| `LczCafe` | `LczComputerRoom` |
| `LczPlants` | `LczGreenhouse` |
| `HczHid` | `HczMicroHID` |
| `HczNuke` | `HczWarhead` |
| `Surface` | `Outside` |

Cette table suit le sens des noms : la valeur portee par chaque salle est
stockee dans les prefabs du jeu. Une salle introuvable est signalee dans les
logs.

Les couloirs n'ont pas de nom cote jeu et valent tous `Unnamed`. Deux cles
optionnelles les distinguent, par exemple pour l'ancien `LczCurve` :

```yml
locations:
- room: Unnamed
  zone: LightContainment
  shape: Curve
```

| Cle | Defaut | Role |
|---|---|---|
| `zone` | `None` | `MapGeneration.FacilityZone`. `None` accepte toutes les zones |
| `shape` | `Undefined` | `MapGeneration.RoomShape`. `Undefined` accepte toutes les formes |

Les variantes d'un meme couloir, comme `HczStraightC` et `HczStraightVariant`,
ne se distinguent plus : l'une ou l'autre peut etre tiree.

Les autres cles sont inchangees, sauf `is_enabled`, retiree. Les armes rendues
transmettent leurs accessoires au client par `FirearmItem.AttachmentsCode`, et le
chargeur passe par `FirearmItem.StoredAmmo`. `PlayerEvents.DroppingItem` part
apres le test `AllowDropping` du jeu : un objet que le jeu refuse de lacher n'est
plus echange.

## Note de portage precedente

La version d'origine ciblait LabAPI directement. Cette version 2.0 est une
reecriture complete sur EXILED.

| Point | Version 1.x | Version 2.0 |
|---|---|---|
| API | LabAPI (`Plugin<Config>`) | EXILED (`Plugin<Config, Translation>`) |
| Toy | `SimplifiedToy`, instanciation manuelle du prefab et `NetworkServer.Spawn` | `Exiled.API.Features.Toys.Primitive` |
| Salle cible | `RoomIdentifier.AllRoomIdentifiers.First(...)` a chaque spawn | `Room.List` filtre par `RoomType`, tirage au sort |
| Recompenses | liste plate, tirage uniforme | liste ponderee |
| Textes | figes dans la config, en anglais | fichier `Translation`, en francais |
| Hints | `player.SendHint`, ecrases par les autres plugins | canal HintServiceMeow dedie, voir [HINTS.md](../HINTS.md) |
| Exclusions | un booleen pour SCP-3114, sac a bonbons code en dur | `ignored_roles` et `ignored_items` |

### Correctifs

- **Anomalie jamais nettoyee.** Le toy n'etait detruit que par la commande
  `despawn` : ni le redemarrage de round ni `Disable()` ne le retiraient, et le
  champ statique qui le reference n'etait pas remis a zero. Un `reload` a chaud
  laissait donc le cylindre sur la carte tout en croyant l'avoir toujours.
  Le nettoyage est desormais fait au redemarrage de round et dans `OnDisabled`.
- **Objet intermediaire inutile a chaque spawn.** Un `GameObject` `Point-1162`
  etait cree, parente a la salle pour convertir l'offset local en position
  monde, puis detruit dans la foulee. Le calcul passe maintenant par
  `Room.WorldPosition`.
- **`First()` sur une salle absente.** `AllRoomIdentifiers.First(x => ...)`
  levait `InvalidOperationException` — jamais `null`, contrairement au test qui
  suivait — quand la salle n'existait pas sur la carte generee. Le plugin
  s'arretait avant d'avoir place quoi que ce soit.
- **Tirages incoherents.** Les trois jets utilisaient trois plages differentes
  (`Random.Range(0, 100)` pour l'apparition et les mains, `Random.Range(1, 101)`
  pour la suppression), ce qui decalait les pourcentages annonces. Tous passent
  par le meme tirage.
- **Inventaire plein.** `player.AddItem(...)` pouvait renvoyer `null`, dereference
  aussitot pour le test d'arme. L'objet consomme etait perdu et une exception
  remontait dans la chaine d'evenements.
- **Duplication possible.** `RemoveItem` renvoie un booleen que la version
  d'origine ignorait : si le retrait echouait, le joueur gardait son objet et
  recevait quand meme la recompense. La recompense n'est plus accordee tant que
  le retrait n'est pas confirme.
- **Aucune capture d'exception.** Les handlers ne protegeaient rien : une erreur
  dans le traitement d'un drop cassait la chaine pour les autres plugins.
- **Pas de cooldown.** Un joueur pouvait vider son inventaire objet par objet en
  quelques secondes. `cooldown_seconds` borne la cadence, indexe sur `UserId`.
- **Config non validee.** Un pourcentage a 500 ou une distance negative etaient
  acceptes tels quels. Les valeurs sont bornees au demarrage avec un avertissement.
