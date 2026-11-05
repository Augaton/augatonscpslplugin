using System.ComponentModel;
using Cassie;
using LabApi.Features.Wrappers;
using MEC;

namespace TeamGenocide.API
{
    public sealed class Announcement
    {
        [Description("Annonce C.A.S.S.I.E. Vide = desactivee.")]
        public string Cassie { get; set; } = string.Empty;

        [Description("Sous-titres C.A.S.S.I.E. Vide = pas de sous-titres.")]
        public string Subtitle { get; set; } = string.Empty;

        [Description("Broadcast diffuse a tous. Vide = desactive.")]
        public string Broadcast { get; set; } = string.Empty;

        [Description("Hint affiche a tous. Vide = desactive.")]
        public string Hint { get; set; } = string.Empty;

        [Description("Duree d'affichage en secondes.")]
        public ushort DisplayTime { get; set; } = 10;

        [Description("Effet lumineux accompagnant l'annonce. Laisser vide pour desactiver.")]
        public LightEffect Lights { get; set; }

        public CoroutineHandle Announce()
        {
            if (!string.IsNullOrEmpty(Cassie))
            {
                CassieTtsPayload payload = string.IsNullOrEmpty(Subtitle)
                    ? new CassieTtsPayload(Cassie, false, false)
                    : new CassieTtsPayload(Cassie, Subtitle, false);

                Announcer.Message(payload, 0f, 0f);
            }

            if (!string.IsNullOrEmpty(Broadcast))
                Server.SendBroadcast(Broadcast, DisplayTime);

            if (!string.IsNullOrEmpty(Hint))
            {
                foreach (Player player in Player.ReadyList)
                    Plugin.Instance.Hints.Show(player, Hint, DisplayTime);
            }

            return Lights is null ? default : Lights.Apply();
        }
    }
}
