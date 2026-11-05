# RealisticSizes 5.0

> Portage LabAPI 1.1.7 d'un plugin de **JesusQC**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Donne a chaque joueur une taille legerement differente a l'apparition.

**LabAPI 1.1.7** — `dotnet build -c Release plugins/RealisticSizes/RealisticSizes.csproj`

## Modes

| Mode | Effet |
|---|---|
| `Roleplay` | Variations discretes, credibles en RP (defaut) |
| `Fun` | Variations larges et visibles |
| `Manual` | Une plage par role, definie dans `manual_ranges` |

`allow_unproportional_values` autorise une largeur differente de la hauteur.
A `false`, la silhouette reste proportionnee.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `affect_scps` | `false` | Deconseille : modifier la taille d'un SCP change ses hitbox |
| `ignored_roles` | Tutorial, Overwatch, Filmmaker, SCP-079 | Roles jamais redimensionnes |
| `apply_delay` | `0.4` | Delai avant application, le temps que le spawn se termine |
| `spread_workload` | `true` | Etale les applications sur plusieurs frames en spawn massif |

Les plages sont des objets typees (`min_height`, `max_height`, `min_width`,
`max_width`) et non plus une chaine `"0.5:0.5::1.15:1.15"`.

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `out/plugins/RealisticSizes.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` n'est pas un plugin : il va dans `dependencies/`, jamais dans
`plugins/`. Le mettre a jour en meme temps que ce plugin.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `sizes status` | `realisticsizes.manage` | Mode actif et reglages courants |
| `sizes reset <joueur\|*>` | `realisticsizes.manage` | Remet la taille normale |

Alias `rsize`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Installation

`dotnet build -c Release` a la racine du depot produit les DLL dans `out/`. Les
copier selon le tableau ci-dessus :

```
LabAPI/plugins/<port>/RealisticSizes.dll
LabAPI/dependencies/<port>/AugatonLib.dll
```

Si plusieurs plugins de la collection sont installes, garder la version
d'AugatonLib la plus recente : elle est partagee par tous.

Les fichiers de configuration sont crees au premier lancement :

```
LabAPI/configs/<port>/RealisticSizes/config.yml
```

Ce plugin n'a aucun texte a traduire, donc pas de `translation.yml`.

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la
cle `is_enabled` n'existe plus.

## Portage LabAPI

Comportement inchange. Les cles de configuration restent les memes, sauf
`is_enabled`, retiree.

`PlayerEvents.ChangedRole` remplace `Spawned`, et non `PlayerEvents.Spawned`.
Ce dernier ne part que pour les roles FPC dotes d'un point d'apparition, avant la
fin de l'initialisation du role. Il manquerait le passage en spectateur et en
SCP-079, qui remettent le joueur a sa taille normale. `ChangedRole` part a chaque
changement de role, comme `Spawned` d'EXILED.

`Player.Scale` de LabAPI passe par le `FpcScaleController` du jeu : il n'agit que
sur les roles FPC et vaut zero ailleurs. La taille d'un spectateur reste donc
memorisee par le jeu jusqu'a son prochain role, ou le plugin la ramene a la
normale.

## Note de portage precedente

La version 3.x ciblait EXILED 2.3.4. Quatre defauts corriges :

- **`float.Parse` sans culture invariante.** Les tailles etaient stockees en
  chaine puis parsees avec la culture du systeme. Sur une machine francaise,
  ou la virgule est le separateur decimal, `"1.15"` ne se parse pas comme
  attendu. Le format chaine a disparu au profit d'un objet typee.
- **Condition inversee.** `if (ev.Player != null || ev.NewRole != RoleType.None)`
  utilisait `||` la ou `&&` etait attendu : un `Player` nul passait la garde et
  provoquait une exception.
- **Course entre joueurs.** Les tailles calculees etaient stockees dans deux
  champs d'instance partages par tous les joueurs. Combine au delai aleatoire de
  0,4 a 5 secondes de l'anti-lag, deux apparitions simultanees s'ecrasaient
  mutuellement. Ce sont maintenant des variables locales.
- **Resynchronisation manuelle.** Le plugin envoyait lui-meme un
  `ObjectDestroyMessage` puis rappelait `SendSpawnMessage` par reflexion.
  `Player.Scale` d'EXILED fait tout cela correctement.

L'anti-lag aleatoire est remplace par un etalement deterministe, et les
callbacks differes sont annules au depart du joueur et au changement de round.

## Avec Replacer

Avec `keep_size_on_replacement`, le remplacant garde la taille du joueur parti au lieu d'en tirer une
nouvelle.

## Effets de la taille

Desactive par defaut, `enable_size_gameplay` donne un petit avantage a chaque gabarit. Il ne touche
que les humains : un petit personnage va plus vite, un grand a plus de vie. Reglages :
`speed_percent_per_tenth`, `health_percent_per_tenth`, `max_speed_percent`, `max_health_percent`.
Avec les valeurs par defaut, une taille de 0,9 donne +3 % de vitesse et -5 % de vie.
