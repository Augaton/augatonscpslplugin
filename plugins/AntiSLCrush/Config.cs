// Copyright (c) 2026 ui_2506.
// Modified by Augaton.
// Licensed under the ui_2506 Non-Commercial License.
// See LICENSE.md in this folder for full license terms.

using System.ComponentModel;

namespace AntiSLCrush
{
    public sealed class Config
    {
        [Description("Active les logs de debug.")]
        public bool Debug { get; set; } = false;

        [Description("Deconnecte les paquets venant d'une connexion inconnue du serveur.")]
        public bool ProtectTransportData { get; set; } = true;

        [Description("Retire les objets detruits de la liste reseau avant l'arrivee d'un joueur.")]
        public bool ProtectPlayerJoin { get; set; } = true;

        [Description("Stoppe l'erreur en boucle causee par une arme dont le proprietaire est detruit.")]
        public bool ProtectReserveAmmo { get; set; } = true;

        [Description("Ignore un second ajout de sac SCP-330 pour le meme joueur dans la meme frame.")]
        public bool ProtectScp330Bag { get; set; } = true;

        [Description("Remet a zero la visee du serveur quand un joueur rengaine son arme.")]
        public bool ProtectAdsHolster { get; set; } = true;

        [Description("Retire regulierement les objets detruits suivis par chaque joueur.")]
        public bool CleanObservers { get; set; } = true;
    }
}
