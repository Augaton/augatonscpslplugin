# BetterCoinflipsRewritten 3.0


> Portage LabAPI 1.1.7 d'un plugin de **Mikihero, Matisiowy**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Pile ou face : la piece declenche un effet benefique ou nefaste.

**LabAPI 1.1.7** — `dotnet build -c Release` a la racine du depot

## Consommation de la piece

`consume_coin_on_flip` est a **`true`** par defaut : la piece est detruite apres
le lancer. Un joueur ne peut donc plus rester assis sur une piece a enchainer
les tirages jusqu'a decrocher le bon effet, ce que le cooldown par joueur ne
faisait que ralentir.

La destruction est **differee** de `coin_consume_delay` (1,5 s par defaut). Elle suit la piece :
lachee puis ramassee par un autre pendant ce delai, elle disparait de son inventaire, et elle ne
peut pas etre relancee entre-temps.
`PlayerEvents.FlippingCoin` part avant l'envoi du RPC qui lance l'animation :
retirer l'objet pendant l'event la casse et laisse le jeu manipuler un item
detruit. Ne pas descendre sous la seconde.

Lacher la piece pendant ce delai ne la sauve pas : si elle n'est plus dans
l'inventaire, le pickup correspondant est detruit au sol par son numero de
serie.

Le seul moyen de finir un lancer avec plus de pieces qu'au depart est l'effet
`double_coin_chance`, volontairement rare.

## Spawn des pieces

Deux mecanismes distincts, qui ne font pas la meme chose.

### `add_bonus_coins_to_loot` (nouveau, `true` par defaut)

Ajoute `bonus_coin_amount` pieces **a cote** du loot existant, sans rien
detruire. C'est la contrepartie de la consommation : les pieces brulent a
l'usage, il en faut donc davantage sur la carte.

`bonus_coin_source_items` liste les pools de loot qui recoivent une piece en
plus. `bonus_coins_per_source_type` plafonne le nombre de pieces posees par type
d'objet, pour ne pas concentrer les dix pieces sur les seuls medkits.

Comme rien n'est detruit, ce mode **cohabite avec SCP500s** : les deux plugins
ne se disputent aucun pickup.

### `spawn_coins_on_map` (`false` par defaut)

Remplacement pur et simple du loot par des pieces. Le remplacement appartient a
SCP500s, qui place pieces et pilules avec un ratio unique (10 % piece, 90 %
pilule).

Ne repasser cette option a `true` que si SCP500s est absent ou si son mode de
spawn est `SpawnPoints`. Sinon les deux plugins se disputent les memes pickups
et aucun des deux ratios ne tient.

Les largages de **SupplyDrop** embarquent egalement deux pieces par caisse, MTF
comme Chaos.

## Effets

Les effets saisonniers du jeu (`BecomingFlamingo`, `Prismatic`, `Metal`,
`SugarRush`) sont **inutilisables** : le jeu ne les applique que pendant
Halloween, Noel ou le 1er avril. Aucun effet du plugin ne s'appuie dessus,
sinon un tirage sur trois ne ferait rien onze mois par an.

### Face

Carte d'acces, medkit, teleportation a l'evasion, soin, vie maximale, SCP-268,
Logicer a une balle, SCP-2176, bonbon rose, changement de taille, bouclier
temporaire, fantome, montee d'adrenaline, piece dedoublee, seconde chance, jackpot,
chargeur plein, soin de groupe, detecteur de SCP.

### Pile

Perte de vie, cellules Classe-D, coupure de lumiere, grenade aveuglante, 1 PV,
inventaire vide, grenade instantanee, teleportation aleatoire, ivresse, trip
arc-en-ciel, amnesie, projection en l'air, echange de position avec un autre
joueur, fausse detonation d'ogive, tantrum SCP-173, petrification, lumieres en
mode discotheque, invasion de souris, annonce C.A.S.S.I.E. moqueuse, fausse
explosion, desarmement, position revelee aux SCP, fausse alerte C.A.S.S.I.E.

Un effet qui n'a pas de sens sur le moment n'est pas tire : pas de chargeur plein sans arme entamee, pas de
detecteur sans SCP. La seconde chance ne vaut que pour le role en cours.

`disco_lights`, `mice_invasion` et `cassie_mock` touchent tout le serveur : ils
passent par `facility_effect_cooldown`, comme `lights_out`.

`position_swap` deplace un second joueur, qui recoit son propre message. Son
poids reste bas pour cette raison.

Une fois la LCZ decontaminee, aucun effet n'y envoie plus personne : la
teleportation aleatoire choisit une autre zone, et `tp_to_class_d_cells` comme
`position_swap` sortent du tirage quand ils n'ont plus de destination sure.

Les annonces de `cassie_mock_lines` sont validees au premier lancer de la
session, une fois la base C.A.S.S.I.E. chargee par le jeu : une ligne refusee
est signalee dans les logs et n'est jamais jouee.

## Affichage

Les messages passent par **HintServiceMeow** sur la ligne
`hint_y_coordinate` (750 par defaut), pour ne pas ecraser ceux des autres
plugins. Repli automatique sur les hints natifs si le service est absent.

Les textes joueur sont integralement en francais dans le fichier de traduction.

## Configuration notable

| Cle | Defaut | Role |
|---|---|---|
| `coin_cooldown` | `5` | Cooldown par joueur entre deux lancers |
| `log_flips` | `false` | Journalise chaque lancer. Une ecriture disque par lancer |
| `facility_effect_cooldown` | `60` | Cooldown **serveur** des effets a portee globale |
| `spawn_coins_on_map` | `false` | Voir ci-dessus |
| `consume_coin_on_flip` | `true` | Detruit la piece apres le lancer |
| `coin_consume_delay` | `1.5` | Delai avant destruction, laisse l'animation finir |
| `add_bonus_coins_to_loot` | `true` | Ajoute des pieces sans detruire de loot |
| `bonus_coin_amount` | `10` | Nombre de pieces ajoutees |

