# Tests en jeu

Tout le depot compile contre le jeu public, mais aucun plugin n'a encore tourne sur un serveur. Ces
points sont ceux que la compilation ne peut pas prouver. Cocher au fur et a mesure.

## 1. Chargement

- [ ] La banniere AugatonLib apparait dans la console et annonce 12/12 plugins enregistres.
- [ ] Aucune erreur de chargement, et HintServiceMeow est detecte.
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
- [ ] Avec `enable_size_gameplay: true` : un petit humain va plus vite, un grand a plus de vie, les
      SCP ne changent pas, et l'effet disparait a la mort et apres `sizes reset`.
- [ ] Fin de round : les tailles reviennent a la normale. C'est un changement : elles restaient avant.

**ScpProximityChat**
- [ ] Touche visible dans les reglages a la connexion, bascule, voix entendue a portee.
- [ ] Chat SCP coupe si `use_default_scp_chat: false`, mode NoClip.
- [ ] Le haut-parleur disparait quand un SCP en proximite se deconnecte.
- [ ] `labapi reload configs` applique un nouveau `volume` sans redemarrer.

**SCP1162**
- [ ] Position de l'anomalie dans `Lcz173`, echange d'armes et de munitions.
- [ ] Correspondance `RoomType` vers `RoomName` du README : seul `Lcz173` a ete verifie.

**SupplyDrop**
- [ ] Armes larguees chargees, y compris revolver et fusil a pompe.
- [ ] Des pilules SCP500s apparaissent dans les largages, et rien ne casse sans SCP500s.
- [ ] Le profil `mtf` part quand les 3 generateurs sont actifs, en respectant `max_drops_per_round`.
- [ ] `decontamination_trigger` et `warhead_trigger` declenchent bien un largage.

**RandomGamemode**
- [ ] Vote : le broadcast apparait dans le lobby, `.votemode` liste les choix, `.votemode 2` vote.
- [ ] Vote : le mode le plus vote demarre, et sans vote un des modes proposes est tire au sort.
- [ ] Vote : un mode qui manque de joueurs au depart est ecarte.
- [ ] `gamemode force` dans le lobby l'emporte sur le vote.
- [ ] Dodgeball : balle x3 visible, explosion 1 s apres le premier rebond, cote lanceur compris.
- [ ] Fin forcee : la limite de temps de `bluescreen` et la victoire a GunGame terminent le round
      en moins de 3 s, meme avec plusieurs camps en vie.
- [ ] Randomiseur : pas de fin avec 2 survivants ou plus d'une meme equipe.
- [ ] Vagues de respawn retablies au round suivant et apres `gamemode stop`.
- [ ] Portails A et B refuses avec le retour visuel, SCP-079 au niveau 3, CASSIE sans fond sonore.

**BetterCoinflipsRewritten**
- [ ] La piece disparait apres l'animation, y compris lachee au sol.
- [ ] Le Logicer garde 1 balle apres ramassage.
- [ ] L'ecran noir et la secousse ne touchent que le lanceur.
- [ ] La souris apparait, la flaque de tantrum est visible et tache.
- [ ] Les annonces CASSIE gardent leurs sous-titres, les grenades respectent leur fuse.
- [ ] `labapi reload configs` applique les nouveaux poids et durees des effets.

**SCP500s**
- [ ] Halo au spawn, apres depot, ramassage et mort, et disparition avec le pickup.
- [ ] Rengainer pendant la prise : effet applique une seule fois.
- [ ] `cure_negative_effects` retire aussi `Blindness` et `Sinkhole`.
- [ ] Mort, forceclass, Betrayal, evasion, SCP-914 : les pilules restent des pilules.
- [ ] Hints a la selection, au ramassage et au `give`.
- [ ] Deguisement, surtout avec un NTF.
- [ ] Tantrum, fusee, `SpawnPoints`, vagues forcees, Cassie.
- [ ] Teinte de salle, y compris deux pilules de suite dans la meme salle.
- [ ] Chaos : le nombre de pilules au sol monte au fil du round, sans depasser `max_pills_on_ground`.
- [ ] Chaos : une pilule consommee reapparait ailleurs apres 20 s, jamais sous les yeux d'un joueur.
- [ ] Chaos : la part de roulettes augmente en fin de round.
- [ ] TPS du serveur stable en fin de round long, avec beaucoup de pilules et de halos.
- [ ] Armes donnees : chargeur et attachements.
