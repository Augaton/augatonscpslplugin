# RemoteKeycard 5.0


> Portage LabAPI 1.1.7 d'un plugin de **Beryl**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Ouvre portes, generateurs, casiers SCP et panneau d'ogive sans avoir la carte en
main : il suffit de la porter dans son inventaire.

**LabAPI 1.1.7** — `dotnet build -c Release plugins/RemoteKeycard/RemoteKeycard.csproj`

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `amnesia_matters` | `true` | L'effet Amnesie bloque l'ouverture a distance |
| `affect_doors` | `true` | Agit sur les portes |
| `affect_generators` | `true` | Agit sur les generateurs |
| `affect_warhead_panel` | `true` | Agit sur le panneau de l'ogive |
| `affect_scp_lockers` | `true` | Agit sur les casiers SCP |
| `ignored_roles` | `[]` | Roles ne beneficiant jamais de l'ouverture a distance |
| `require_held_keycard` | `false` | Exige la carte en main plutot que dans l'inventaire |
| `extras.enable_events` | `false` | Expose l'evenement `UsingKeycard` aux autres plugins |

## API

Les plugins tiers peuvent intercepter chaque ouverture a distance :

```csharp
RemoteKeycard.API.Events.UsingKeycard += ev =>
{
    if (ev.Target == RemoteTarget.WarheadPanel)
        ev.IsAllowed = false;
};
```

`ev.Target` indique le type d'objet (`Door`, `Generator`, `WarheadPanel`,
`ScpLocker`), `ev.OriginalEvent` porte l'`EventArgs` LabAPI d'origine
(`PlayerInteractingDoorEventArgs`, `PlayerUnlockingGeneratorEventArgs`,
`PlayerUnlockingWarheadButtonEventArgs` ou `PlayerInteractingLockerEventArgs`),
et `ev.Player` est un `LabApi.Features.Wrappers.Player`. Le systeme est
desactive par defaut : l'activer coute une allocation par
interaction, inutile si aucun plugin ne l'ecoute.

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `out/plugins/RemoteKeycard.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` n'est pas un plugin : il va dans `dependencies/`, jamais dans
`plugins/`. Le mettre a jour en meme temps que ce plugin.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `remotekeycard status` | `remotekeycard.manage` | Cibles actives et garde-fous |

Alias `rk`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Installation

`dotnet build -c Release` a la racine du depot produit les DLL dans `out/`. Les
copier selon le tableau ci-dessus :

```
LabAPI/plugins/<port>/RemoteKeycard.dll
LabAPI/dependencies/<port>/AugatonLib.dll
```

Si plusieurs plugins de la collection sont installes, garder la version
d'AugatonLib la plus recente : elle est partagee par tous.

Les fichiers de configuration sont crees au premier lancement :

```
LabAPI/configs/<port>/RemoteKeycard/config.yml
```

Ce plugin n'a aucun texte a traduire, donc pas de `translation.yml`.

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la
cle `is_enabled` n'existe plus.

## Portage LabAPI

Les cles de configuration restent les memes, sauf `is_enabled`, retiree. Une
seule difference visible : un refus sur le panneau d'ogive joue de nouveau
l'animation de refus du jeu, qu'EXILED supprimait.

LabAPI separe deux notions qu'EXILED confondait dans `IsAllowed` : `CanOpen`
porte le resultat du controle de carte, `IsAllowed` annule toute l'interaction.
Le plugin ecrit donc `CanOpen`, sauf sur le panneau d'ogive, ou `IsAllowed` joue
les deux roles. Une interaction deja annulee par un autre plugin reste annulee.

Le controle `IsLocked` est indispensable : sur une porte verrouillee, le jeu
emet aussi `InteractingDoor`, et `CanOpen` y leverait le verrou.

Les plugins tiers qui ecoutent `UsingKeycard` doivent etre recompiles, car
`ev.Player` et `ev.OriginalEvent` ont change de type.

## Note de portage precedente

La version 3.x ciblait EXILED 4.1.0. Le portage touche surtout au systeme de
permissions : le rework des cartes de la 14.0 a remplace `KeycardPermissions`
par `DoorPermissionFlags`. Le type est aliase en un seul point par fichier
(`using PermissionFlags = ...`) pour qu'un changement d'API ne se propage pas.

Les exceptions ne sont plus masquees derriere une option de config : elles
partent systematiquement en `Log.Error`.
