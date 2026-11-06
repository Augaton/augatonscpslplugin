# Changelog

Les versions de chaque plugin figurent dans le [README](README.md). Rien de ce qui suit n'a encore été testé en jeu : voir [TESTS-EN-JEU.md](TESTS-EN-JEU.md).

## 0.1.0 - 2026-11-06 (pré-version)

### Nouveaux plugins

- **PocketCorpses** : les corps laissés dans la dimension de poche retombent dans le complexe.
- **MicroHidDock** : le Micro-HID se repose sur son socle, où il se recharge lentement.
- **TickGuard** : surveille les TPS et suspend les événements automatiques si le serveur souffre.
- **DiscordAudit** : envoie les actions du staff vers un webhook Discord.
- **Scp914Recipes** : recettes de SCP-914 réglables en config.
- **AmbientCassie** : annonces d'ambiance de C.A.S.S.I.E pendant le round.
- **Contracts** : un objectif secret par joueur, récompensé s'il le remplit.
- **ScpScaling** : les PV des SCP suivent le nombre de joueurs.
- **Streaks** : séries de kills récompensées, et primes sur les meilleurs joueurs.
- **ScpLevels** : les SCP montent de niveau en tuant.
- **ArcadeShop** : des points gagnés pendant le round, à dépenser avec `.shop`.
- **WarmupLobby** : les joueurs se battent en attendant le round.
- **SpectatorVote** : les morts votent pour le prochain incident avec `.vi`.
- **RoundMutators** : une règle légère sur certains rounds normaux.

### Ajouts

- RandomGamemode : modes Patate chaude, Un traître parmi nous, Roi de la salle, Escorte, Zones condamnées et Course aux générateurs.
- BetterCoinflips : effets Seconde chance, Jackpot, Chargeur plein, Soin de groupe, Détecteur, Désarmement, Repéré et Fausse alerte.
- RoundMutators : mutateurs Nuit noire, Munitions rares, Soins rares, SCP rapides et Humains fragiles.
- SupplyDrop : un largage peut être piégé, et une grenade part quand un humain s'en approche.
- RoundRecap : le mutateur du round et les contrats remplis.
- ArcadeShop et ChaosDirector : un contrat rempli rapporte des points.
- SiteIncidents et SupplyDrop gardent 60 secondes d'écart entre un incident et un largage.
- `docs/PROFILS.md` propose trois sélections de plugins, du serveur calme à l'arcade complet.
- `collection check` vérifie les récompenses des contrats et les points fixes des nouveaux modes.
- AugatonLib : `GamemodeState` voit tous les modes de jeu, `ItemGrant` donne un objet sans qu'il se perde.
- SiteIncidents : un autre plugin peut demander un incident par le bus.
- ChaosDirector : points pour les paliers de série de kills.
- RoundRecap : meilleure série de kills.
- `collection check` vérifie sur le vrai serveur ce que la compilation ne voit pas, et son bilan part tout seul dans la console au premier round.
- Commandes de test pour PocketCorpses et MicroHidDock.
- SiteIncidents : fuite de gaz, surcharge des portes Tesla et coupure de caméras.
- RandomGamemode : modes Cache-cache et Évacuation générale.
- RoundRecap : mode du round, meilleur tueur, première évasion, survivant et échanges à SCP-1162.
- ChaosDirector : points pour les échanges à SCP-1162.
- SCPReplacer annonce ses reprises sur le bus : elles comptent dans le bilan, et le volontaire garde la taille du SCP parti.
- `labapi reload configs` recharge les pilules de SCP500s.
- `scp261 status` indique la salle de chaque machine.
- `collection check` couvre aussi les annonces de TeamGenocide et de BetterCoinflips, les portes de La vie selon Larry et le point de largage.
- La config de BetterCoinflips est décrite en français.
- DiscordAudit reçoit aussi les alertes du serveur et le bilan de round.
- Le niveau de chaos pilote les largages piégés, les mutateurs en cours de round et le ton des annonces.
- ScpLevels annonce chaque niveau : points de chaos, ligne « SCP le plus dangereux » au bilan, réaction de C.A.S.S.I.E.
- AmbientCassie réagit aux mutateurs et aux niveaux de SCP.
- Contracts ajoute quatre contrats : pièces, pilules, SCP-261 et SCP-1162.
- ArcadeShop paie les paliers de série et vend le changement de contrat avec `.shop contrat`.
- Le vainqueur d'un mode est nommé au bilan, et il commence le round suivant avec des points et une avance de série.
- SpectatorVote propose aussi des largages et des mutateurs, et n'a plus besoin de SiteIncidents pour s'ouvrir.
- WarmupLobby rappelle le vote du mode.
- Pièces et pilules gagnent l'incident provoqué et le largage surprise.
- MicroHidDock réagit aux incidents : une surcharge Tesla accélère la recharge, une panne la coupe.
- SupplyDrop compense un peu les mutateurs Munitions rares et Soins rares.

