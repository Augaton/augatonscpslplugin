# SCP500s 7.0


> Portage LabAPI d'un plugin de **VividZap, Lumi**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

44 pilules SCP-500 personnalisees. LabAPI n'ayant pas d'objets personnalises, le
plugin suit lui-meme les numeros de serie, qui survivent au passage objet / pickup.

## Ce qui change en 7.0

Portage de EXILED vers LabAPI, qui vise le meme comportement. La config est a recreer
dans `LabAPI/configs/<port>/SCP500s/` : le format des effets change (voir
[Format des effets](#format-des-effets)).

## Ce qui change en 6.0

Le catalogue ne contient plus **aucune pilule purement punitive**. Ramasser une
SCP-500 identifiee n'est jamais une sanction : au pire elle est absurde, jamais
une perte seche.

Le hasard est concentre dans une seule pilule, `chaos` (**SCP500-Roulette**),
qui represente **40 % des SCP-500 placees sur la carte**
(`random_pill_share_percent`). A la consommation elle tire une categorie
— **bon**, **mauvais** ou **neutre** — puis une issue au poids dans cette
categorie. Tout ce qui etait autrefois une pilule-piege (poison, cecite,
hemorragie, dimension de poche, projection en l'air) vit desormais dans sa
categorie `Bad`.

Six pilules ont un effet **actif jusqu'a la mort** : `cola` (SCP-207),
`anticola` (anti-SCP-207), `lumien` (passage dans le camp adverse), `xerneas`
(teleportation), `disguise` (apparence de SCP) et `wave` (vague de renfort
forcee).

> **Mise a jour depuis la 5.x :** la config existante est conservee.
> Les nouvelles pilules et la roulette n'apparaissent qu'apres suppression des
> sections `pills` et `chaos_outcomes` du YAML, qui se regenerent au demarrage
> suivant.

## Spawn : ce plugin possede le remplacement du loot

En mode `LootReplacement` (defaut), SCP500s remplace le loot de la carte
quelques secondes apres le debut du round :

- `replaceable_source_items` liste les objets remplacables (anti-douleurs,
  medkits, lampes, radios, cartes faibles)
- `replacement_chance_percent` decide quelle proportion est effectivement
  remplacee (60 % par defaut)
- de ces remplacements, `coin_share_percent` donne une **piece** (10 %) et le
  reste une **pilule** (90 %)
- de ces pilules, `random_pill_share_percent` donne la **roulette** (40 %) et
  le reste une pilule du catalogue, tiree par poids (60 %)
- `max_replacements_per_round` plafonne le total

La roulette ignore les poids de spawn et le plafond par round : sa part est
fixe. Les autres pilules gardent `spawn_weight` et `max_per_round`.

> **A faire une fois :** laisser `spawn_coins_on_map: false` dans la config de
> BetterCoinflips. Deux plugins qui se disputent les memes pickups ne peuvent
> pas garantir un ratio. Ici, un seul arbitre decide, donc le 10/90 est reel.

Les autres modes : `SpawnPoints` rend la main aux emplacements fixes declares
par pilule, `Both` combine les deux.

### Presence continue

En plus du remplacement de depart, `continuous_spawn` fait reapparaitre des
pilules pendant **tout le round**, un peu partout sur la carte. Au lancement,
le plugin releve les emplacements de tout le loot de la facility ; ensuite, a
chaque verification, il complete jusqu'a `continuous_spawn_target` pilules au
sol, en reprenant le meme tirage que le loot (part de roulette, poids, plafonds
par round).

| Cle | Defaut | Role |
|---|---|---|
| `continuous_spawn` | `true` | Active la presence continue |
| `continuous_spawn_interval_seconds` | `60` | Intervalle entre deux verifications (30 minimum) |
| `continuous_spawn_target` | `12` | Nombre de pilules maintenues au sol |
| `continuous_spawn_batch` | `3` | Pilules ajoutees au plus par verification |
| `continuous_spawn_player_distance` | `15` | Jamais sous les yeux d'un joueur vivant |
| `continuous_spawn_spacing` | `10` | Ecart minimal entre deux pilules au sol |
| `chaos_escalation` | `true` | Fait monter le chaos par paliers au fil du round |
| `chaos_step_seconds` | `120` | Duree d'un palier |
| `chaos_max_level` | `10` | Nombre maximal de paliers |
| `chaos_target_per_level` | `3` | Pilules maintenues en plus par palier |
| `chaos_batch_per_level` | `1` | Pilules ajoutees en plus par verification et par palier |
| `chaos_interval_per_level` | `3` | Secondes retirees a l'intervalle par palier, sans passer sous 30 |
| `chaos_random_share_per_level` | `4` | Points de part de roulette en plus par palier |
| `renewal_per_consumed_pill` | `1` | Pilules rendues a la carte par pilule consommee |
| `renewal_bonus_chance` | `25` | Chance en pourcent d'en rendre une de plus |
| `renewal_delay_seconds` | `20` | Delai avant leur apparition |
| `max_pills_on_ground` | `60` | Plafond absolu, pour proteger le serveur |

Avec les valeurs par defaut, le round demarre avec 12 pilules maintenues au sol et en vise
42 au bout de 20 minutes, dont la part de roulettes passe de 40 % a 80 %.

Chaque pilule consommee revient aussi ailleurs sur la carte, et parfois en double : plus les
joueurs en prennent, plus il y en a. Ce renouvellement s'ajoute a l'apparition continue.

Une pilule n'apparait jamais en LCZ une fois la zone decontaminee, ni sous
terre apres la detonation de l'ogive. Quand les plafonds par round du catalogue
sont atteints, les apparitions se replient sur la roulette.

## Catalogue

Chaque pilule reste un **SCP-500** : avant son effet propre, elle soigne 100 PV
et guerit ce qu'un SCP-500 guerit (saignement, poison, brulure, cecite, surdite,
arret cardiaque, SCP-207, Anti-SCP-207, SCP-1853). Une pilule remplace donc
proprement l'effet de SCP-207 ou d'Anti-SCP-207 d'une pilule precedente, sans
l'explosion que provoque le melange des vraies boissons.

Les 60 pilules vivent dans `pills`, dans la config. Chaque entree porte son
identifiant, sa cle, son nom, sa couleur de halo, son poids de tirage, son
plafond par round, son message, ses effets et son `behaviour`.

Tout est **pilote par les donnees** : effets, objets donnes, soin, points de
vie artificiels (`artificial_health`), regeneration (`regeneration_per_second`
pendant `regeneration_seconds`), vie maximale, degats, lachage d'inventaire.
Un objet donne alors que l'inventaire est plein tombe aux pieds du joueur, et
une arme donnee arrive avec ses reserves de munitions pleines.
`cure_negative_effects` ne retire que les effets negatifs. Ajouter une entree avec un identifiant libre suffit a creer une
nouvelle pilule, sans recompiler.

Les pilules livrees par une nouvelle version s'ajoutent **toutes seules** a une
config existante : au chargement, chaque pilule par defaut absente de `pills`
(ni son identifiant ni sa cle) y est ajoutee, sans toucher aux reglages deja
presents. Meme chose pour les issues de la roulette.

Les pilules deja presentes sont aussi **mises a niveau** quand un reglage par
defaut est corrige : un effet, une description ou un message qui vaut encore
exactement l'ancienne valeur par defaut prend la nouvelle. Une valeur modifiee
a la main n'est jamais touchee. Pour retirer une pilule,
passer `is_enabled: false` ; la supprimer de la liste ne suffit pas, elle
reviendrait au chargement suivant.

### Format des effets

`effects` liste des entrees `type`, `intensity`, `duration`. `type` est le **nom
de la classe** de l'effet dans le jeu, pas une valeur d'enum :

```yaml
effects:
- type: Scp207
  intensity: 2
  duration: 0
- type: Blindness
  intensity: 1
  duration: 5
```

La casse est ignoree et le nom doit etre exact. Trois noms different de l'ancien
format : `Blinded` devient `Blindness`, `SinkHole` devient `Sinkhole`, et
`Marshmallow` devient `MarshmallowEffect`. Un nom inconnu est signale par un
`Warn` au chargement et a chaque `labapi reload configs`, puis ignore : le reste de
la pilule fonctionne.

### Effets negatifs : roulette uniquement

`random_only_pills` liste les pilules qui n'apparaissent **jamais** sur la
carte. Leurs effets, negatifs ou risques, ne sortent que via la roulette, qui
les contient deja. Le staff peut toujours les donner avec `scp500 give`, et
`scp500 list` les marque `roulette uniquement`.

Par defaut : `lumien` (trahison), `xerneas` (teleportation), `swap`, `summon`,
`wave`, `blackout`, `tantrum`, `contagion`, `disguise`, `prism` et `fog`. Toutes
les pilules posees sur la carte sont donc positives ou neutres. `cola` et
`anticola` en font partie : SCP-207 accelere et donne une endurance infinie
contre un peu de vie, Anti-SCP-207 regenere et protege, et le prochain SCP-500
met fin a l'un comme a l'autre.

### Reglages des effets

Les intensites suivent le fonctionnement reel des effets du jeu :

| Pilule | Effet |
|---|---|
| `shadow` | Traverse les portes et silhouette d'ombre, 10 s |
| `sonic` | +30 % de vitesse et endurance infinie, 10 s |
| `super` | Invisible, deplacements silencieux, plus rapide, 7 s |
| `vitality` | Immunite aux effets negatifs, 30 s |
| `adrenaline` | -25 % de degats, +15 % de vitesse, endurance infinie, 15 s |
| `surgeon` | SCP-1853 : soins, rechargement et maniement acceleres, 45 s |
| `juggernaut` | Vie maximale 200, -15 % aux tirs dans le corps et -20 % a tous les degats, 60 s |
| `nightvision` | Vision nocturne nette, 60 s |
| `feather` | +60 % de hauteur de saut, +10 % de vitesse, 30 s |
| `regeneration` | +3 PV par seconde pendant 20 s |
| `clairvoyance` | Plus de brouillard a vos yeux : vous voyez bien plus loin, 60 s |
| `cola` / `anticola` | Une dose de SCP-207 / Anti-SCP-207, jusqu'au prochain SCP-500 |

### Pilules utilitaires

| Cle | Effet |
|---|---|
| `bouclier` | 60 points de vie artificiels |
| `munitions` | Reserves de munitions au maximum pour les armes portees, ou de tous calibres sans arme |
| `recensement` | Nombre de survivants par camp |
| `radar` | SCP en vie, du plus proche au plus eloigne, avec leur distance |
| `promotion` | Remplace la meilleure carte d'acces par la suivante de `keycard_upgrade_path` |
| `infirmier` | Medkit et anti-douleurs |
| `sourdine` | Pas silencieux pendant une minute |
| `camouflage` | Silhouette translucide pendant trente secondes |
| `vaccin` | Guerit et protege des effets negatifs pendant une minute |
| `ration` | Petit soin et 20 points de vie artificiels |
| `marathon` | Endurance infinie et vitesse accrue pendant une minute |
| `pieces`, `bonbons`, `radio`, `lampe` | Deux pieces, un sachet SCP-330, une radio, une lampe torche |

La roulette gagne les issues correspondantes en `Good` (`carapace`,
`ravitaillement`, `radar`, `recensement`, `promotion`, `sourdine`,
`camouflage`, `marathon`) et en `Neutral` (`tirelire`, `friandise`, `talkie`).

Une duree d'effet de `0` rend l'effet **actif jusqu'a la mort**. Toute autre
valeur est bornee par `max_effect_duration`.

Ce qu'aucune donnee ne decrit passe par `behaviour`, un comportement code
choisi dans une liste fermee :

| `behaviour` | Effet |
|---|---|
| `None` | Rien de plus que les donnees |
| `Shrink` / `Enlarge` | Retrecit ou agrandit le joueur jusqu'a sa reapparition |
| `RandomTeleport` | Teleporte dans une salle au hasard, hors LCZ une fois la zone decontaminee |
| `Betrayal` | Bascule dans le camp adverse au grade equivalent |
| `Disguise` | Apparence d'un SCP au hasard, jusqu'a la mort. SCP-079 est exclu de la liste : il n'a pas de modele, l'apparence reviendrait a une invisibilite permanente |
| `SkyLaunch` | Projette en l'air puis fait exploser |
| `Swap` / `Summon` | Echange de position, ou attire une cible. Personne n'est envoye dans une LCZ decontaminee : l'effet est alors annule |
| `ForceWave` | Declenche une vague de renfort, camp tire au sort |
| `RandomEffect` | Un effet du jeu tire au hasard |
| `Blackout` | Coupe les lumieres de la zone du joueur |
| `DiscoLights` | Repeint les lumieres de la salle, puis les restaure |
| `Mice` | Fait apparaitre des souris dans l'abri |
| `Tantrum` | Laisse une flaque de SCP-173 |
| `Fireworks` | Explosion purement decorative |
| `Contagion` | Applique les effets de la pilule a tout le monde autour |
| `CassiePrank` | Joue une annonce tiree de `cassie_lines` |
| `AmmoRefill` | Remplit les reserves de munitions |
| `Census` | Affiche le nombre de survivants de chaque camp |
| `ScpRadar` | Liste les SCP en vie et leur distance (`radar_max_entries`) |
| `KeycardUpgrade` | Remplace la meilleure carte d'acces par la suivante de `keycard_upgrade_path` |

Mettre `is_enabled: false` sur une entree la desinscrit completement : ni
enregistrement, ni spawn.

### Pilules evenementielles

Cinq pilules reposent sur des effets que Northwood n'active que pendant les
evenements — `flamingo`, `marshmallow`, `metal`, `snow`, `sugar`. Elles sont
livrees avec `is_enabled: false` parce qu'en dehors d'Halloween, de Noel et du
premier avril elles ne feraient **rien du tout**. Les issues correspondantes de
la roulette sont livrees avec un poids de `0`. Passer les unes a `true` et les
autres a un poids non nul pendant l'evenement.

## La roulette

`chaos_outcomes` liste les issues de la pilule `chaos`. Chaque issue porte une
`category` (`Good`, `Bad`, `Neutral`), un `weight` a l'interieur de sa
categorie, et exactement les memes champs qu'une pilule : effets, objets, soin,
degats, `behaviour`.

La categorie est tiree en premier, selon `chaos_good_weight`,
`chaos_bad_weight` et `chaos_neutral_weight` (34 / 33 / 33 par defaut), puis
l'issue est tiree au poids dans la categorie retenue. Une issue de poids `0`
n'est jamais tiree, et une categorie vide se replie sur une categorie non vide.

## Affichage

Les messages passent par **HintServiceMeow** (paquet `HintServiceMeow-LabAPI`),
sur la ligne `hint_y_coordinate` (700 par defaut). Chaque plugin de la
collection a sa propre ligne, ce qui evite qu'ils s'ecrasent mutuellement.

- Le message d'une pilule s'affiche **avant** ses effets : un effet qui echoue
  ne peut plus le faire disparaitre.
- `show_item_info` (actif) affiche le nom et la description d'une pilule quand
  on la ramasse ou qu'on la selectionne, sur la meme ligne, au format
  `item_info` de la traduction.
- `broadcast_messages` (inactif) double le message en broadcast personnel.
  A activer si les hints ne s'affichent pas sur le serveur.
- `hint_y_coordinate` et `hint_font_size` sont bornes au chargement pour rester
  a l'ecran.

Si HintServiceMeow n'est pas installe sur le serveur, le plugin bascule
automatiquement sur les hints natifs du jeu et le signale une fois dans les
logs.

## Halo

Chaque pilule a sa propre couleur, declaree en hexadecimal dans sa definition.
Les 44 couleurs par defaut sont distinctes ; le plugin previent au demarrage si
deux entrees partagent une couleur.

Le systeme est centralise : un seul abonnement a `ItemPickupBase.OnPickupAdded`
/ `OnPickupDestroyed` pour toutes les pilules, une indexation par numero de serie
du pickup, et un plafond `max_active_glows` (chaque halo est un objet reseau).

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `scp500 list [filtre]` | `scp500s.give` | Liste les pilules enregistrees, avec id, poids et plafond |
| `scp500 give <joueur> <cle> [n]` | `scp500s.give` | Donne `n` exemplaires d'une pilule (1 par defaut, 8 max) |
| `scp500 give * <cle> [n]` | `scp500s.give` | Distribue a tous les joueurs vivants |

La cle accepte aussi l'identifiant numerique. Les emplacements d'inventaire
pleins sont respectes, et chaque distribution est tracee en `Logger.Info` avec
l'auteur et la cible.

## Garde-fous

| Cle | Defaut | Role |
|---|---|---|
| `pill_cooldown_seconds` | `3` | Anti-chainage par joueur : pendant ce delai la prise est bloquee, la pilule reste en inventaire |
| `allow_scp_usage` | `false` | Les SCP ne consomment pas de pilules |
| `allow_scp_targets` | `false` | Les SCP ne sont cibles ni par swap, ni par summon, ni par contagion |
| `ignored_roles` | Tutorial, Overwatch, Filmmaker | Ni consommation ni ciblage |
| `max_effect_duration` | `300` | Borne toute duree d'effet **non nulle** |
| `max_active_glows` | `60` | Plafond d'objets lumineux reseau |
| `max_inventory_slots` | `8` | Refuse de donner un objet si l'inventaire est plein |
| `random_pill_share_percent` | `40` | Part des pilules placees qui sont la roulette |
| `giant_scale` | `1.4` | Borne a 3 pour eviter de coincer un joueur dans le decor |
| `contagion_radius` | `12` | Portee en metres de la contagion |

Les annonces de `cassie_lines` sont validees au premier round, une fois la
base C.A.S.S.I.E. chargee par le jeu : une ligne refusee est signalee dans les
logs et ne sera jamais jouee.

Toutes les actions differees (fusee de `SkyLaunch`, restauration des lumieres
apres `DiscoLights`) sont suivies et tuees a `Disable` comme au changement de
round, et les lumieres repeintes sont restaurees. Un `reload` a chaud ne laisse
ni coroutine ni salle coloree derriere lui.

Les comportements a fort impact (changement de camp, teleportation, permutation,
invocation, projection, vague forcee, contagion, annonce) sont traces en
`Logger.Info` avec le `UserId` de l'auteur et celui de la cible, que la pilule ait
ete ramassee ou tiree par la roulette.

## Dependances

Voir le [README racine](../../README.md) pour la compilation et l'installation.

| Fichier | Destination |
|---|---|
| `SCP500s.dll` | `LabAPI/plugins/<port>/` |
| `AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| HintServiceMeow | `LabAPI/plugins/<port>/` |

La config et les textes vivent dans `LabAPI/configs/<port>/SCP500s/config.yml` et
`translation.yml`. Le plugin s'active dans `properties.yml`, cree par LabAPI : il
n'y a plus de cle `is_enabled` a la racine de la config.

## Note de portage

La version 4.x ciblait EXILED 9.8.1 et ne compilait pas en l'etat (`using
ItemGlow;` sans reference correspondante). La reecriture a extrait le code de
halo duplique dans les treize fichiers, remplace les treize abonnements a
`UsedItem` par une **repartition centrale en O(1)** par numero de serie, et
supprime la config qui serialisait des instances completes de `CustomItem`
sans jamais les utiliser a l'enregistrement.

La 6.0 a supprime les huit sous-classes `CustomItem` a comportement special.
Une pilule et une issue de roulette partagent desormais le meme type de charge
utile, applique par le meme executeur, ce qui permet a la roulette de tirer
n'importe quel comportement du catalogue sans le dupliquer.

La 7.0 remplace `CustomItem`, absent de LabAPI, par un suivi des numeros de serie
dans `PillRegistry`. Les comportements utiles sont conserves : une pilule tombe
au sol a la mort de son porteur et lors d'un changement de role, reapparait a ses
pieds apres une evasion, et traverse SCP-914 sans etre transformee.
