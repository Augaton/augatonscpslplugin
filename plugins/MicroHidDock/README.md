# MicroHidDock

Le Micro-HID peut retourner sur son socle quand celui-ci est vide. Deux gestes suffisent, au choix :

- viser le socle avec le Micro-HID en main et interagir ;
- lacher le Micro-HID a portee du socle.

Il reste le meme objet : il garde son energie et son etat, et n'importe quel joueur peut le reprendre.
Sur le socle, il se recharge lentement. Il ne se pose pas tant qu'il tire ou qu'il monte en puissance.

Aucun son n'est joue : cela exigerait une dependance audio de plus.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `dock_range` | `2` | Distance maximale au socle pour y poser le Micro-HID en le lachant |
| `dock_cooldown_seconds` | `5` | Delai entre deux depots d'un meme joueur |
| `recharge_per_second` | `0.005` | Energie rendue par seconde. `0.005` recharge tout en 200 secondes, `0` coupe la recharge |
| `recharge_limit` | `1` | Niveau d'energie ou la recharge s'arrete, de 0 a 1 |
| `hint_duration`, `hint_y_coordinate`, `hint_font_size` | `4`, `850`, `20` | Affichage du message |

Le message se regle dans `translation.yml`.

## Commandes

| Commande | Permission | Effet |
|---|---|---|
| `microdock status` | `microhiddock.manage` | Socles trouves et energie des Micro-HID poses |
| `microdock test` | `microhiddock.manage` | Vous donne un Micro-HID charge a 10 %, pour tester le depot et la recharge |

Alias `mdock`. Chaque test est trace.

## Attribution

D'apres une idee d'EngineerLND, reecrit sans reprendre son code : voir `NOTICE.md`.
