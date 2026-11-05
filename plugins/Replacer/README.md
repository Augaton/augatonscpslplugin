# Replacer 3.1


> Portage LabAPI d'un plugin de **DGvagabond**. Depot non affilie a
> l'auteur d'origine. Voir [NOTICE.md](NOTICE.md) pour l'attribution.

Remplace un joueur qui quitte la partie par un spectateur.

## Fonctionnement

Quand un joueur vivant se deconnecte, le plugin capture un instantane de son etat
(role, position, orientation, vie, AHP, menottes, effets, donnees SCP-079) puis
attribue ce role a un spectateur eligible. Le niveau de SCP-079 est recalcule par
le jeu a partir de son experience.

Les objets passent au remplacant tels quels, dans la meme frame : accessoires,
chargeur, usure, et suivi des objets personnalises comme les pilules de SCP500s.
Un objet que le jeu refuse de lacher, comme SCP-1344 actif, est remplace par un
objet neuf du meme type.

La capture passe par `AugatonLib.Runtime.PlayerDeparture`, et non `PlayerEvents.Left`,
qui voit deja le joueur mort avec `disconnect_drop` actif.

## Remplacement differe

Sans spectateur eligible, le role reste a prendre pendant `pending_role_seconds` : il revient au
premier joueur qui devient spectateur, ou a celui qui se reconnecte. Le remplacant apparait a la
place du joueur parti et y retrouve ses objets au sol. L'annonce d'extinction de son equipe attend
le resultat.

## Prisonniers

Un joueur menotte reste menotte par le meme gardien. Si c'est le gardien qui part, son remplacant
reprend ses prisonniers, a condition que `transfer_cuffs` et `transfer_position` soient actifs. Ce
transfert ne s'applique pas a un remplacement differe.

## Selection du remplacant

Un spectateur est eligible s'il est verifie, hors Overwatch, hors cooldown, et s'il n'est
pas un dummy. Les dummies ne comptent pas non plus dans `minimum_players`.
Avec `use_fair_queue: true` (defaut), c'est le spectateur qui attend depuis le
plus longtemps qui est choisi, au lieu d'un tirage aleatoire. Le cooldown
`per_player_cooldown_seconds` evite qu'un meme joueur enchaine les remplacements. Il
survit a une reconnexion et se remet a zero a chaque round.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `pause_during_gamemodes` | `true` | Ne remplace personne pendant un mode arene |
| `replacement_delay` | `0.4` | Delai avant application de l'etat, laisse le spawn se terminer |
| `max_round_seconds` | `0` | Fenetre de remplacement apres le debut du round. `0` = illimite |
| `minimum_players` | `4` | Nombre de joueurs connectes requis |
| `replaceable_roles` | `[]` | Roles remplacables. Vide = tous les roles vivants |
| `ignored_roles` | Tutorial, Overwatch, Filmmaker, SCP-049-2 | Roles jamais remplaces |
| `transfer_position` / `transfer_health` / `transfer_inventory` | `true` | Elements transferes |
| `transfer_cuffs` | `true` | Transfert des menottes |
| `transfer_effects` | `false` | Transfert des effets actifs |
| `use_fair_queue` | `true` | File d'attente equitable |
| `per_player_cooldown_seconds` | `120` | Anti-repetition. `0` = desactive |
| `pending_role_seconds` | `30` | Duree de vie d'un role sans remplacant. `0` = role perdu |
| `command_cooldown_seconds` | `3` | Delai entre deux `.noreplace` |
| `announce_to_server` | `false` | Broadcast serveur en plus du hint |

Les textes joueur sont dans le fichier de traduction.

## Dependances

Voir le [README racine](../../README.md) pour la compilation et l'installation.

| Fichier | Destination |
|---|---|
| `Replacer.dll` | `LabAPI/plugins/<port>/` |
| `AugatonLib.dll` | `LabAPI/dependencies/<port>/` |
| HintServiceMeow | `LabAPI/plugins/<port>/` |

## Commande joueur

`.noreplace` (alias `.nr`), dans la console `~`, refuse ou accepte d'etre choisi comme remplacant
jusqu'a la fin du round.

## Commandes staff

| Commande | Permission | Effet |
|---|---|---|
| `replacer status` | `replacer.manage` | File d'attente, cooldown et transferts actifs |

Alias `rep`.

Toutes les commandes de la collection partagent le meme socle : verification de
permission en premiere ligne, arguments bornes en longueur, exceptions
capturees, actions a impact tracees avec l'auteur. Une commande parente sans
argument liste ses sous-commandes.
