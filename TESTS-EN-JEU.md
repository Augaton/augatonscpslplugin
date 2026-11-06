# Tests en jeu

Tout le depot compile contre le jeu public, mais aucun plugin n'a encore tourne sur un serveur. Ces
points sont ceux que la compilation ne peut pas prouver. Cocher au fur et a mesure.

## 1. Chargement

La console resume les verifications 20 s apres le premier round. `chaos collection check` donne le detail a tout moment.

- [ ] La banniere AugatonLib apparait dans la console et annonce tous les plugins installes.
- [ ] Aucune erreur de chargement, et HintServiceMeow est detecte avec le `0Harmony.dll` de l'archive.
- [ ] Sans HintServiceMeow, la banniere le signale et le serveur continue de tourner.
- [ ] `config.yml`, `translation.yml` et `properties.yml` sont crees dans `configs/<port>/<Plugin>/`.
- [ ] `labapi reload configs` s'execute sans erreur et applique une nouvelle position de hint.

## 2. Depart d'un joueur (le coeur du portage)

`disconnect_drop` doit rester a sa valeur par defaut, `true`.

- [ ] Replacer : un humain blesse et equipe se deconnecte. Le spectateur recoit son role, sa position,
      sa vie, ses objets et ses munitions, et aucun objet n'est duplique au sol.
- [ ] Replacer : un joueur qui part juste apres son apparition, sous protection, est remplace.
- [ ] Replacer : avec `disconnect_drop: false`, le remplacement fonctionne aussi.
- [ ] Replacer : un kick et un ban declenchent le meme remplacement.
- [ ] Replacer : l'arme transferee garde ses accessoires et son chargeur.
- [ ] Replacer : une pilule SCP500s transferee reste une pilule, avec son effet.
- [ ] Replacer : SCP-1344 actif dans l'inventaire ne bloque pas le transfert.
- [ ] Replacer : un joueur menotte est remplace par un joueur menotte par le meme gardien.
- [ ] Replacer : l'AHP passe au remplacant et s'estompe normalement.
- [ ] Replacer : un dummy spectateur n'est jamais choisi.
- [ ] Replacer : se deconnecter puis revenir ne contourne pas `per_player_cooldown_seconds`.
- [ ] Replacer : SCP-079 se deconnecte. Le remplacant garde son niveau et son energie.
- [ ] SCPReplacer : un SCP a 100 % se deconnecte et la loterie demarre ; sous 95 %, elle ne demarre pas.
- [ ] SCPReplacer et Replacer : un seul des deux traite le depart d'un SCP.
- [ ] Dernier SCP deconnecte : pas d'annonce d'extinction pendant la loterie, et annonce si personne
      ne se porte volontaire.
- [ ] Replacer : un gardien se deconnecte, son remplacant reprend ses prisonniers.
- [ ] Replacer : `.noreplace` empeche d'etre choisi, un second appel annule, et le choix tombe au
      round suivant.
- [ ] Replacer : sans spectateur, le role revient au prochain mort dans les 30 s, a la position du
      joueur parti, avec ses objets au sol. Apres 30 s, le role est perdu et l'extinction annoncee.
- [ ] SCPReplacer : le rappel `.human` arrive bien aux SCP en debut de round.

## 3. Plugins

**AntiSLCrush**
- [ ] `anticrash status` repond, et aucune protection n'est en echec au demarrage.
- [ ] Le jeu fonctionne normalement avec les protections : connexions, visee, sacs SCP-330, munitions.
- [ ] TPS identiques avec et sans le plugin.

**AutoFFToggle**
- [ ] Tir allie coupe au debut du round, reactive avec broadcast a la fin, restaure au restart.

**TeamGenocide**
- [ ] Annonce a la mort du dernier membre, et a sa deconnexion.
- [ ] C.A.S.S.I.E avec et sans sous-titres, teinte par zone puis retour a la normale.
- [ ] Teinte par zone pendant une teinte globale de BetterCoinflips : la salle reprend la bonne couleur.

**RemoteKeycard**
- [ ] Carte en inventaire : porte, generateur, casier, panneau d'ogive.
- [ ] Une porte verrouillee reste fermee, et l'effet Amnesie est respecte.
- [ ] Carte d'un autre niveau tenue en main.

