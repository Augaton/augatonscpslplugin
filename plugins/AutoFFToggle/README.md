# AutoFFToggle 3.0


> Portage LabAPI 1.1.7 d'un plugin de **Kognity**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Coupe le tir allie pendant le round, le reactive a la fin.

**LabAPI 1.1.7** — `dotnet build -c Release plugins/AutoFFToggle/AutoFFToggle.csproj`

## Fonctionnement

Le plugin memorise l'etat du tir allie au chargement. Il le coupe a
`RoundStarted` et le reactive a `RoundEnded`, puis restaure l'etat memorise au
redemarrage du round et a la desactivation du plugin.

Il agit sur `Server.FriendlyFire` via `FriendlyFireArbiter`, l'**arbitre** partage
avec les autres plugins de la collection dans AugatonLib.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `disable_during_round` | `true` | Coupe le tir allie au debut du round |
| `enable_on_round_end` | `true` | Le reactive a la fin du round |
| `enable_delay` | `0` | Delai avant reactivation, en secondes |
| `restore_on_round_restart` | `true` | Restaure l'etat initial au restart et a la desactivation |
| `announce_on_enable` | `true` | Broadcast a la reactivation |
| `announce_duration` | `8` | Duree du broadcast |

Le texte du broadcast est dans le fichier de traduction.

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `out/plugins/AutoFFToggle.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` n'est pas un plugin : il va dans `dependencies/`, jamais dans
`plugins/`. Le mettre a jour en meme temps que ce plugin.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `autoff status` | `autofftoggle.manage` | Etat du tir allie et liste des demandeurs |
| `autoff set <on\|off\|auto>` | `autofftoggle.manage` | Force le tir allie jusqu'a la fin du round, ou rend la main au plugin |

Alias `ff`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Installation

`dotnet build -c Release` a la racine du depot produit les DLL dans `out/`. Les
copier selon le tableau ci-dessus :

```
LabAPI/plugins/<port>/AutoFFToggle.dll
LabAPI/dependencies/<port>/AugatonLib.dll
```

Si plusieurs plugins de la collection sont installes, garder la version
d'AugatonLib la plus recente : elle est partagee par tous.

Les fichiers de configuration sont crees au premier lancement :

```
LabAPI/configs/<port>/AutoFFToggle/config.yml
LabAPI/configs/<port>/AutoFFToggle/translation.yml
```

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la
cle `is_enabled` n'existe plus.

## Portage LabAPI

Comportement inchange. Les cles de configuration restent les memes, sauf
`is_enabled`, retiree. `ServerEvents.RoundRestarted` remplace
`RestartingRound`, et `Round.IsRoundInProgress` couvre l'activation en cours de
round.

## Note de portage precedente

La version 1.x ciblait EXILED 3.0.0-alpha.84. Elle plantait a la desactivation
lorsque le tir allie etait deja actif au demarrage : `OnEnabled` sortait sans
instancier le handler, et `OnDisabled` s'en desabonnait quand meme. Cette
version restaure proprement l'etat au lieu de s'auto-desactiver.
