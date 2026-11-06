using System.ComponentModel;

namespace BetterCoinflipsRewritten
{
    public sealed class Translation
    {
        [Description("Titre affiche au-dessus du resultat du lancer.")]
        public string CoinFlipTitle { get; set; } =
            "<b><color=yellow>PILE OU FACE</color></b>";

        [Description("Message affiche quand aucun effet valide ne peut etre selectionne.")]
        public string NoAvailableEffect { get; set; } =
            "<color=white>Aucun effet disponible n'a pu etre selectionne.</color>";

        [Description("Message affiche quand l'execution d'un effet echoue.")]
        public string EffectError { get; set; } =
            "<b><color=red>Erreur d'effet de piece</color></b>\n" +
            "<color=white>Merci de signaler ce probleme a l'administration du serveur.</color>";

        [Description("Message de cooldown. {seconds} est remplace par le temps restant.")]
        public string CooldownMessage { get; set; } =
            "<b><color=red>La piece est en cooldown</color></b>\n" +
            "<color=white>Temps restant : </color>" +
            "<color=yellow>{seconds} secondes</color>";

        [Description("Ligne de resultat. {result} est remplace par pile ou face.")]
        public string ResultFormat { get; set; } =
            "<color=white>Resultat :</color> {result}";

        [Description("Ligne d'effet. {effect} est remplace par le message de l'effet selectionne.")]
        public string EffectFormat { get; set; } =
            "<color=white>Effet :</color> {effect}";

        [Description("Ligne ajoutee au resultat quand la piece est consommee par le lancer.")]
        public string CoinConsumedLine { get; set; } =
            "<color=#9e9e9e><i>La piece s'est consumee.</i></color>";

        [Description("Nom affiche pour face.")]
        public string HeadsName { get; set; } =
            "<color=green>Face</color>";

        [Description("Nom affiche pour pile.")]
        public string TailsName { get; set; } =
            "<color=red>Pile</color>";

        [Description("Message de soin. {amount} est remplace par les points de vie rendus.")]
        public string HealEffectMessage { get; set; } =
            "La piece vous a rendu {amount} PV.";

        [Description("Message de vie maximale. {percentage} est remplace par le pourcentage d'augmentation.")]
        public string MoreHealthEffectMessage { get; set; } =
            "Votre vie maximale a augmente de {percentage} %.";

        [Description("Message de repli pour l'effet de carte aleatoire.")]
        public string RandomKeycardEffectMessage { get; set; } =
            "Vous avez recu une carte d'acces aleatoire.";

        [Description("Message de carte aleatoire. {keycard} est remplace par le nom de la carte tiree.")]
        public string RandomKeycardSelectedMessage { get; set; } =
            "Vous avez recu une {keycard}.";

        [Description("Nom affiche de la carte d'agent d'entretien.")]
        public string JanitorKeycardName { get; set; } =
            "Carte d'Agent d'Entretien";

        [Description("Nom affiche de la carte de scientifique.")]
        public string ScientistKeycardName { get; set; } =
            "Carte de Scientifique";

        [Description("Nom affiche de la carte de responsable de zone.")]
        public string ZoneManagerKeycardName { get; set; } =
            "Carte de Responsable de Zone";

        [Description("Nom affiche de la carte de garde.")]
        public string GuardKeycardName { get; set; } =
            "Carte de Garde";

        [Description("Nom affiche de la carte de coordinateur de recherche.")]
        public string ResearchCoordinatorKeycardName { get; set; } =
            "Carte de Coordinateur de Recherche";

        [Description("Nom affiche de la carte d'ingenieur de confinement.")]
        public string ContainmentEngineerKeycardName { get; set; } =
            "Carte d'Ingenieur de Confinement";

        [Description("Message quand un medkit est ajoute a l'inventaire.")]
        public string MedicalKitEffectMessage { get; set; } =
            "Vous avez recu un medkit.";

        [Description("Message quand un medkit est lache au sol parce que le joueur en avait deja un.")]
        public string MedicalKitDroppedMessage { get; set; } =
            "Vous aviez deja un medkit, un second a ete lache a vos pieds.";

        [Description("Message de recompense SCP-268.")]
        public string HatEffectMessage { get; set; } =
            "Vous avez recu le SCP-268.";

        [Description("Message de recompense SCP-2176.")]
        public string LightbulbEffectMessage { get; set; } =
            "Vous avez recu le SCP-2176.";

