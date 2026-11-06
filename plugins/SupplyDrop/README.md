# SupplyDrop 5.0

> Portage LabAPI d'un plugin de **Wafel, KadeDev, JesusQC**. Depot non
> affilie aux auteurs d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Largages de ravitaillement periodiques pour le MTF et l'Insurrection du Chaos.

## Profils

Les largages sont **pilotes par les donnees**. Chaque entree de `profiles` decrit
un type de largage complet : contenu, horaires, positions, annonce, balise.
Ajouter une entree avec une cle inutilisee cree un nouveau type de largage sans
recompiler.

Deux profils par defaut : `mtf` et `chaos`. Les deux embarquent **deux pieces**
depuis la 4.3 : BetterCoinflipsRewritten detruit la piece a l'usage, les
largages sont donc une source de reappro.

| Cle | Role |
|---|---|
| `first_drop_seconds` | Delai avant le premier largage, depuis le debut du round |
| `interval_seconds` | Delai entre deux largages du meme profil |
| `announcement_lead_seconds` | Temps entre l'annonce et l'apparition des objets |
| `max_drops_per_round` | Plafond par round. `-1` = illimite |
| `fallback_role` | Role dont le point d'apparition sert d'ancre au largage |
| `ammo/armor/item/weapon_position` | Coordonnees fixes par categorie. `0,0,0` = ancre du role de repli |
| `scatter_radius` | Dispersion horizontale autour du point, en metres |
| `enable_beacon` | Halo lumineux sur la zone de largage |
| `cassie_announcement` | Annonce C.A.S.S.I.E. Vide = desactivee |

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `supplydrop list` | `supplydrop.call` | Liste les profils, leurs horaires et leur etat |
| `supplydrop call <profil>` | `supplydrop.call` | Declenche un largage immediatement, avec annonce |
| `supplydrop call <profil> silencieux` | `supplydrop.call` | Largage sans annonce ni C.A.S.S.I.E |
| `supplydrop call <profil> ici` | `supplydrop.call` | Largage aux pieds de l'appelant, pour verifier le contenu |

Alias `sd`. Les modificateurs se cumulent : `sd call mtf silencieux ici`. Chaque
appel est trace avec l'auteur et la position forcee eventuelle.

## Position du largage

Le largage n'utilise **aucune coordonnee en dur**. L'ancre est le point
d'apparition reel du role indique par `fallback_role` : le MTF se pose la ou
arrive une vague MTF, l'Insurrection la ou arrive son fourgon. Ces points sont
lus dans la scene chargee, ils suivent donc les remaniements de carte de
Northwood sans intervention.

Ordre de resolution de l'ancre :

1. point d'apparition de `fallback_role`, lu par `AugatonLib.Spawning.RoleSpawn`
2. porte `SURFACE_NUKE` (`DoorName.SurfaceNuke`), 1,5 m au-dessus et 3 m devant
3. salle de surface (`RoomName.Outside`)

Si les trois echouent, le largage est **annule avec un `Logger.Error`** plutot que
depose en `0,0,0`, c'est-a-dire dans le vide sous la carte.

Renseigner `ammo/armor/item/weapon_position` force des coordonnees fixes pour la
categorie concernee et court-circuite l'ancre. A n'utiliser que pour un point
precis verifie en jeu : une coordonnee obsolete envoie le contenu hors de la
carte sans aucun message d'erreur, puisque la valeur est syntaxiquement valide.

`spawn_height_offset` (config globale, 0,5 m par defaut) souleve les objets pour
qu'ils ne traversent pas le sol au moment de l'apparition.

## Configuration

Un largage sur sept environ est piege par defaut : une grenade, explosive ou aveuglante, part quelques secondes apres qu'un humain s'en approche.

| Cle | Defaut | Role |
|---|---|---|
| `min_event_gap_seconds` | `60` | Ecart minimal avec un incident de SiteIncidents. Le largage retente apres `retry_seconds` |
| `trap_chance_percent` | `15` | Chance qu'un largage soit piege. `0` = jamais |
| `trap_chance_per_chaos_level` | `5` | Chance ajoutee par niveau du Directeur du chaos |
| `trap_range` | `4` | Distance en metres qui declenche le piege |
| `trap_fuse_seconds` | `2` | Delai avant l'explosion |
| `trap_flash_seconds` | `4` | Duree de l'aveuglement |
| `trap_lifetime_seconds` | `180` | Duree de vie d'un piege intact |

Les fichiers sont `LabAPI/configs/<port>/SupplyDrop/config.yml` et `translation.yml`.
Le format des valeurs ne change pas, coordonnees `x` / `y` / `z` comprises.

- `is_enabled`, au niveau du plugin, n'existe plus : l'activation se regle dans
  `properties.yml`, cree par LabAPI. Le `is_enabled` de chaque profil reste.
- `custom_items` est retire de tous les profils.
- `no_permission` et `plugin_unavailable` sont retires de la traduction : le refus
  de permission vient du socle de commandes d'AugatonLib.

## Dependances

Voir le [README racine](../../README.md) pour la compilation et l'installation.