**RealisticSizes**
- [ ] Taille visible par les autres clients, conservee apres mort et respawn, hitbox correcte.
- [ ] `sizes reset *`.
- [ ] Un remplacant de Replacer garde la taille du joueur parti.
- [ ] Avec `enable_size_gameplay: true` : un petit humain va plus vite, un grand a plus de vie, les
      SCP ne changent pas, et l'effet disparait a la mort et apres `sizes reset`.
- [ ] Fin de round : les tailles reviennent a la normale. C'est un changement : elles restaient avant.
- [ ] Sans RealisticSizes : un joueur retreci par SCP-261 ou une pilule retrouve sa taille normale au respawn.

**ChaosDirector**
- [ ] Le niveau monte avec les morts, pilules et pieces, et s'annonce. `chaos status` le confirme.
- [ ] Il accelere les pilules, les pieces et les incidents, et repart de zero au round suivant.
- [ ] Equilibrage : 30 points par niveau et 5 niveaux au maximum sont des valeurs de depart.
- [ ] Pendant un mode arene, le niveau de chaos ne monte pas. Un echange a SCP-1162 rapporte des points.

**TickGuard**
- [ ] `tickguard status` affiche des TPS plausibles et le plus bas du round.
- [ ] Avec `min_tps` au-dessus des TPS reels, l'alerte part apres `checks_before_alert` mesures et les evenements s'arretent, puis reprennent.

**DiscordAudit**
- [ ] Sans webhook, rien ne part et `discordaudit status` le dit.
- [ ] `discordaudit test` arrive dans le salon, sans ralentir le serveur.
- [ ] Un pseudo avec `@everyone` ou des backticks ne sonne personne et ne casse pas le bloc.
- [ ] Une adresse fausse produit un avertissement sans jamais afficher l'adresse.

**Scp914Recipes**
- [ ] Pose au sol et en main : une piece en `Fine` donne parfois une pilule, et sinon la machine agit normalement.
- [ ] La sortie `SCP500` est une vraie pilule avec SCP500s, et une SCP-500 ordinaire sans lui.
- [ ] Une pilule de SCP500s ressort intacte.

**AmbientCassie**
- [ ] `ambient play` se fait entendre avec ses sous-titres, et `collection check` ne signale aucun mot inconnu.
- [ ] Aucune annonce ne coupe une autre annonce, ni le compte a rebours de l'ogive.

**WarmupLobby**

A tester en premier : un defaut ici priverait des joueurs de role.

- [ ] Monter `lobby_waiting_time` a 60. Avec deux joueurs, chacun apparait au meme endroit, dans deux camps ennemis, et ils peuvent se battre.
- [ ] Un mort revient en quelques secondes, dans le camp le moins nombreux.
- [ ] Au lancement, **tout le monde recoit un role normal du round**, SCP compris, et personne ne reste spectateur ni dans son role d'echauffement.
- [ ] L'inventaire de l'echauffement ne survit pas, les corps ont disparu, et le bilan de fin de round ne compte pas ses morts.
- [ ] Pendant l'echauffement : portes, casiers, ascenseurs, menottes, ramassage et lacher d'objets sont refuses, et un mort ne laisse rien au sol.
- [ ] Apres plusieurs kills pendant l'echauffement, aucun bandeau d'objectif n'apparait, et la premiere vague arrive a l'heure habituelle.
- [ ] `first_role` egal a `second_role` : avertissement au chargement et valeurs par defaut remises.
- [ ] `collection check` valide le point d'apparition de l'arene.

**ScpScaling**
- [ ] `scpscaling status` affiche le facteur attendu : 16 joueurs donnent 1.00.
- [ ] Un SCP apparait avec PV pleins et multiplies, barre de vie correcte cote client. SCP-079 et SCP-049-2 ne changent pas.
- [ ] Un SCP repris par Replacer garde les PV du joueur parti. Un mode de jeu a base de SCP garde ses propres PV.

**Streaks**
- [ ] `streaks add <joueur>` jusqu'a 3 : la trousse arrive, et le hint s'affiche. A 5 : prime annoncee a tous.
- [ ] Une grenade, un inventaire plein : l'objet tombe aux pieds du joueur. `SCP500` donne une vraie pilule avec SCP500s.
- [ ] Mourir remet la serie a zero. Tuer un joueur mis a prix donne la prime au chasseur et l'annonce a tous.
- [ ] Le tir allie et le suicide ne comptent pas. Rien ne compte pendant un mode de jeu, Gun Game compris.
- [ ] La meilleure serie apparait dans le bilan.

