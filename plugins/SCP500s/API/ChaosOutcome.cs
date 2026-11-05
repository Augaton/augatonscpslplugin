using System.ComponentModel;

namespace SCP500s.API
{
    public sealed class ChaosOutcome : EffectPayload
    {
        [Description("Cle interne, utilisee dans les logs.")]
        public string Key { get; set; } = string.Empty;

        [Description("Categorie du tirage : Good, Bad ou Neutral.")]
        public ChaosCategory Category { get; set; } = ChaosCategory.Neutral;

        [Description("Poids relatif a l'interieur de sa categorie. 0 = jamais tire.")]
        public int Weight { get; set; } = 10;
    }
}
