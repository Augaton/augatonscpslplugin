# RandomGamemode 4.1

> Portage LabAPI d'un plugin de **OPGman**. Depot non affilie a l'auteur
> d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Modes de jeu speciaux tires au hasard au debut d'un round.

## Les 19 modes

**Portes depuis la version 2.x**

| Cle | Nom | Principe |
|---|---|---|
| `dodgeball` | Balle au prisonnier | Sept SCP-018 chacun, tir allie actif |
| `peanutraid` | Invasion de cacahuetes | Les SCP deviennent Classe-D miniatures, les humains deviennent SCP-173 |
| `bluescreen` | Ecran bleu de la mort | Scientifiques contre un SCP-079 niveau 3, quinze minutes |
| `livingnerd` | La nuit du scientifique fou | Un scientifique arme traque les Classe-D dans le noir |
| `randomizer` | Randomiseur | Roles, objets et positions au hasard, dernier vivant gagne |
| `mimicry` | Mimetisme agacant | Jailbird contre SCP-939, mourir vous transforme |
| `lockedin` | Confinement total | Portails scelles, decontamination desactivee |
| `infection` | Infection | Chaque mort rejoint SCP-049-2 |
| `larry` | La vie selon Larry | SCP-106 traque les Classe-D en zone legere |

**Nouveaux**

| Cle | Nom | Principe |
|---|---|---|
| `customrolechaos` | Anarchie des roles | Un role UCR personnalise au hasard pour chacun |
| `customteamwar` | Guerre des factions | Deux equipes UCT larguees a vingt secondes d'ecart |
| `customitemrush` | Arsenal experimental | Inventaire rempli d'objets UCI personnalises |
| `pillparty` | Soiree pilules | Pilules SCP-500, vanilla ou personnalisees |
| `coinroyale` | Pile ou face royale | Une poignee de pieces chacun |
| `juggernaut` | Juggernaut | Un joueur surpuissant contre tous les autres |
| `blackout` | Panne generale | Obscurite permanente, lampes distribuees |
| `gungame` | Gun Game | Chaque elimination ameliore votre arme |
| `hideandseek` | Cache-cache | Les Classe-D se cachent, puis un chasseur les traque jusqu'a la fin du temps |
| `hotpotato` | Patate chaude | Chacun son arme, et une balle passe la bombe a la cible. Quand elle explose, son porteur meurt. |
| `traitor` | Un traitre parmi nous | Un scientifique est un tueur secret. Tuer un innocent coute son inventaire au tireur. |
| `evacuation` | Evacuation generale | L'ogive part au bout d'une minute, verrouillee : il faut rejoindre la surface |

`pillparty` et `coinroyale` s'appuient sur SCP500s et BetterCoinflips s'ils sont
installes, mais fonctionnent aussi en vanilla.

## Integration UCI / UCR / UCT

L'integration passe par **AugatonLib**, qui atteint les trois plugins
Uncomplicated par reflexion sur des signatures **verifiees contre les DLL
publiees** :

| Cible | Catalogue | Application |
|---|---|---|
| Roles | `CustomRole.List` | `SummonedCustomRole.Summon(LabApi.Player, ICustomRole)` |
| Objets | `CustomItem.List` | `new SummonedCustomItem(ICustomItem, LabApi.Player)` |
| Equipes | `Team.List` | `TeamSpawner.SpawnSpecificTeam(Team, bool, bool)` |

Le catalogue est lu directement dans les assemblys chargees, donc il reflete ce
que les plugins ont reellement enregistre, y compris les roles refuses au
chargement.

Le plugin fonctionne sans eux : les trois modes concernes se retirent
automatiquement du tirage si l'integration n'est pas disponible. L'etat exact
est journalise au demarrage et visible via `gamemode list`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `gamemode list` | `randomgamemode.manage` | Modes, poids, prerequis et etat de l'integration |
| `gamemode force <cle\|none>` | `randomgamemode.manage` | Force un mode au prochain round |
| `gamemode start <cle>` | `randomgamemode.manage` | Demarre un mode immediatement |
| `gamemode stop` | `randomgamemode.manage` | Arrete le mode en cours |

Alias `gm`. Chaque appel est trace avec l'auteur.

## Configuration

Chaque mode a son entree dans `gamemodes` : `is_enabled`, `weight`,
`min_players`, `max_players`, `description`, `pause_respawn_waves`,
`force_friendly_fire`, `round_time_limit`, `suppress_genocide_announcements`. Une
entree absente est recreee avec les valeurs par defaut.

Le tirage est **pondere** : un mode a `weight: 20` sort deux fois plus souvent
qu'un mode a `weight: 10`. `weight: 0` le retire du tirage sans empecher de le
forcer a la main.