        [Description("Message de recompense du Logicer a une balle.")]
        public string OneAmmoLogicerEffectMessage { get; set; } =
            "Vous avez recu un Logicer avec une seule balle.";

        [Description("Message de recompense du bonbon rose.")]
        public string PinkCandyEffectMessage { get; set; } =
            "Vous avez recu un bonbon rose.";

        [Description("Message de teleportation vers la zone d'evasion.")]
        public string TeleportToEscapeEffectMessage { get; set; } =
            "Vous avez ete teleporte vers la zone d'evasion.";

        [Description("Message de changement de taille. {size} est remplace par la taille tiree.")]
        public string SizeChangeEffectMessage { get; set; } =
            "Votre taille est passee a {size}.";

        [Description("Nom affiche de la petite taille.")]
        public string SmallSizeName { get; set; } =
            "petite";

        [Description("Nom affiche de la grande taille.")]
        public string LargeSizeName { get; set; } =
            "grande";

        [Description("Nom de taille par defaut.")]
        public string ChangedSizeName { get; set; } =
            "une taille differente";

        [Description("Message de perte de vie. {amount} est remplace par les points de vie retires.")]
        public string HpReductionEffectMessage { get; set; } =
            "Vous avez perdu {amount} PV.";

        [Description("Message de l'effet de grenade instantanee.")]
        public string InstantExplosionEffectMessage { get; set; } =
            "Une grenade a explose a vos pieds.";

        [Description("Message de reinitialisation de l'inventaire.")]
        public string InventoryResetEffectMessage { get; set; } =
            "Tout votre inventaire a ete supprime.";

        [Description("Message d'extinction des lumieres. {duration} est remplace par la duree.")]
        public string LightsOutEffectMessage { get; set; } =
            "Les lumieres du complexe ont ete coupees pendant {duration} secondes.";

        [Description("Message de l'effet laissant 1 PV.")]
        public string OneHpLeftEffectMessage { get; set; } =
            "La piece ne vous a laisse qu'un seul PV.";

        [Description("Message de teleportation aleatoire. {room} est remplace par le nom de la salle.")]
        public string RandomTeleportEffectMessage { get; set; } =
            "Vous avez ete teleporte dans une salle au hasard : {room}.";

        [Description("Nom de salle par defaut.")]
        public string UnknownRoomName { get; set; } =
            "salle inconnue";

        [Description("Message de teleportation vers les cellules des Classe-D.")]
        public string TeleportToClassDCellsEffectMessage { get; set; } =
            "Vous avez ete teleporte dans les cellules des Classe-D.";

        [Description("Message de l'effet de grenade aveuglante.")]
        public string TrollFlashEffectMessage { get; set; } =
            "Une grenade aveuglante est apparue a vos pieds.";

        [Description("Message du bouclier temporaire. {amount} est remplace par les points de bouclier.")]
        public string AhpShieldEffectMessage { get; set; } =
            "Un bouclier de {amount} points vous enveloppe.";

        [Description("Message de l'effet fantome. {duration} est remplace par la duree.")]
        public string GhostlyEffectMessage { get; set; } =
            "Vous devenez un fantome pendant {duration} secondes.";

        [Description("Message de la montee de sucre. {duration} est remplace par la duree.")]
        public string SugarRushEffectMessage { get; set; } =
            "Une montee de sucre vous propulse pendant {duration} secondes.";

        [Description("Message de la piece qui se dedouble.")]
        public string DoubleCoinEffectMessage { get; set; } =
            "La piece s'est dedoublee : vous en recuperez deux.";

        [Description("Message de la piece dedoublee lachee au sol faute de place.")]
        public string DoubleCoinDroppedMessage { get; set; } =
            "La piece s'est dedoublee, mais votre inventaire etait plein : les deux pieces sont a vos pieds.";

        [Description("Message de l'effet d'ivresse. {duration} est remplace par la duree.")]
        public string DrunkEffectMessage { get; set; } =
            "Vous titubez pendant {duration} secondes, comme apres une tournee generale.";

        [Description("Message du trip prismatique. {duration} est remplace par la duree.")]
        public string PrismaticTripEffectMessage { get; set; } =
            "Le complexe part en couleurs pendant {duration} secondes.";

        [Description("Message d'amnesie. {duration} est remplace par la duree.")]
        public string AmnesiaEffectMessage { get; set; } =
            "Vous ne reconnaissez plus rien pendant {duration} secondes.";

