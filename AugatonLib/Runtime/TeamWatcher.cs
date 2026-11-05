using System;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// Detecte l'extinction d'une equipe et publie BusTopics.TeamWiped, pour que SupplyDrop et
    /// TeamGenocide en profitent chacun sans dependre de l'autre.
    /// </summary>
    internal static class TeamWatcher
    {
        private const string Source = "AugatonLib";

        // L'attribution des roles au debut du round ne doit pas passer pour une extinction.
        private const float ActivationSeconds = 5f;

        private static bool hooked;

        internal static void Hook()
        {
            if (hooked)
                return;

            hooked = true;
            PlayerEvents.ChangingRole += OnChangingRole;
            PluginBus.Subscribe(Source, BusTopics.TeamCheckRequested, OnCheckRequested);
        }

        private static void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || ev.ChangeReason != RoleChangeReason.Died)
                    return;

                Team leaving = ev.OldRole.Team;

                if (leaving != Team.Dead && ev.NewRole.GetTeam() != leaving)
                    ReportIfWiped(leaving, ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"TeamWatcher: {e}");
            }
        }

        private static void OnCheckRequested(BusMessage message)
        {
            if (message.TryGet(out Team team))
                ReportIfWiped(team, null);
        }

        private static void ReportIfWiped(Team team, Player excluded)
        {
            if (!Round.IsRoundInProgress || Round.Duration.TotalSeconds < ActivationSeconds)
                return;

            if (GenocideArbiter.IsSuppressed || GenocideArbiter.IsHeld(team))
                return;

            foreach (Player player in Player.ReadyList)
            {
                if (player != excluded && player.Team == team)
                    return;
            }

            PluginBus.Publish(BusTopics.TeamWiped, Source, team);
        }
    }
}