### Corrections

- Sans RealisticSizes, une taille réduite revenait après la mort et restait jusqu'à la fin du round.
- Une pièce lâchée pendant son délai de consommation survivait, et deux joueurs pouvaient l'alterner sans limite.
- Une pièce lancée ne peut plus servir une seconde fois dans SCP-261, SCP-1162 ou SCP-914 avant de disparaître.
- `.human` faisait perdre un rôle de SCP quand personne ne pouvait le reprendre.
- `.human` et `.volunteer` pouvaient casser les modes de jeu fondés sur des SCP.
- Pilules, pièces, SCP-1162, Replacer et ChaosDirector ignoraient en partie les modes arène.
- SCP-261 pouvait consommer une pièce sans rien donner.
- Le bulletin d'un joueur parti comptait encore au vote.
- L'échange de position d'une pièce pouvait viser le rôle Tutorial.
- Après l'ogive, une pièce ou une pilule pouvait renvoyer un survivant dans le complexe détruit, sans sortie.
- `collection check` prenait les bruitages de C.A.S.S.I.E pour des mots inconnus.
- Un mode de jeu qui plante se termine proprement au lieu de rester actif à moitié appliqué.
- Les grenades piégées et aveuglantes sans lanceur n'avaient aucun effet avec le tir allié coupé.
- Gun Game et Dodgeball se terminaient dès le départ : le jeu ne voyait qu'un seul camp.
- Le rappel `.human` ne s'affiche plus pendant un mode de jeu, et les refus de `.human` se traduisent.
- SCP-106 ne montait presque jamais de niveau : une mort par décomposition n'a pas de tueur. Elle compte désormais pour lui.
- Un remplaçant reprend le niveau de SCP, les points, le contrat et la série du joueur parti.
- SiteIncidents ne tire plus de panne d'éclairage quand tout le site est déjà dans le noir.
- Les pilules par défaut ont toutes un halo de couleur distincte.

### Sécurité

- Les cooldowns des actions de joueurs ne peuvent plus être mis à zéro par la config.
- Les messages de refus d'une pièce ou d'une pilule sont limités à un par seconde.
- La CI n'a plus que le droit de lecture sur le dépôt et vérifie l'empreinte de gitleaks.
- Un test et le `.gitignore` empêchent de publier une adresse de webhook.

### Performance

- La voix de proximité ne fait plus tourner le codec sans auditeur, et ne part plus au-delà de la portée.
- Le remplacement des pilules au début du round est étalé sur plusieurs frames.
- Les modes de jeu changent les rôles par paquets de 5 par frame.

### Outillage

- Les règles du dépôt sont vérifiées par les tests : abonnements défaits, ressources arbitrées, cooldowns bornés, en-têtes de licence.
- `tools/check.sh` lance en local les contrôles de la CI, et la CI comme la release l'appellent.
- `tools/new-plugin.sh` crée un plugin prêt à compiler, et `tools/deploy.sh` envoie l'archive par SFTP.
- L'archive livre la licence de chaque plugin, chaque DLL embarque la sienne, et `SHA256SUMS` accompagne les releases.
- `package.sh` refuse une archive incomplète, et lit les versions de ses dépendances dans les projets.
- `fetch-references.sh` vérifie l'empreinte des assemblies du jeu, et ne remplace leur dossier qu'après réussite.
- CI : actions épinglées par SHA, caches, délais maximum et `shellcheck`. La publication d'une release a son propre job, seul à pouvoir écrire.

### À savoir pour la mise à jour

- RealisticSizes démarre désormais en mode `Fun`, plus visible sur un serveur arcade. Un `config.yml` existant garde son mode.
- Après une mise à jour du jeu, `fetch-references.sh` et la CI échouent tant que `./tools/fetch-references.sh --update` n'a pas accepté la nouvelle empreinte.
- CorpsesFallPocketDimension et MicroHIDReturn sont remplacés : ne les gardez pas avec PocketCorpses et MicroHidDock.
- `ScaleArbiter.Clear` devient `ResetBaselines`, et le sujet de bus `round.reset` disparaît.
- Le message de bus `round.mutator.started` porte désormais un `RoundMutatorInfo`. `PillConsumed`, `VendingUsed` et `ItemExchanged` portent le joueur.
- Un `config.yml` existant garde ses listes : les nouveaux contrats de Contracts et les nouveaux choix de SpectatorVote s'y ajoutent à la main. Un `translation.yml` existant d'AmbientCassie ne reçoit pas les annonces `Mutator` et `ScpLevel`. Les nouvelles pilules, elles, sont ajoutées toutes seules.
