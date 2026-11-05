using System.Collections.Generic;
using System.ComponentModel;

namespace SCP500s.API
{
    public class EffectPayload
    {
        [Description("Message affiche au joueur.")]
        public string Message { get; set; } = string.Empty;

        [Description("Comportement code applique en plus des donnees. None pour une pilule purement declarative.")]
        public PillBehaviour Behaviour { get; set; } = PillBehaviour.None;

        [Description("Effets appliques. Une duree de 0 rend l'effet actif jusqu'a la mort.")]
        public List<EffectEntry> Effects { get; set; } = new List<EffectEntry>();

        [Description("Objets donnes.")]
        public List<ItemType> GrantedItems { get; set; } = new List<ItemType>();

        [Description("Si true, un seul objet est tire au hasard dans granted_items au lieu de tous les donner.")]
        public bool GrantOneRandomItem { get; set; } = false;

        [Description("Points de vie rendus immediatement.")]
        public float Heal { get; set; } = 0f;

        [Description("Points de vie artificiels (AHP) accordes, qui s'erodent avec le temps. 0 = aucun.")]
        public float ArtificialHealth { get; set; } = 0f;

        [Description("Points de vie regeneres chaque seconde. 0 = aucune regeneration.")]
        public float RegenerationPerSecond { get; set; } = 0f;

        [Description("Duree de la regeneration en secondes, bornee par max_effect_duration.")]
        public float RegenerationSeconds { get; set; } = 0f;

        [Description("Vie maximale minimale garantie. 0 = inchangee.")]
        public float MinimumMaxHealth { get; set; } = 0f;

        [Description("Retire les effets negatifs en cours avant d'appliquer les siens. Les effets positifs sont conserves.")]
        public bool CureNegativeEffects { get; set; } = false;

        [Description("Fait lacher au joueur tout son inventaire.")]
        public bool DropInventory { get; set; } = false;

        [Description("Points de degats infliges au joueur.")]
        public float Damage { get; set; } = 0f;
    }
}