**ScpLevels**
- [ ] `scplevels add <joueur>` sur un SCP : au niveau 1, les PV max montent et sont soignes, et le hint est vu par tous.
- [ ] La vitesse augmente vraiment, y compris pour SCP-173 et SCP-096. A verifier, ces roles ont leurs propres mouvements.
- [ ] La regeneration agit au niveau 2 et s'arrete aux PV max.
- [ ] Changer de role retire les niveaux, sans laisser ni vitesse ni PV.
- [ ] Equilibrage : les niveaux par defaut sont un point de depart.

**ArcadeShop**
- [ ] `.shop` affiche solde et catalogue. Un kill ennemi rapporte des points, avec un hint.
- [ ] `.shop soin` debite et donne l'objet. Sans assez de points, rien n'est debite.
- [ ] `.shop munitions` ajoute des munitions a la reserve, sans objet dans l'inventaire.
- [ ] La limite par objet tient sur le round, meme apres avoir quitte et rejoint le serveur. Deux achats trop rapides : le second est refuse.
- [ ] Tuer un SCP-049-2 rapporte le prix d'un kill normal, pas celui d'un SCP.
- [ ] Allumer, couper et rallumer un generateur ne paie qu'une fois.
- [ ] Spectateur, SCP, Tutorial ou joueur menotte ne peuvent rien acheter. Boutique fermee pendant un mode de jeu.
- [ ] Le solde repart de zero au round suivant.

**SpectatorVote**
- [ ] Mort, `spectatorvote open` : le hint liste les choix, seuls les spectateurs le voient.
- [ ] `.vi 2` enregistre le vote, et on peut en changer. Un joueur vivant est refuse, comme `.vi 99` ou `.vi abc`.
- [ ] A la fin du vote, le resultat s'affiche et l'incident part avec son annonce de C.A.S.S.I.E.
- [ ] Sans bulletin, rien ne se passe. Sans SiteIncidents, aucun vote ne s'ouvre et `collection check` le signale.

**Contracts**
- [ ] Avec ArcadeShop, un contrat rempli ajoute des points. Sans lui, rien ne change.
- [ ] Chaque humain recoit un hint de contrat au debut du round, et `.contrat` le rappelle.
- [ ] Chaque type se remplit : kills d'ennemis, generateur, evasion, soins, survie.
- [ ] La recompense n'arrive qu'une fois, et un inventaire plein la laisse au sol.
- [ ] Un joueur revenu par une vague recoit un contrat s'il n'en avait pas.
- [ ] Rien n'est distribue ni compte pendant un mode de jeu.

**RoundMutators**
- [ ] `mutators force LowGravity` : les humains sautent plus haut, les SCP non, et `mutators stop` remet tout a la normale.
- [ ] La gravite retombe aussi pour un humain devenu SCP-049-2, et pour un joueur mort.
- [ ] `Swift`, `Tiny` et `Tireless` agissent, et un renfort arrive avec le mutateur en moins de 3 secondes.
- [ ] Une pilule qui rapetisse garde son effet avec `Tiny`.
- [ ] Un mode de jeu force retire le mutateur, et aucun mutateur ne part pendant un mode.
- [ ] `mutators stop` ne retire pas la vitesse d'un SCP monte de niveau.
- [ ] Sur plusieurs rounds, jamais deux mutateurs de suite : le plugin reste discret.
- [ ] `mutators force NightFall` : lumieres coupees tout le round, chaque humain recoit une seule lampe, et tout revient apres `mutators stop`.
- [ ] `ScarceAmmo` et `ScarceMedical` : les objets disparaissent bien de la carte, sans saccade.
- [ ] `SwiftScps` et `FragileHumans` : l'effet suit les renforts, et disparait quand un mode demarre.

