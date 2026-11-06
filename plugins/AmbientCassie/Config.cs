using System.ComponentModel;

namespace AmbientCassie
{
    public sealed class Config
    {
        [Description("Delai avant la premiere annonce, en secondes.")]
        public float FirstAnnouncementSeconds { get; set; } = 240f;

        [Description("Delai minimal entre deux annonces, 60 au plus bas.")]
        public float MinIntervalSeconds { get; set; } = 300f;

        [Description("Delai maximal entre deux annonces.")]
        public float MaxIntervalSeconds { get; set; } = 600f;

        [Description("Secondes de round a partir desquelles les annonces Late remplacent les annonces Early.")]
        public float LateRoundSeconds { get; set; } = 600f;

        [Description("Niveau du Directeur du chaos qui fait aussi passer aux annonces Late. 0 = jamais.")]
        public int ChaosLevelForLate { get; set; } = 2;

        [Description("Secondes minimales entre deux annonces de reaction, Mutator et ScpLevel, 30 au plus bas.")]
        public float ReactionCooldownSeconds { get; set; } = 120f;

        [Description("Joueurs requis pour une annonce.")]
        public int MinPlayers { get; set; } = 4;

        [Description("Suspend les annonces pendant un mode arene de RandomGamemode.")]
        public bool PauseDuringGamemodes { get; set; } = true;
    }
}
