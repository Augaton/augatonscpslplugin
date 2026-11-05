# ScpProximityChat 3.0


> Portage LabAPI 1.1.7 d'un plugin de **Bolton**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Permet aux SCP de parler dans le chat de proximite, entendus par les humains a portee.

**LabAPI 1.1.7** — `dotnet build -c Release plugins/ScpProximityChat/ScpProximityChat.csproj`

## Fonctionnement

Quand un SCP active le mode proximite, un `SpeakerToy` est attache a son
transform. Ses paquets voix du canal SCP sont decodes, amplifies, reencodes et
renvoyes aux joueurs a portee via ce haut-parleur.

## Activation

Deux modes via `activation_type` :

- `ServerSpecificSettings` (defaut) : une touche configurable est exposee dans les
  parametres serveur du client.
- `NoClip` : la touche noclip sert de bascule pour les roles concernes.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `scp_roles` | 049, 049-2, 096, 106, 173, 939 | Roles autorises. SCP-079 est retire automatiquement |
| `use_default_scp_chat` | `true` | Si `false`, les SCP n'entendent plus le locuteur hors portee |
| `volume` | `10` | Gain applique a la voix |
| `max_volume` | `25` | Garde-fou contre la saturation |
| `min_distance` / `max_distance` | `2` / `10` | Portee du haut-parleur |
| `toggle_cooldown_seconds` | `1` | Anti-spam sur la bascule |
| `disable_on_role_change` | `true` | Coupe la proximite a la mort ou au changement de role |

Les messages d'activation, de desactivation et de rappel de touche sont
configurables individuellement (type broadcast ou hint, duree, affichage).

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `out/plugins/ScpProximityChat.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` n'est pas un plugin : il va dans `dependencies/`, jamais dans
`plugins/`. Le mettre a jour en meme temps que ce plugin.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `scpproximity status` | `scpproximitychat.manage` | Mode d'activation, roles, volume et portee |

Alias `prox`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Installation

`dotnet build -c Release` a la racine du depot produit les DLL dans `out/`. Les
copier selon le tableau ci-dessus :

```
LabAPI/plugins/<port>/ScpProximityChat.dll
LabAPI/dependencies/<port>/AugatonLib.dll
```

Si plusieurs plugins de la collection sont installes, garder la version
d'AugatonLib la plus recente : elle est partagee par tous.

Les fichiers de configuration sont crees au premier lancement :

```
LabAPI/configs/<port>/ScpProximityChat/config.yml
```

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la
cle `is_enabled` n'existe plus.

## Portage LabAPI

Les cles de configuration restent les memes, sauf `is_enabled`, retiree.
Il n'y a pas de `translation.yml` : les messages du plugin vivent dans `config.yml`, avec leur
type, leur duree et leur affichage.

`PlayerEvents.SendingVoiceMessage` part au meme endroit que `VoiceChatting`
d'EXILED, dans `VoiceTransceiver.ServerReceiveMessage`, apres la validation du
canal et avant le relais. L'annuler coupe donc bien le chat SCP.

La touche est ajoutee a `ServerSpecificSettingsSync.DefinedSettings`, sans
ecraser les reglages d'un autre plugin, et retiree a la desactivation. Le jeu
envoie deja ces reglages a chaque joueur quand il s'authentifie ; le plugin les
renvoie a `PlayerEvents.Joined`, comme il le faisait a `Verified`.
