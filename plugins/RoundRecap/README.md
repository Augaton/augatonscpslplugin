# RoundRecap

Affiche un bilan a la fin de chaque round : mode de jeu, morts, evasions, pilules avalees, pieces
lancees, largages, remplacements, incidents de site, echanges a SCP-1162 et equipes eliminees.

Il nomme aussi le meilleur tueur, la premiere evasion et l'humain en vie depuis le plus longtemps. Les
pseudos passent par `SafeText` : un joueur ne peut pas glisser de mise en forme dans le bilan.

Le plugin ecoute le bus d'AugatonLib et ne depend d'aucun autre plugin. Une ligne dont le total vaut
zero n'apparait pas : sans SCP500s, le bilan ne parle simplement pas de pilules.

## Configuration

| Cle | Defaut | Role |
|---|---|---|
| `broadcast_duration` | `10` | Duree du bilan, en secondes |

Les lignes se reglent dans `translation.yml`. Une ligne vide est masquee.
