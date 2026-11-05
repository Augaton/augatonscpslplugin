# SCPReplacer 3.1


> Portage LabAPI 1.1.7 d'un plugin de **Jon M**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Permet aux joueurs de se porter volontaires pour remplacer un SCP parti en debut de round.

**LabAPI 1.1.7** — `dotnet build -c Release plugins/SCPReplacer/SCPReplacer.csproj`

## Commandes joueur

| Commande | Effet |
|---|---|
| `.volunteer` / `.v` | Liste les SCP actuellement remplacables |
| `.volunteer <numero>` | S'inscrit a la loterie pour ce SCP |
| `.human` / `.no` | Renonce a son role de SCP pour un role humain aleatoire |

Les deux commandes sont soumises a `command_cooldown_seconds`.

## Loterie

Le premier volontaire declenche une loterie de `lottery_period_seconds`. A
l'expiration, un gagnant est tire parmi les volontaires encore connectes et
encore eligibles. Si plus aucun volontaire n'est valide, le remplacement est
simplement annule.

Les volontaires sont stockes par `UserId`, jamais par reference joueur : une
deconnexion pendant la loterie ne laisse rien derriere elle. Les coroutines de
loterie sont tuees a `RoundEnded`, `RoundRestarted` et `Disable`.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `pause_during_gamemodes` | `true` | Coupe `.human`, `.volunteer` et la reprise des SCP pendant un mode de jeu |
| `quit_cutoff` | `60` | Fenetre pendant laquelle le depart d'un SCP declenche l'annonce |
| `replace_cutoff` | `90` | Fenetre pendant laquelle `.volunteer` reste utilisable |
| `required_health_percentage` | `95` | Vie minimale du SCP partant pour etre remplacable |
| `lottery_period_seconds` | `10` | Duree de la loterie |
| `command_cooldown_seconds` | `3` | Anti-spam par joueur |
| `allow_alive_volunteers` | `false` | Si `false`, seuls les spectateurs peuvent se porter volontaires |
| `allow_human_command` | `true` | Active `.human` |
| `max_replacements_per_round` | `0` | Plafond par round. `0` = illimite |
| `ignored_roles` | SCP-049-2 | Roles non remplacables |
| `human_command_roles` | ClassD 45 / Scientist 45 / Guard 10 | Tirage pondere de `.human` |
| `class_d_bonus_items` | Flashlight, Coin | Objets bonus si `.human` donne Classe-D |

Les textes joueur sont dans le fichier de traduction.

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection.

| Fichier | Destination |
|---|---|
| `out/plugins/SCPReplacer.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` n'est pas un plugin : il va dans `dependencies/`, jamais dans
`plugins/`. Le mettre a jour en meme temps que ce plugin.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `scpreplacer status` | `scpreplacer.manage` | SCP en attente de remplacement et fenetres actives |

Alias `scprep`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.

## Installation

`dotnet build -c Release` a la racine du depot produit les DLL dans `out/`. Les
copier selon le tableau ci-dessus :

```
LabAPI/plugins/<port>/SCPReplacer.dll
LabAPI/dependencies/<port>/AugatonLib.dll
```

Si plusieurs plugins de la collection sont installes, garder la version
d'AugatonLib la plus recente : elle est partagee par tous.

Les fichiers de configuration sont crees au premier lancement :

```
LabAPI/configs/<port>/SCPReplacer/config.yml
LabAPI/configs/<port>/SCPReplacer/translation.yml
```

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la
cle `is_enabled` n'existe plus.

## Portage LabAPI

Les cles de configuration restent les memes, sauf `is_enabled`, retiree.

**Fonctionnalite retiree : l'integration `Exiled.CustomRoles`.** EXILED n'etant
plus la, un SCP porteur d'un role personnalise est traite comme son role de base :
son numero vient de son role, et le gagnant de la loterie recoit ce role, sans
role personnalise.

Le depart d'un SCP passe par `PlayerDeparture.Departing`. L'arbitrage avec
Replacer ne change pas : proprietaire `SCPReplacer`, priorite 100, portee
`CanClaimDeparture`.

Le kill de `disconnect_drop` met la vie a zero avant `Departing`. Le plugin la
retient donc a `PlayerEvents.Hurting`, avant ce kill, pour que
`required_health_percentage` juge la vie reelle du SCP.

Le rappel de `.human` suit `CharacterClassManager.OnRoundStarted`, le signal que
suivait EXILED. `ServerEvents.RoundStarted` part avant l'attribution des roles :
aucun joueur n'y est encore SCP.
