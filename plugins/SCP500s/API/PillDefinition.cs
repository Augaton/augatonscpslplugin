using System.Collections.Generic;
using System.ComponentModel;

namespace SCP500s.API
{
    public sealed class PillDefinition : EffectPayload
    {
        [Description("Identifiant unique de l'objet personnalise. Ne jamais reutiliser un identifiant deja pris.")]
        public uint Id { get; set; }

        [Description("Cle interne, utilisee dans les logs, les commandes et pour desactiver la pilule.")]
        public string Key { get; set; } = string.Empty;

        [Description("Nom affiche en jeu.")]
        public string DisplayName { get; set; } = string.Empty;

        [Description("Description affichee en jeu.")]
        public string Description { get; set; } = string.Empty;

        [Description("Active ou desactive la pilule.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Couleur du halo au sol, au format hexadecimal (#RRGGBB).")]
        public string GlowColor { get; set; } = "#FFFFFF";

        [Description("Poids relatif dans le tirage de remplacement du loot. 0 = jamais tiree au sort.")]
        public int SpawnWeight { get; set; } = 10;

        [Description("Nombre maximum d'exemplaires places par round. 0 = sans limite.")]
        public int MaxPerRound { get; set; } = 2;

        [Description("Emplacements de spawn fixes. Laisser vide si le mode de spawn est LootReplacement.")]
        public List<SpawnLocationType> SpawnLocations { get; set; } = new List<SpawnLocationType>();

        [Description("Limite de spawn pour les emplacements fixes.")]
        public int SpawnLimit { get; set; } = 1;
    }
}
