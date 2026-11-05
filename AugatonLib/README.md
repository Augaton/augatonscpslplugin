# AugatonLib

Bibliotheque partagee par les plugins LabAPI de la communaute **Zone-Shilari**.

**LabAPI 1.1.7** — compilee avec le reste du depot, voir le [README racine](../README.md).

## Deploiement

Ce n'est **pas** un plugin. Le fichier va dans :

```
LabAPI/dependencies/<port>/AugatonLib.dll
```

Jamais dans `LabAPI/plugins/` : ce dossier est reserve aux assemblies qui contiennent un plugin.
LabAPI charge `dependencies/` en premier, ce qui rend la bibliotheque disponible a tous les plugins.

Il doit etre mis a jour en meme temps que les plugins qui en dependent.

## Contenu

| Espace de noms | Role |
|---|---|
| `Hints.HintChannel` | Canal de hint isole par plugin, adosse a HintServiceMeow, avec repli sur les hints natifs. Chaque plugin a le sien : `plugin.Hints` |
| `Hints.IHintSettings` | Position et taille des hints, appliquees a chaque chargement de la config |
| `Arbitration.LightArbiter` | Pannes et teintes, globales ou par salle |
| `FriendlyFire.FriendlyFireArbiter` | Arbitrage de `Server.FriendlyFire` entre plugins,  |
| `Caching.RoomCache` | Cache des salles eligibles, reconstruit au debut du round |
| `Caching.CooldownTracker` | Cooldowns indexes sur `UserId` |
| `Random.Chance` | Tirage pondere, melange Fisher-Yates, jets en pourcentage |
| `Text.SafeText` | Assainissement de tout texte controle par un joueur avant affichage |
| `Commands.StaffCommand` | Socle de commande staff : permission, bornage des arguments, capture d'exception, audit |
| `Commands.StaffParentCommand` | Commande parente listant ses sous-commandes automatiquement |
| `Commands.StatusCommand` | Sous-commande `status` uniforme |
| `Integrations.UncomplicatedBridge` | Pont vers UncomplicatedCustomItems, CustomRoles et CustomTeams |
| `Runtime.AugatonPlugin` | Classe de base : config, traduction, validation rejouee a chaque rechargement |
| `Effects.EffectLookup` | Effet recherche par nom de classe exact, et verification d'un nom de config |
| `Runtime.PlayerDeparture` | Depart d'un joueur tant que son etat est lisible, voir plus bas |
| `Spawning.RoleSpawn` | Point d'apparition d'un role |
| `World.Mice` | Apparition des souris, avec controle du type |
| `Text.RoleNames` | Nom complet d'un role |

## Depart d'un joueur

`PlayerEvents.Left` arrive trop tard pour lire l'etat d'un joueur. Dans l'ordre, le jeu :

1. retire la connexion de `NetworkServer.connections` ;
2. appelle `CustomNetworkManager.OnServerDisconnect`, qui tue le joueur si `disconnect_drop` est
   actif, ce qui est le cas par defaut : ragdoll, objets au sol, passage en spectateur ;
3. detruit l'objet reseau. Le serveur tourne en mode hote, donc `OnStopClient` passe le role a
   `Destroyed`, meme si le joueur etait encore en vie ;
4. detruit le `GameObject`, et `ReferenceHub.OnDestroy` declenche `PlayerEvents.Left`.

`PlayerDeparture.Departing` part entre les etapes 1 et 2, le point qu'utilisait EXILED. Il intercepte
le delegue public `NetworkServer.OnDisconnectedEvent`, sans patch Harmony, et vaut donc aussi avec
`disconnect_drop` coupe, le mode dieu ou la protection d'apparition.

`PlayerEvents.Left` reste le bon choix pour un simple nettoyage d'etat. Il sert aussi pour les
dummies, qui n'ont pas de connexion et ne levent pas `Departing`.

## Assainissement des textes joueur

Un pseudo SCP:SL peut contenir des balises rich text. Diffuse tel quel dans un
broadcast ou un hint, il permet a n'importe quel joueur d'imposer une taille de
police, une couleur ou un `<link>` malforme a tous les autres — spam visuel au
mieux, crash client au pire.

`SafeText.Sanitize` retire les chevrons, les caracteres de controle, les
caracteres de formatage et les zero-width, borne la longueur, et renvoie un
repli si rien ne subsiste. Les emoji sont preserves.

```csharp
Announce(Text.Winner.Replace("%PLAYER%", SafeText.Nickname(player)));
```

**Regle** : tout texte venant d'un joueur et affiche a un autre joueur passe par
`SafeText`. Les logs serveur et les reponses de console staff gardent la valeur
brute, qui est ce que le staff a besoin de voir.

## Le pont Uncomplicated

Les trois plugins Uncomplicated ne sont pas distribues sur NuGet. Le pont les atteint par
reflexion, sur des signatures verifiees contre leurs sources :

| Cible | Catalogue | Application |
|---|---|---|
| Roles | `CustomRole.List` | `SummonedCustomRole.Summon(Player, ICustomRole)` |
| Objets | `CustomItem.List` | `new SummonedCustomItem(ICustomItem, Player)` |
| Equipes | `Team.List` | `TeamSpawner.SpawnSpecificTeam(Team, bool, bool)` |

`Player` est `LabApi.Features.Wrappers.Player`. L'assembly des objets s'appelle
**`UncomplicatedCustomItems-LabApi`**, pas `UncomplicatedCustomItems`.

Si un plugin Uncomplicated est absent, la partie correspondante se signale comme indisponible et
les fonctionnalites qui en dependent se desactivent d'elles-memes.