**PocketCorpses**
- [ ] Un joueur meurt dans la dimension de poche : son corps retombe du plafond, au-dessus d'un joueur vivant.
- [ ] Le corps tombe vraiment : il ne reste pas fige au plafond, et garde son nom et sa cause de mort.
- [ ] L'alerte sonore retentit avant la chute, et les clients voient le gouffre au sol.
- [ ] Le gouffre ralentit un humain, pas un SCP, et disparait apres `sinkhole_seconds`.
- [ ] SCP-049 peut ranimer le corps retombe.
- [ ] Le corps d'un joueur deconnecte retombe aussi, sans erreur dans les logs.
- [ ] `sinkhole_under_items: true` : un gouffre precede les objets rendus.
- [ ] `pocketcorpses test` : tout le scenario se joue au-dessus du testeur.

**MicroHidDock**
- [ ] Micro-HID en main, l'interaction sur le socle vide fonctionne. Si le socle ne recoit pas l'interaction, lacher le Micro-HID reste possible.
- [ ] Lache a portee, le Micro-HID se pose sur le socle, au bon endroit et sans trembler vu par un second client.
- [ ] Il se reprend normalement, et garde son energie.
- [ ] Il se recharge sur le socle, jusqu'a `recharge_limit`.
- [ ] Un socle occupe refuse un second Micro-HID, et un Micro-HID casse le reste apres un passage sur le socle.
- [ ] `microdock test` donne un Micro-HID a 10 %, et `microdock status` montre sa recharge une fois pose.

**Scp261**
- [ ] `scp261 status` indique la salle de chaque machine.
- [ ] La machine apparait dans `EzOfficeSmall`, et une piece en main permet de l'utiliser.
- [ ] Le bloc simple recoit bien l'interaction, malgre son collider : c'est la version sans ProjectMER.
- [ ] Avec ProjectMER et le schema d'origine, le vrai modele apparait et reagit.
- [ ] Chaque resultat fonctionne : objet, effet, teleportation, tantrum visible, retrecissement, flash, explosion.
- [ ] Inventaire plein : l'objet tombe au sol.
- [ ] Le retrecissement disparait au changement de role, et cohabite avec RealisticSizes.
- [ ] La machine est hors service pendant un mode arene.

**ScpProximityChat**
- [ ] Touche visible dans les reglages a la connexion, bascule, voix entendue a portee.
- [ ] Chat SCP coupe si `use_default_scp_chat: false`, mode NoClip.
- [ ] Le haut-parleur disparait quand un SCP en proximite se deconnecte.
- [ ] `labapi reload configs` applique un nouveau `volume` sans redemarrer.
- [ ] Un humain hors de `max_distance` n'entend plus rien, et un spectateur n'entend que s'il suit un joueur a portee.
- [ ] La premiere syllabe reste nette quand un auditeur arrive a portee apres un silence : le codec n'a pas tourne entre-temps.

**SCP1162**
- [ ] Position de l'anomalie dans `Lcz173`, echange d'armes et de munitions.
- [ ] Correspondance `RoomType` vers `RoomName` du README : seul `Lcz173` a ete verifie.
- [ ] Mode arene : lacher un objet pres de l'anomalie ne fait rien, en Dodgeball comme en Gun Game.

**FairScpSelection**
- [ ] Le plugin se charge avec `0Harmony.dll`, et les SCP sont bien attribues au debut du round.
- [ ] Son interface de lobby reste lisible avec HintServiceMeow.
- [ ] Avec Better3114Spawn, SCP-3114 demarre bien deguise. La combinaison n'a pas ete verifiee.

**SiteIncidents**
- [ ] `incidents trigger Blackout`, `Alert` et `Lockdown` fonctionnent et s'annoncent avec sous-titres.
- [ ] Les annonces C.A.S.S.I.E se prononcent correctement : les mots sont a confirmer.
- [ ] Les portes se debloquent apres le delai, et une porte verrouillee par le staff le reste.
- [ ] Les lumieres reviennent a la normale, meme pendant une autre panne.
- [ ] Rien en zone legere pendant la decontamination, rien apres l'ogive, rien pendant un mode arene.
- [ ] `incidents trigger GasLeak` : une salle occupee passe au vert, et seuls ses humains sont empoisonnes.
- [ ] `incidents trigger TeslaSurge` : les portes Tesla dechargent pendant toute la duree, puis s'arretent.
- [ ] `incidents trigger CameraOutage` : SCP-079 perd ses cameras, et le trigger refuse s'il est absent.

