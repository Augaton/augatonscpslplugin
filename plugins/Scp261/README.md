# Scp261

Ajoute SCP-261, une machine distributrice. Un joueur tenant une piece peut l'utiliser pour obtenir un
resultat au hasard.

D'apres le plugin [Scp261](https://github.com/MedveMarci/Scp261) de **MedveMarci**. Voir [NOTICE.md](NOTICE.md).

| Resultat | Poids | Effet |
|---|---|---|
| Objet | 50 | Donne un objet du stock. Une pilule de SCP500s si ce plugin est installe |
| Effet | 30 | Applique un effet au hasard |
| Teleportation | 10 | Teleporte pres d'une porte deverrouillee |
| Tantrum | 9 | Fait apparaitre une flaque de SCP-173 |
| Retrecissement | 5 | Rapetisse le joueur jusqu'a son prochain changement de role |
| Grenade flash | 5 | Fait apparaitre une grenade aux pieds du joueur |
| Explosion | 1 | Fait exploser le joueur |

Un resultat desactive dans la config donne un objet a la place.

## Modele de la machine

Sans rien de plus, la machine est un simple bloc colore (`fallback_color`). Pour le vrai modele, installez
[ProjectMER](https://github.com/Michal78900/ProjectMER/), puis placez le fichier `Scp261.zip` des
[releases d'origine](https://github.com/MedveMarci/Scp261/releases) dans son dossier de schemas. Le plugin
l'utilise alors automatiquement.

## Configuration

Les cles principales sont `spawn_locations`, `spawn_only_once`, `activation_items`, `vending_machine_stock`,
`effect_list` et les poids `*_chance`. Chaque cle est decrite dans `config.yml`, et les textes sont dans
`translation.yml`. Les pieces viennent par exemple de BetterCoinflips, qui les renouvelle pendant le round.

`pause_during_gamemodes` met la machine hors service pendant un mode de type arene. Le retrecissement passe
par `ScaleArbiter`, donc il cohabite avec RealisticSizes.

## Modifications par rapport a l'original

- **Plus d'appel reseau.** L'original contactait un serveur tiers a chaque round pour verifier les mises a
  jour, ce qui revelait l'adresse du serveur. Cet appel est retire.
- **ProjectMER devient optionnel.** Sans lui, la machine fonctionne avec un modele simple.
- **Correction : effets.** L'original cherchait l'effet par un `Type.GetType` qui ne voit pas les classes du
  jeu : l'effet n'etait jamais applique, alors que le message s'affichait.
- **Correction : tantrum.** La flaque est maintenant creee cote reseau, donc visible des joueurs.
- **Correction : inventaire plein.** L'objet tombe aux pieds du joueur au lieu d'etre perdu avec la piece.
- **Correction : boucle de teleportation.** La recherche d'une position sure a maintenant une limite.
- **Traduction.** Les textes sont en francais, dans `translation.yml`, et les hints passent par
  HintServiceMeow.
- **Collection.** Taille arbitree, cooldown par joueur, pause des modes arene, bilan de round et niveau de chaos.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `scp261 status` | `scp261.manage` | Etat du plugin et salle de chaque machine |

Alias `261`.