        [Description("Message de projection en l'air. {height} est remplace par la hauteur.")]
        public string SkyLaunchEffectMessage { get; set; } =
            "La piece vous a expedie a {height} metres du sol. Bon vol.";

        [Description("Message d'echange de position. {player} est remplace par le nom du partenaire.")]
        public string PositionSwapEffectMessage { get; set; } =
            "Vous avez echange de place avec {player}.";

        [Description("Message affiche au joueur echange contre son gre. {player} est remplace par le nom du lanceur.")]
        public string PositionSwapPartnerMessage { get; set; } =
            "<b><color=yellow>PILE OU FACE</color></b>\n" +
            "<color=white>{player} a echange sa place avec la votre.</color>";

        [Description("Message de la fausse detonation d'ogive.")]
        public string FakeWarheadEffectMessage { get; set; } =
            "L'ogive vient d'exploser. Enfin, c'est ce que vous avez cru.";

        [Description("Message du piege tantrum.")]
        public string TantrumTrapEffectMessage { get; set; } =
            "Le SCP-173 a laisse un souvenir sous vos pieds.";

        [Description("Message de petrification. {duration} est remplace par la duree.")]
        public string PetrifiedEffectMessage { get; set; } =
            "Vous etes petrifie pendant {duration} secondes.";

        [Description("Message des lumieres en mode discotheque. {duration} est remplace par la duree.")]
        public string DiscoLightsEffectMessage { get; set; } =
            "Le complexe passe en discotheque pendant {duration} secondes.";

        [Description("Message de l'invasion de souris.")]
        public string MiceInvasionEffectMessage { get; set; } =
            "Une invasion de souris se declare dans l'abri.";

        [Description("Message de l'annonce moqueuse de C.A.S.S.I.E.")]
        public string CassieMockEffectMessage { get; set; } =
            "C.A.S.S.I.E. a un mot a dire sur votre compte.";

        [Description("Message de l'effet seconde chance.")]
        public string SecondChanceEffectMessage { get; set; } =
            "Une seconde chance vous est accordee : votre prochaine mort sera annulee.";

        [Description("Message affiche quand la seconde chance sauve le joueur.")]
        public string SecondChanceUsed { get; set; } =
            "<color=yellow>Votre seconde chance vous a sauve.</color>";

        [Description("Message de l'effet jackpot.")]
        public string JackpotEffectMessage { get; set; } =
            "Jackpot ! Des objets tombent a vos pieds.";

        [Description("Message de l'effet chargeur plein.")]
        public string FullMagazineEffectMessage { get; set; } =
            "Le chargeur d'une de vos armes est plein.";

        [Description("Message de l'effet soin de groupe. {count} est le nombre de joueurs soignes, lanceur compris.")]
        public string GroupHealEffectMessage { get; set; } =
            "Une onde de soin touche {count} joueur(s) autour de vous.";

        [Description("Message de l'effet detecteur. {distance} est la distance du SCP le plus proche, en metres.")]
        public string DetectorEffectMessage { get; set; } =
            "Le SCP le plus proche est a {distance} m.";

        [Description("Message de l'effet desarmement.")]
        public string DisarmEffectMessage { get; set; } =
            "Une de vos armes tombe au sol.";

        [Description("Message de l'effet repere, pour le lanceur.")]
        public string SpottedEffectMessage { get; set; } =
            "Les SCP savent ou vous etes.";

        [Description("Hint des SCP. {player} est le pseudo du joueur repere, {distance} sa distance en metres.")]
        public string SpottedScpHint { get; set; } =
            "<color=red>Proie reperee : {player} a {distance} m</color>";

        [Description("Message de l'effet fausse alerte, pour le lanceur.")]
        public string FalseAlarmEffectMessage { get; set; } =
            "C.A.S.S.I.E. vient de sonner une fausse alerte.";

        [Description("Sous-titres de la fausse alerte.")]
        public string FalseAlarmSubtitles { get; set; } =
            "Attention. SCP detecte dans cette zone.";

        [Description("Message de l'incident provoque.")]
        public string TriggerIncidentEffectMessage { get; set; } =
            "La piece retombe mal : quelque chose se detraque sur le site.";

        [Description("Message du largage surprise.")]
        public string SurpriseDropEffectMessage { get; set; } =
            "La piece brille : un largage arrive sur le site.";

        [Description("Message de la fausse explosion.")]
        public string FakeExplosionEffectMessage { get; set; } =
            "Une explosion vous a traverse sans une egratignure.";
    }
}
