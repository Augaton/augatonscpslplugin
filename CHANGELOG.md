# Changelog

Les versions de chaque plugin figurent dans le [README](README.md). Rien de ce qui suit n'a encore été testé en jeu : voir [TESTS-EN-JEU.md](TESTS-EN-JEU.md).

## Non publié

### Nouveaux plugins

- **PocketCorpses** : les corps laissés dans la dimension de poche retombent dans le complexe.
- **MicroHidDock** : le Micro-HID se repose sur son socle, où il se recharge lentement.
- **TickGuard** : surveille les TPS et suspend les événements automatiques si le serveur souffre.
- **DiscordAudit** : envoie les actions du staff vers un webhook Discord.
- **Scp914Recipes** : recettes de SCP-914 réglables en config.
- **AmbientCassie** : annonces d'ambiance de C.A.S.S.I.E pendant le round.

### Ajouts

- `collection check` vérifie sur le vrai serveur ce que la compilation ne voit pas.
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

### Corrections

- Sans RealisticSizes, une taille réduite revenait après la mort et restait jusqu'à la fin du round.
- Une pièce lâchée pendant son délai de consommation survivait, et deux joueurs pouvaient l'alterner sans limite.
- `.human` faisait perdre un rôle de SCP quand personne ne pouvait le reprendre.
- `.human` et `.volunteer` pouvaient casser les modes de jeu fondés sur des SCP.
- Pilules, pièces, SCP-1162, Replacer et ChaosDirector ignoraient en partie les modes arène.
- SCP-261 pouvait consommer une pièce sans rien donner.
- Le bulletin d'un joueur parti comptait encore au vote.
- L'échange de position d'une pièce pouvait viser le rôle Tutorial.
- Après l'ogive, une pièce ou une pilule pouvait renvoyer un survivant dans le complexe détruit, sans sortie.
- `collection check` prenait les bruitages de C.A.S.S.I.E pour des mots inconnus.
- Un mode de jeu qui plante se termine proprement au lieu de rester actif à moitié appliqué.
- Le rappel `.human` ne s'affiche plus pendant un mode de jeu, et les refus de `.human` se traduisent.

### Sécurité

- Les cooldowns des actions de joueurs ne peuvent plus être mis à zéro par la config.
- Les messages de refus d'une pièce ou d'une pilule sont limités à un par seconde.
- La CI n'a plus que le droit de lecture sur le dépôt et vérifie l'empreinte de gitleaks.
- Un test et le `.gitignore` empêchent de publier une adresse de webhook.

### Performance

- La voix de proximité ne fait plus tourner le codec sans auditeur, et ne part plus au-delà de la portée.
- Le remplacement des pilules au début du round est étalé sur plusieurs frames.

### Outillage

- Les règles du dépôt sont vérifiées par les tests : abonnements défaits, ressources arbitrées, cooldowns bornés, en-têtes de licence.

### À savoir pour la mise à jour

- CorpsesFallPocketDimension et MicroHIDReturn sont remplacés : ne les gardez pas avec PocketCorpses et MicroHidDock.
- Les deux nouveaux modes n'ont pas leurs réglages par défaut dans un `config.yml` existant : ajoutez-les, ou régénérez le fichier.
- `ScaleArbiter.Clear` devient `ResetBaselines`, et le sujet de bus `round.reset` disparaît.