**SupplyDrop**
- [ ] Avec SiteIncidents, un largage automatique et un incident ne partent pas dans la meme minute.
- [ ] Armes larguees chargees, y compris revolver et fusil a pompe.
- [ ] `team_wipe_drops` fonctionne sans TeamGenocide, et une seule fois par equipe et par round.
- [ ] Des pilules SCP500s apparaissent dans les largages, et rien ne casse sans SCP500s.
- [ ] Le profil `mtf` part quand les 3 generateurs sont actifs, en respectant `max_drops_per_round`.
- [ ] `decontamination_trigger` et `warhead_trigger` declenchent bien un largage.
- [ ] Avec `trap_chance_percent: 100` : rien ne part tant que personne n'approche, puis une grenade part apres le delai.

**RandomGamemode**
- [ ] Un traitre ou un VIP qui se deconnecte met fin au round au lieu de le bloquer.
- [ ] Mode arene (Gun Game, Dodgeball) : ni largage, ni pilule, ni piece n'apparait, meme au debut du round, et tout reprend ensuite.
- [ ] Vote : le broadcast apparait dans le lobby, `.votemode` liste les choix, `.votemode 2` vote.
- [ ] Vote : le mode le plus vote demarre, et sans vote un des modes proposes est tire au sort.
- [ ] Vote : un mode qui manque de joueurs au depart est ecarte.
- [ ] `gamemode force` dans le lobby l'emporte sur le vote.
- [ ] Soiree pilules : les pilules recues sont celles de SCP500s, avec leurs effets.
- [ ] Sans BetterCoinflips, Pile ou face royale n'est jamais tire ni propose au vote.
- [ ] Dodgeball : balle x3 visible, explosion 1 s apres le premier rebond, cote lanceur compris.
- [ ] Fin forcee : la limite de temps de `bluescreen` et la victoire a GunGame terminent le round
      en moins de 3 s, meme avec plusieurs camps en vie.
- [ ] Randomiseur, Gun Game et Dodgeball : pas de fin avec 2 survivants ou plus d'une meme equipe.
- [ ] Vagues de respawn retablies au round suivant et apres `gamemode stop`.
- [ ] Portails A et B refuses avec le retour visuel, SCP-079 au niveau 3, CASSIE sans fond sonore.
- [ ] Mode arene : personne n'est remplace, et le Juggernaut qui se deconnecte met fin au mode normalement.
- [ ] `.human` et `.volunteer` sont refuses pendant Cacahuetes, Larry, Mimetisme et Infection, puis remarchent au round suivant.
- [ ] Le rappel `.human` n'apparait pas pendant un mode, et arrive normalement 1 s apres le debut d'un round classique.
- [ ] Un joueur qui vote puis quitte le lobby ne compte plus au depouillement.
- [ ] Cache-cache : le round ne se termine pas pendant la cachette, le chasseur arrive ensuite, et les survivants gagnent a la fin du temps.
- [ ] Cache-cache : si le chasseur se deconnecte pendant la cachette, le round se termine proprement.
- [ ] Evacuation : l'ogive part apres le delai, sans pouvoir etre annulee, et le verrou saute au round suivant.
- [ ] Patate chaude : une balle passe la bombe et ne blesse pas, le porteur meurt a la fin du compte a rebours, et le dernier survivant gagne.
- [ ] Un traitre parmi nous : seul le traitre recoit le message prive, tuer un innocent vide l'inventaire du tireur, et la fin du traitre termine le round.
- [ ] Roi de la salle : la salle s'eclaire en jaune, le score monte seulement pour le camp majoritaire, et les morts reviennent.
- [ ] Escorte : les escortes demarrent avec le scientifique, sa sortie fait gagner le NTF, sa mort ou sa sortie menottee fait gagner le Chaos.
- [ ] Zones condamnees : les zones passent au rouge dans l'ordre, y rester tue, et la surface reste sure.
- [ ] Course aux generateurs : la victoire tombe au troisieme generateur, et la fin du temps fait gagner les SCP.
- [ ] Un config.yml ancien donne bien aux nouveaux modes leurs reglages par defaut, tir allie compris pour Patate chaude.