| Fichier | Destination |
|---|---|
| `SupplyDrop.dll` | `LabAPI/plugins/<port>/` |
| `AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| HintServiceMeow | `LabAPI/plugins/<port>/` |

## Note de version 5.0

- Portage sur LabAPI 1.1.7. Le comportement des largages ne change pas.
- **Retrait des objets personnalises.** Le pont `CustomItemBridge` reposait sur
  `Exiled.CustomItems`, sans equivalent LabAPI. La cle `custom_items`, ses types et
  le diagnostic de `supplydrop list` disparaissent : les pilules de SCP500s ne sont
  plus larguees.
- Une arme larguee part chargee, avec ses accessoires par defaut. LabAPI cree un
  pickup au chargeur vide : `FirearmPickup.OnDistributed` le remplit, apres
  l'enregistrement d'un code d'accessoires pour eviter le tirage au hasard.
- `labapi reload configs` remplace l'objet `Config` : le service relit `plugin.Config`
  a chaque usage, et un largage annonce relit son profil par sa cle.

## Note de version 4.3

- Le tirage des objets personnalises devient aleatoire par defaut : plus de
  liste predefinie, le registre d'Exiled.CustomItems est interroge a chaque
  largage et filtre par `base_items`.
- `source` permet de revenir a une liste explicite (`Pool`) ou de melanger les
  deux (`Both`).
- `excluded` ecarte une pilule precise sans quitter le mode aleatoire.

## Note de version 4.2

- Compatibilite optionnelle avec les `CustomItem` d'EXILED, donc avec les
  SCP-500 modifies de SCP500s. Tirage pondere, sans doublon par defaut.
- `supplydrop list` diagnostique le pool d'objets personnalises.
- Un profil peut n'avoir que des objets personnalises, sans `items`.

## Note de version 4.1

- Les coordonnees en dur des deux profils par defaut, heritees de la 3.x, sont
  supprimees. Elles designaient des points de la surface d'une version anterieure
  du jeu et prenaient le pas sur le point de repli, qui n'etait donc jamais
  utilise : tout le contenu partait hors de la carte, balise comprise, sans la
  moindre erreur en console.
- Un largage sans position exploitable echoue desormais bruyamment.
- `spawn_height_offset` evite le clipping a travers le sol.
- `supplydrop call <profil> ici` permet de verifier un profil sans se deplacer.
- L'helicoptere du profil `mtf` est nomme « helicoptere MTF ».

## Note de portage

La version 3.x ciblait EXILED 3.0.0-alpha.83. La reecriture corrige plusieurs
defauts de comportement du plugin d'origine :

- **Priorite des operateurs.** Le test de position s'ecrivait
  `IsKeycard() || IsMedical() || IsUtility() || IsScp() && position != zero`.
  `&&` liant plus fort que `||`, la verification de coordonnee ne s'appliquait
  qu'aux objets SCP : les cartes, soins et utilitaires etaient envoyes a
  `0,0,0` quand la position n'etait pas configuree. Meme defaut sur les armes.
- **Fuite de position entre objets.** La variable de position etait declaree
  hors de la boucle : un objet ne correspondant a aucune categorie heritait de
  la position de l'objet precedent au lieu du point de repli.
- **Tirage decale.** `Random.Next(100) <= Chance` donnait 1 % de chance a un
  objet configure a `Chance = 0`.
- **Objet reutilise.** Une seule instance d'objet etait creee puis « spawnee »
  autant de fois que la quantite demandee, au lieu d'un pickup par exemplaire.
- **Verification des joueurs au mauvais moment.** Le nombre de joueurs etait
  teste *avant* l'attente de dix minutes, pas au moment du largage.
- **Intervalle du vehicule fausse.** `TimeDifference` etait attendu a chaque
  iteration de la boucle et non une seule fois, donc l'intervalle reel valait
  `TimeDifference + CarTime + 30` au lieu de `CarTime`.
- **Alias de config.** `using Config = Exiled.Loader.Config;` masquait la classe
  de configuration du plugin dans `Plugin.cs`.
- **Nettoyage incomplet.** Les coroutines n'etaient arretees qu'a
  `WaitingForPlayers`, pas a `RoundEnded` ni `RestartingRound`.

`RespawnEffectsController`, qui jouait le son d'arrivee, a disparu avec la
refonte des vagues de respawn de la 14.0. Il est remplace par une annonce
C.A.S.S.I.E configurable par profil.

Les deux coroutines independantes sont remplacees par **un seul ordonnanceur**
qui pilote tous les profils, conformement aux conventions du projet.

## Pilules et declencheurs

`pill_draws` et `pill_chance` ajoutent des pilules SCP500s a un largage. Sans SCP500s, ces cles sont
sans effet. Elles remplacent `custom_items`, qui dependait d'EXILED.

Un profil peut aussi partir sur un evenement de la partie, en plus de son minuteur :
`generators_trigger` (nombre de generateurs actives), `decontamination_trigger` et `warhead_trigger`.
Ces largages comptent dans `max_drops_per_round`. Par defaut, seul le profil `mtf` en utilise un :
il part quand les 3 generateurs sont actifs.

`pause_during_gamemodes` suspend les largages automatiques pendant un mode de type arene de RandomGamemode. Les appels du staff restent possibles. Sans RandomGamemode, ce reglage n'a aucun effet.