Les fichiers sont `LabAPI/configs/<port>/RandomGamemode/config.yml` et
`translation.yml`. Le format des valeurs ne change pas. `is_enabled`, au niveau du
plugin, n'existe plus : l'activation se regle dans `properties.yml`, cree par
LabAPI. `no_permission` et `plugin_unavailable` sont retires de la traduction : le
refus de permission vient du socle de commandes d'AugatonLib.

`labapi reload configs` s'applique tout de suite : chaque mode relit ses reglages a
chaque usage, y compris ceux d'un mode en cours.

## Dependances

Voir le [README racine](../../README.md) pour la compilation et l'installation.

| Fichier | Destination |
|---|---|
| `RandomGamemode.dll` | `LabAPI/plugins/<port>/` |
| `AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| HintServiceMeow | `LabAPI/plugins/<port>/` |

## Note de version 4.0

Portage sur LabAPI 1.1.7. Le but est le meme comportement en jeu. Plusieurs points
de declenchement different toutefois de ce que leur nom laisse penser :

- **Debut de mode.** Le tirage ecoute `CharacterClassManager.OnRoundStarted`, car
  `ServerEvents.RoundStarted` part avant l'attribution des roles.
- **Fin de round du Randomiseur.** Le mode pose `RoundEndingConditionsCheck.CanEnd`.
  Annuler `RoundEnding` laisserait `RoundSummary.IsRoundEnded` a vrai.
- **Reapparition.** Les modes ecoutent `ChangedRole`, car `Spawned` ignore SCP-079 et
  part avant la remise de l'inventaire.
- **Portails scelles.** Le refus passe par `CanOpen` pour garder le retour "acces
  refuse" de la porte.
- **Balle au prisonnier.** LabAPI previent apres le spawn de la balle : elle est
  respawnee pour que les clients recoivent son echelle. La meche d'une seconde
  s'ecrit par reflexion dans `TimeGrenade._fuseTime`, car le premier rebond la
  relance depuis ce champ prive.

Rien ne survit a un mode. L'arret libere le tir allie, la suppression des annonces
d'extinction, les lumieres, les tailles, les portes et les vagues de respawn. Il
retire aussi les bonus de vie et les effets poses par le mode.

## Note de portage

La version 2.7.3 ciblait EXILED 9.12.0. Defauts corriges :

- **Poids fausses au reload.** `EnabledList` etait une liste statique remplie
  par `Add` dans `OnEnabled` sans jamais etre videe. Chaque `reload` doublait
  toutes les entrees.
- **Coroutines jamais arretees.** `Timing.RunCoroutine` etait appele sans
  conserver le handle. Un redemarrage de round en plein mode laissait la
  coroutine tourner sur le round suivant.
- **Tir allie mal restaure.** L'etat d'origine etait capture mais la
  restauration forcait `false` au lieu de restaurer la valeur capturee.
- **Liste vide.** `EnabledList.RandomItem()` levait une exception si tous les
  modes etaient desactives.
- **Cast non valide.** `forcegamemode` castait un entier arbitraire vers l'enum
  sans verifier qu'il correspondait a un mode existant.
- **Etat non reinitialise** a `RestartingRound`, seulement a `RoundEnded`.
- **Objets impossibles.** Le Randomiseur tirait dans l'integralite de l'enum
  `ItemType`, y compris `None` et les entrees de debug. Le pool est desormais
  explicite en configuration.

L'architecture change : au lieu d'un `switch` geant et de handlers qui testaient
tous `if (CurrentGamemode == X)`, chaque mode est une classe autonome et un
registre unique repartit les evenements vers le seul mode actif.

## Tir allie et AutoFFToggle

`force_friendly_fire` ne touche plus directement `Server.FriendlyFire`. Les deux
plugins passent par un **arbitre** a demandes nommees,
`AugatonLib.FriendlyFire.FriendlyFireArbiter`. Les deux peuvent intervenir sur le
meme round sans s'ecraser.

## Vote du mode

Avec `enable_vote`, `gamemode_chance` est tire dans le lobby. Si un mode doit avoir lieu, `vote_options`
modes sont proposes et les joueurs votent dans la console `~` : `.votemode` liste les choix,
`.votemode 2` vote, et `.vm` est un alias. Le mode le plus vote demarre, avec un tirage en cas
d'egalite. Sans vote, ou si aucun choix ne peut demarrer, le tirage pondere habituel s'applique.
`gamemode force` l'emporte toujours.

## Modes et plugins presents

Pile ou face royale n'est tire que si BetterCoinflips est installe. Soiree pilules distribue les
pilules de SCP500s s'il est present, puis les objets d'UncomplicatedCustomItems, et des SCP-500
normales sinon. `pill_party_custom_chance` fixe la part de pilules speciales.