**BetterCoinflipsRewritten**
- [ ] La piece disparait apres l'animation, y compris lachee au sol.
- [ ] Le Logicer garde 1 balle apres ramassage.
- [ ] L'ecran noir et la secousse ne touchent que le lanceur.
- [ ] La souris apparait, la flaque de tantrum est visible et tache.
- [ ] Les annonces CASSIE gardent leurs sous-titres, les grenades respectent leur fuse.
- [ ] `labapi reload configs` applique les nouveaux poids et durees des effets.
- [ ] Apres l'ogive, aucune piece ni pilule ne teleporte un survivant dans le complexe.
- [ ] Des pieces reapparaissent pendant le round, jamais pres d'un joueur, et leur nombre monte.
- [ ] Lancer, lacher : la piece ramassee par un autre disparait de son inventaire, et il ne peut pas la relancer avant.
- [ ] La taille donnee par une piece disparait au changement de role.
- [ ] Spammer une piece en cooldown n'affiche qu'un message par seconde.
- [ ] Seconde chance : la mort est annulee une seule fois, et disparait au changement de role.
- [ ] Jackpot : 3 objets au sol. Chargeur plein : tire seulement avec une arme entamee dans l'inventaire.
- [ ] Soin de groupe : seuls les allies a portee sont soignes. Detecteur : la distance annoncee est plausible.
- [ ] Desarmement : une arme de l'inventaire tombe au sol, et l'effet n'est pas tire sans arme.
- [ ] Repere : les SCP recoivent le hint pendant la duree prevue, et plus du tout si le joueur meurt.
- [ ] Fausse alerte : C.A.S.S.I.E parle, avec ses sous-titres, et `collection check` ne signale aucun mot inconnu.

**RoundRecap**
- [ ] Le bilan s'affiche en fin de round, et une ligne a zero n'apparait pas.
- [ ] Les compteurs repartent de zero au round suivant.
- [ ] Le bilan affiche le mode du round, et compte les volontaires de SCPReplacer et les echanges a SCP-1162.
- [ ] Le bilan nomme le meilleur tueur, la premiere evasion et le survivant, sans role Tutorial.
- [ ] Le bilan affiche le mutateur du round et les contrats remplis.

**SCP500s**
- [ ] Halo au spawn, apres depot, ramassage et mort, et disparition avec le pickup.
- [ ] Rengainer pendant la prise : effet applique une seule fois.
- [ ] `cure_negative_effects` retire aussi `Blindness` et `Sinkhole`.
- [ ] Mort, forceclass, Betrayal, evasion, SCP-914 : les pilules restent des pilules.
- [ ] Hints a la selection, au ramassage et au `give`.
- [ ] Deguisement, surtout avec un NTF.
- [ ] Tantrum visible par les joueurs, fusee, `SpawnPoints`, vagues forcees, Cassie.
- [ ] Teinte de salle, y compris deux pilules de suite dans la meme salle.
- [ ] Chaos : le nombre de pilules au sol monte au fil du round, sans depasser `max_pills_on_ground`.
- [ ] Chaos : une pilule consommee reapparait ailleurs apres 20 s, jamais sous les yeux d'un joueur.
- [ ] Chaos : la part de roulettes augmente en fin de round.
- [ ] TPS du serveur stable en fin de round long, avec beaucoup de pilules et de halos.
- [ ] Armes donnees : chargeur et attachements.
- [ ] Aucune saccade 5 s apres le debut du round, et tout le loot prevu est bien remplace.
- [ ] `labapi reload configs` applique une pilule ajoutee ou un poids change. Une pilule deja au sol reste utilisable.
- [ ] Sans pilule active ou inventaire plein, SCP-261 donne bien quelque chose au joueur.

## 4. Synergies entre plugins

**Remplacement**
- [ ] Avec Replacer : le remplacant reprend les points et la serie du joueur parti.
- [ ] Un remplacant sans contrat reprend celui du joueur parti et recoit son hint. Un remplacant qui en a deja un garde le sien.
- [ ] Un joueur remplace qui revient dans le round garde ses limites d'achat, et ne recoit pas de nouveau contrat si le sien a ete repris.
- [ ] `.human` : le joueur garde ses points et son contrat, et le volontaire ne recoit rien de lui.
- [ ] Avec SCPReplacer : un SCP de niveau 2 parti, son remplacant a le meme niveau et ses effets, et la vie n'est pas donnee deux fois.