Les poids de chaque effet et les tunables associes restent configurables
individuellement. Les textes joueur sont dans le fichier de traduction.

## Passe d'optimisation

- `SelectEffect` n'alloue plus. Il utilisait `.Where().ToList()` puis `.Sum()`
  a chaque lancer ; il remplit maintenant un tampon preexistant.
- `RandomTeleportEffect` s'appuie sur un cache de salles reconstruit au debut du
  round. Il faisait auparavant un `Room.List.Where(...).ToList()` complet dans
  `CanApply`, donc a chaque lancer, puis un second dans `Execute`.
- `MedicalKitEffect` et `CoinSpawnService` n'utilisent plus LINQ.
- Le `Logger.Info` par lancer est passe derriere `log_flips`.
- Les quatre `System.Random` instancies a quelques microsecondes d'intervalle
  sont remplaces par une instance partagee. Sur .NET Framework, la graine derive
  du compteur de ticks : deux instances proches peuvent produire la meme suite.
- La coroutine de spawn est tuee au redemarrage de round et a la desactivation.

## Cycle de vie

Les effets differes (destruction de la piece, retour des lumieres, fin de
l'ecran noir) passent par `API.Scheduler`, qui garde les handles MEC et les tue
au debut du round, au `RoundRestarted` et dans `Disable`. Aucune coroutine ne
survit a une desactivation.

## Passe de securite

- `LightsOutEffect` coupait toute la facility depuis la piece d'un seul joueur,
  sans cooldown global : plusieurs joueurs pouvaient enchainer les extinctions
  en continu. Il passe maintenant par `facility_effect_cooldown`, un verrou
  serveur et non par joueur.
- Suppression du code mort : `Commands/CoinUsesCommand.cs` (classe vide
  compilee dans la DLL), `ForceRespawnChance` (declare, jamais consomme) et
  l'abonnement `Verified` qui ne faisait rien.

## Point restant

`InstantExplosionEffect` attribue la grenade au lanceur via
`TimedGrenadeProjectile.SpawnActive(position, type, player, fuseTime)`. Les coequipiers tues dans le rayon comptent
donc comme des teamkills de sa part, ce qui peut declencher un autoban
anti-TK selon les plugins installes. Non corrige : le comportement souhaite
depend de votre moderation.

## Portage LabAPI

Le comportement en jeu reste celui de la version EXILED, a ces ecarts pres :

- **Inventaire plein.** `AddItem` d'EXILED ignorait la limite de 8 emplacements,
  celui de LabAPI refuse. La carte d'acces, le medkit, le SCP-268 et le SCP-2176
  tombent donc au sol aux pieds du joueur au lieu de creer un 9e objet.
- **Teleportation aleatoire.** LabAPI ne nomme pas les couloirs. Le message
  affiche la zone et la forme, par exemple `HeavyContainment Curve`.

## Dependances

Ce plugin depend de **AugatonLib**, la bibliotheque partagee de la
collection, et de HintServiceMeow pour l'affichage.

| Fichier | Destination |
|---|---|
| `out/plugins/BetterCoinflipsRewritten.dll` | `LabAPI/plugins/<port>/` |
| `out/dependencies/AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| `HintServiceMeow-LabAPI.dll` | `LabAPI/plugins/<port>/` |
| `0Harmony.dll` (livre avec HintServiceMeow) | `LabAPI/dependencies/<port>/` |

`AugatonLib.dll` ne va **jamais** dans `LabAPI/plugins/<port>/` : LabAPI
tenterait de le charger comme plugin. Il doit etre deploye avant ce plugin et
mis a jour en meme temps. Si plusieurs plugins de la collection sont installes,
garder la version d'AugatonLib la plus recente : elle est partagee par tous.

## Installation

1. Compiler depuis la racine du depot avec `dotnet build -c Release`. Les
   assemblies du jeu viennent de `tools/fetch-references.sh`, ou de `SL_REFERENCES`
   vers `SCPSL_Data/Managed`.
2. Copier les fichiers du tableau ci-dessus, puis redemarrer le serveur.
3. Regler le plugin dans `LabAPI/configs/<port>/BetterCoinflipsRewritten/`, avec
   `config.yml` pour les reglages et `translation.yml` pour les textes.

L'activation du plugin se regle dans `properties.yml`, cree par LabAPI : la cle
`is_enabled` n'existe plus. Les autres cles et le format de leurs valeurs ne
changent pas, y compris `bonus_coin_source_items`, qui reste une liste d'`ItemType`.

`coinflip status`, ou `bcf status`, affiche l'etat du plugin. La commande demande
la permission `bettercoinflips.manage`.

## Renouvellement des pieces

Avec `coin_renewal`, les pieces reapparaissent pendant le round sur des emplacements de loot surs,
jamais pres d'un joueur vivant. Le plugin maintient `coin_renewal_target` pieces au sol, et en ajoute
`coin_renewal_batch` au plus toutes les `coin_renewal_interval_seconds` secondes. Cette cible monte de
`coin_chaos_target_per_level` a chaque palier de `coin_chaos_step_seconds` secondes, jusqu'a
`coin_chaos_max_level` paliers. Avec les valeurs par defaut, elle passe de 6 a 16 pieces.

`pause_during_gamemodes` suspend ce renouvellement pendant un mode de type arene de RandomGamemode. Sans ce plugin, le reglage n'a aucun effet.
