# TeamGenocide 4.0

> Portage LabAPI 1.1.7 d'un plugin de **Heisenberg3666**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Annonce l'extinction complete d'une equipe : C.A.S.S.I.E, broadcast, hint et
effet lumineux. Seule la **mort** du dernier membre declenche l'annonce : une
evasion ou un changement de role force par le staff ne compte pas comme une
extinction. Une deconnexion compte si `disconnect_drop` est actif (defaut), car
le jeu tue alors le joueur qui part.

**LabAPI 1.1.7** — `dotnet build -c Release plugins/TeamGenocide/TeamGenocide.csproj`

## Configuration

`announcements` associe une equipe a une liste d'annonces ; une entree est tiree
au hasard a chaque declenchement. Cinq equipes sont couvertes par defaut :
Classe-D, SCP, scientifiques, Insurrection du Chaos et forces de la Fondation.

| Cle | Defaut | Role |
|---|---|---|
| `activation_delay` | `5` | Delai apres le debut du round avant activation de la detection |
| `announce_once_per_team` | `true` | Une seule annonce par equipe et par round |
| `hint_y_coordinate` | `500` | Ligne HintServiceMeow reservee a ce plugin |

Chaque annonce porte son texte C.A.S.S.I.E, ses sous-titres, son broadcast, son
hint, sa duree et son effet lumineux (couleur, zones, duree).

**Format modifie :** `zones` prend des valeurs `FacilityZone` et non plus
`ZoneType`. Sont acceptes `LightContainment`, `HeavyContainment`, `Entrance`,
`Surface`, `Other` et `None`. `Unspecified` devient `None`, qui vise toute
l'installation, et `Pocket` devient `Other`. Une ancienne config contenant
`Unspecified` ou `Pocket` ne se charge plus : la corriger ou la recreer.

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `out/plugins/TeamGenocide.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` n'est pas un plugin : il va dans `dependencies/`, jamais dans
`plugins/`. Le mettre a jour en meme temps que ce plugin.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `teamgenocide status` | `teamgenocide.manage` | Equipes couvertes et delai d'activation |

Alias `tg`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Installation

`dotnet build -c Release` a la racine du depot produit les DLL dans `out/`. Les
copier selon le tableau ci-dessus :

```
LabAPI/plugins/<port>/TeamGenocide.dll
LabAPI/dependencies/<port>/AugatonLib.dll
```

Si plusieurs plugins de la collection sont installes, garder la version
d'AugatonLib la plus recente : elle est partagee par tous.

Les fichiers de configuration sont crees au premier lancement :

```
LabAPI/configs/<port>/TeamGenocide/config.yml
```

Ce plugin n'a aucun texte a traduire, donc pas de `translation.yml`.

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la
cle `is_enabled` n'existe plus.

## Portage LabAPI

Comportement inchange, avec les memes declencheurs : `PlayerEvents.ChangingRole`
avec la raison `Died`, avant l'application du nouveau role. Le jeu l'emet dans
`ServerSetRole`, EXILED dans `InitializeNewRole`, soit au meme moment du flux.
C.A.S.S.I.E passe par `Announcer`, avec la meme charge utile : sans bruit de fond
ni parasites, sous-titres automatiques coupes si `subtitle` est vide.

`is_enabled` disparait. Seul le format de `zones` change, voir la configuration.

## Note de portage precedente

La version 2.x ciblait EXILED 8.11.0. Trois defauts corriges :

- **Desabonnement par finaliseur.** Les evenements etaient souscrits dans le
  constructeur et desouscrits dans `~PlayerEvents()`. Un finaliseur s'execute
  quand le ramasse-miettes le decide, parfois jamais : les handlers survivaient
  au `reload` et se declenchaient en double. `OnDisabled` se contentait de
  mettre la reference a `null`.
- **Lumieres jamais restaurees.** `ChangeLights` coupait les lumieres puis, apres
  la duree, les recoupait au lieu de restaurer la couleur d'origine. Les
  callbacks differes n'etaient jamais annules au changement de round.
- **Faux positif au respawn.** L'annonce se declenchait meme quand le nouveau
  role appartenait a la meme equipe.

Le comptage LINQ dans le handler `ChangingRole` est remplace par une boucle
explicite, et l'etat est remis a zero a chaque transition de round.