**Discord**
- [ ] `send_alerts` : TPS bas puis retour a la normale, deux messages. Un seul message par type d'exploit bloque.
- [ ] Un probleme de `collection check` au demarrage arrive sur le salon.
- [ ] `send_round_recap` : le bilan arrive sans balise de couleur ni de taille.

**Chaos et niveaux**
- [ ] SCP-106 gagne un niveau quand un joueur meurt dans la dimension de poche.
- [ ] `collection check` ne signale aucun mot inconnu dans les annonces `Mutator` et `ScpLevel`.
- [ ] Niveau de chaos 3 : un mutateur demarre s'il n'y en a pas, et aucun second mutateur ne s'ajoute.
- [ ] Niveau de chaos 2 : les annonces `Late` arrivent avant `late_round_seconds`.
- [ ] Un niveau de SCP ajoute des points de chaos, et le bilan nomme le SCP le plus dangereux.
- [ ] `reaction_cooldown_seconds` espace bien deux reactions de C.A.S.S.I.E.

**Contrats et boutique**
- [ ] Les quatre nouveaux contrats avancent : piece lancee, pilule avalee, SCP-261 utilise, echange a SCP-1162.
- [ ] Une pilule rengainee apres activation compte, une pilule refusee par le cooldown ne compte pas.
- [ ] Une SCP-500 non remplacee fait avancer le contrat et compte au bilan.
- [ ] Sans le plugin concerne, le contrat n'est jamais propose.
- [ ] `.shop contrat` : le contrat change, le type differe, et les points partent une seule fois.
- [ ] `.shop contrat` sans contrat, ou contrat deja rempli : aucun point debite.
- [ ] Un palier de serie rapporte des points de boutique.

**Modes de jeu**
- [ ] Gun Game, Patate chaude et Un traitre parmi nous : le vainqueur est nomme au bilan.
- [ ] Le vainqueur commence le round suivant avec son bonus de points et son avance de serie.
- [ ] WarmupLobby : le rappel `.votemode` s'affiche a l'apparition, au plus une fois par `vote_reminder_seconds`.

**Vote des spectateurs**
- [ ] Les trois types de choix fonctionnent : incident, largage, mutateur.
- [ ] Un mutateur n'est plus propose quand le round en a deja un.
- [ ] Sans SiteIncidents, un vote s'ouvre quand meme avec SupplyDrop et RoundMutators.

**Pieces et pilules**
- [ ] Incident provoque et largage surprise, en piece et en pilule.
- [ ] Ces effets ne sortent pas pendant un mode de jeu, ni sans le plugin cible.
- [ ] `facility_effect_cooldown` espace bien ces effets de piece.
- [ ] Deux pilules d'incident ou de largage a moins de `site_request_cooldown_seconds` : la seconde dit que rien ne se passe.
- [ ] Apres l'ogive, plus d'incident par piece ni par pilule.
- [ ] Soiree pilules : ni pilule d'incident, ni pilule de largage.
- [ ] Profil de largage `is_enabled: false` : le vote annonce que rien ne se produit, et la piece aussi, sans consommer son cooldown.

**Micro-HID et largages**
- [ ] Surcharge Tesla : recharge trois fois plus rapide pendant 30 s, puis retour a la normale.
- [ ] Panne d'eclairage : recharge coupee pendant 20 s.
- [ ] `ScarceAmmo` et `ScarceMedical` : chaque largage contient le bonus de sa config.
- [ ] `trap_chance_percent: 0` : aucun piege, quel que soit le niveau de chaos.

## 5. Ameliorations de plugins

**Modes de jeu**
- [ ] Dodgeball et Randomizer : le dernier debout est annonce, nomme au bilan et recompense.
- [ ] Gun Game gagne au dernier palier : un seul vainqueur est nomme, meme si le round finit avec un survivant.
- [ ] Deux rounds de suite avec un mode : le second n'est ni propose au vote ni tire au sort s'il etait le dernier joue.

**Mutateurs**
- [ ] `Vampire` : un kill rend 25 PV au tueur, jamais au-dela de sa vie maximale.
- [ ] `GlassCannon` : les degats entre joueurs augmentent, pas les chutes, et SCP-173 tue toujours d'un coup.
- [ ] `EndlessAmmo` : la reserve se remplit, jeter des munitions est refuse, et rien ne tombe au sol, ni a la mort ni en changeant de gilet.
- [ ] `Regeneration` : les humains regagnent de la vie, pas les SCP.

