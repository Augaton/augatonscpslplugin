# AntiSLCrush

Protege le serveur contre des bugs du jeu qui peuvent le faire planter ou bloquer des joueurs.

Ecrit a l'origine par **ui_2506**. Cette version modifiee n'est ni ecrite ni cautionnee par l'auteur.
Voir [NOTICE.md](NOTICE.md). **Licence non commerciale** : voir [LICENSE.md](LICENSE.md). Tout usage
commercial, meme indirect, est interdit sans l'accord ecrit de l'auteur, y compris sur un serveur qui
vend des avantages ou se finance par des dons.

## Protections

Les protections se lisent au demarrage : `labapi reload configs` ne les change pas.

| Reglage | Defaut | Role |
|---|---|---|
| `protect_transport_data` | `true` | Deconnecte les paquets d'une connexion inconnue |
| `protect_player_join` | `true` | Retire les objets detruits avant l'arrivee d'un joueur |
| `protect_reserve_ammo` | `true` | Stoppe l'erreur en boucle d'une arme sans proprietaire |
| `protect_scp330_bag` | `true` | Ignore un second ajout de sac pour le meme joueur dans la meme frame |
| `protect_ads_holster` | `true` | Remet la visee a zero quand un joueur rengaine |
| `clean_observers` | `true` | Retire toutes les 5 s les objets detruits suivis par chaque joueur |

Ces reglages s'appliquent au demarrage du serveur, pas a chaud. Le plugin necessite `0Harmony.dll`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `anticrash status` | `anticrash.manage` | Declenchements et protections en echec |

Alias `asc`. Le premier declenchement de chaque protection apparait aussi en console.

## Modifications par rapport a l'original

- **Plus de methodes du jeu recopiees.** `SpawnObserversForConnection` et `UpdateDelta` ne sont plus
  reecrites : un garde agit avant ou apres le code du jeu, ce qui resiste mieux aux mises a jour.
- **Correction : objets desactives.** L'original retirait aussi de la liste reseau les objets simplement
  desactives. Seuls les objets detruits sont retires.
- **Correction : sacs SCP-330.** L'original bloquait un second sac dans la meme frame pour tout le
  serveur. Le blocage ne concerne plus que le meme joueur.
- **Plus leger.** Le patch de `BroadcastToConnection`, appele a chaque tick pour chaque joueur, devient un
  nettoyage toutes les 5 s. La visee passe d'un transpiler a un simple prefix.
- **Protections isolees.** Une protection cassee par une mise a jour du jeu n'emporte plus les autres, et
  chacune se desactive dans la config.
- **Suivi.** `anticrash status` compte les declenchements. `debug` passe a `false` par defaut.
- **Compilation** contre les assemblies normales du jeu, sans DLL publicisees.

## Non repris

`SecureNetThreading.cs`, qui bloquait les appels reseau hors du thread principal. Il derive d'un depot sans
licence et ne peut pas etre redistribue. Ce controle ne protegeait que contre les plugins mal ecrits, et il
ajoutait un cout a chaque envoi reseau.