**Incidents et chaos**
- [ ] `incidents trigger ElevatorOutage` : les ascenseurs se bloquent 20 s puis repartent, et se debloquent en fin de round.
- [ ] Pendant l'ogive et dans la derniere minute avant la decontamination, l'arret des ascenseurs est refuse.
- [ ] L'ogive leve un arret en cours, et la fin d'un arret laisse en place un ascenseur verrouille par le staff.
- [ ] Le niveau de chaos maximal declenche une seule apogee par round.

**Contrats et progression**
- [ ] Un SCP recoit un contrat et sa vie remonte quand il le remplit. SCP-079 et SCP-049-2 n'en recoivent pas.
- [ ] Un contrat rempli en amene un second apres `follow_up_delay_seconds`, et pas de troisieme.
- [ ] `.shop contrat` sur un contrat suivant redonne un contrat suivant, pas un contrat facile.
- [ ] Un kill de SCP-049-2 fait monter SCP-049 de niveau.
- [ ] Une mort dans la dimension de poche fait avancer le contrat de SCP-106.
- [ ] Relancer le meme generateur ne fait pas avancer un contrat, et un second generateur le fait.
- [ ] La prime grandit avec la serie, et la fin d'une serie sans prime est annoncee une seule fois.
- [ ] Un joueur qui se deconnecte ne declenche pas l'annonce de fin de serie.

**Boutique et machines**
- [ ] `.shop vie`, `.shop bouclier` et `.shop vitesse` fonctionnent, et le soin est refuse en pleine forme, sans debit.
- [ ] Le catalogue affiche les services et leurs limites.
- [ ] SCP-1162 : un objet de rang 2 donne une recompense de rang 2, une lampe donne une recompense courante.
- [ ] Jackpot de SCP-261 : les lots tombent, l'annonce part, et la cagnotte repart de zero.
- [ ] Les nouvelles recettes de SCP-914 fonctionnent posees et portees.

**Largages et equipes**
- [ ] Le profil `neutre` tombe dans une salle, l'annonce nomme la bonne zone, et la balise est visible.
- [ ] Apres l'ogive et pendant son compte a rebours, le profil `neutre` tombe en surface.
- [ ] Juste avant la decontamination, le profil `neutre` evite la zone legere.
- [ ] Le butin du profil `neutre` reste atteignable : ni dans un mur, ni dans un casier ferme.
- [ ] Une equipe reduite a un joueur : annonce, bouclier et objet, une seule fois par equipe et par round.

**Lobby, vote et bilan**
- [ ] `collection check` ne signale aucun mot inconnu dans les nouvelles annonces.
- [ ] Le champion du lobby est annonce au debut du round et recoit ses points de boutique.
- [ ] Vote `BlindScps` : les SCP sont aveugles 5 s, puis l'effet s'arrete. Vote `HealHumans` : les humains sont soignes.
- [ ] Le bilan nomme le premier sang, et le joueur mort le plus souvent a partir de deux morts.
- [ ] Le bilan s'affiche bien apres une annonce de fin de mode.

## 6. Securite et performance

**Modes de jeu**
- [ ] Roi de la salle : aucun objet ni corps au sol apres une mort, et la dotation revient a la reapparition.
- [ ] Roi de la salle : tous les joueurs quittent, et le round se termine ou redemarre tout seul.
- [ ] `held_round_limit_seconds` bas et deux joueurs qui ne se battent pas en Dodgeball : le round se termine.
- [ ] Dodgeball, Randomizer, Soiree pilules et Coin Royale a 20 joueurs et plus : pas de saccade au depart, et tout le monde recoit sa dotation.
- [ ] Un joueur en Tutorial ne retient pas un round Dodgeball.

**Autres**
- [ ] `autoff set on`, puis redemarrage du round : le tir allie revient sous le controle du plugin.
- [ ] Chat de proximite : un SCP qui parle normalement reste intelligible, sans coupure.
- [ ] Alterner vite entre une pilule et un objet : le hint de la pilule s'affiche encore a une selection normale.
- [ ] `PocketCorpses` apres l'ogive : les corps en attente ne font pas baisser les TPS.
