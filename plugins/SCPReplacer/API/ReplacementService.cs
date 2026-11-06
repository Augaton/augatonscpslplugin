using System;
using System.Collections.Generic;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;

namespace SCPReplacer.API
{
    public sealed class ReplacementService
    {
        private readonly Plugin plugin;
        private readonly List<ScpToReplace> pending = new List<ScpToReplace>(8);
        private readonly List<Player> candidateBuffer = new List<Player>(16);

        private int replacementsThisRound;
        private CoroutineHandle holdTimeout;
        private bool extinctionHeld;

        public ReplacementService(Plugin plugin) => this.plugin = plugin;

        public IReadOnlyList<ScpToReplace> Pending => pending;

        public bool HasReplacementCutoffPassed()
        {
            return Round.Duration.TotalSeconds > plugin.Config.ReplaceCutoff;
        }

        public bool IsReplacementLimitReached()
        {
            return plugin.Config.MaxReplacementsPerRound > 0
                && replacementsThisRound >= plugin.Config.MaxReplacementsPerRound;
        }

        public ScpToReplace Find(string scpNumber)
        {
            foreach (ScpToReplace entry in pending)
            {
                if (entry.Name == scpNumber)
                    return entry;
            }

            return null;
        }

        public string DescribePending()
        {
            if (pending.Count == 0)
                return string.Empty;

            string[] names = new string[pending.Count];

            for (int i = 0; i < pending.Count; i++)
                names[i] = "SCP-" + pending[i].Name;

            return string.Join(", ", names);
        }

        public void Reset()
        {
            foreach (ScpToReplace entry in pending)
            {
                if (entry.LotteryRunning)
                    Timing.KillCoroutines(entry.LotteryTimeout);
            }

            pending.Clear();
            replacementsThisRound = 0;

            if (extinctionHeld)
            {
                extinctionHeld = false;
                Timing.KillCoroutines(holdTimeout);
                GenocideArbiter.Release(Plugin.DepartureOwner, Team.SCPs);
            }
        }

        // Remis a zero en fin de round et non dans Reset, qui repasse au debut du round, sur le meme
        // evenement que le depart du mode : selon l'ordre de chargement, le drapeau serait efface.
        public bool GamemodeActive { get; set; }

        public bool IsSuspended => GamemodeActive && plugin.Config.PauseDuringGamemodes;

        public void RemoveVolunteer(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            foreach (ScpToReplace entry in pending)
                entry.Volunteers.Remove(userId);
        }

        public bool CanClaimDeparture(Player scpPlayer)
        {
            if (scpPlayer is null || !scpPlayer.IsSCP || IsSuspended)
                return false;

            if (plugin.Config.IgnoredRoles.Contains(scpPlayer.Role))
                return false;

            if (IsReplacementLimitReached())
                return false;

            if (Round.Duration.TotalSeconds > plugin.Config.QuitCutoff)
                return false;

            return scpPlayer.Health >= plugin.Config.RequiredHealthPercentage / 100f * scpPlayer.MaxHealth
                && !string.IsNullOrEmpty(scpPlayer.Role.ScpNumber());
        }

        public bool TryRegisterDeparture(Player scpPlayer, out string reason)
        {
            reason = null;

            if (scpPlayer is null || !scpPlayer.IsSCP)
                return false;

            RoleTypeId role = scpPlayer.Role;

            if (plugin.Config.IgnoredRoles.Contains(role))
                return false;

            if (IsReplacementLimitReached())
            {
                reason = plugin.Translation.HumanCommandLimitReached;
                return false;
            }

            double elapsedSeconds = Round.Duration.TotalSeconds;
            float requiredHealth = plugin.Config.RequiredHealthPercentage / 100f * scpPlayer.MaxHealth;
            string scpNumber = role.ScpNumber();

            if (string.IsNullOrEmpty(scpNumber))
                return false;

            if (elapsedSeconds > plugin.Config.QuitCutoff)
            {
                reason = plugin.Translation.HumanCommandTooLate;
                return false;
            }

            if (scpPlayer.Health < requiredHealth)
            {
                reason = plugin.Translation.HumanCommandTooLowHealth;
                return false;
            }

            if (Find(scpNumber) is not null)
                return false;

            pending.Add(new ScpToReplace
            {
                Name = scpNumber,
                Role = role,
                Size = ScaleArbiter.BaselineOf(scpPlayer),
            });

            Logger.Info($"SCP-{scpNumber} ({scpPlayer.Nickname}) est remplacable apres {elapsedSeconds:0}s de round.");

            Announce(scpNumber);
            HoldExtinctionAnnouncement();
            return true;
        }

        // Le dernier SCP qui part n'est pas une extinction tant qu'un volontaire peut le reprendre.
        private void HoldExtinctionAnnouncement()
        {
            float remaining = UnityEngine.Mathf.Max(0f, plugin.Config.ReplaceCutoff - (float)Round.Duration.TotalSeconds);
            float window = remaining + plugin.Config.LotteryPeriodSeconds + 1f;

            if (extinctionHeld)
                Timing.KillCoroutines(holdTimeout);

            extinctionHeld = true;
            GenocideArbiter.Hold(Plugin.DepartureOwner, Team.SCPs, window);
            holdTimeout = Timing.CallDelayed(window, ReleaseExtinctionAnnouncement);
        }

        private void ReleaseExtinctionAnnouncement()
        {
            if (!extinctionHeld)
                return;

            extinctionHeld = false;
            Timing.KillCoroutines(holdTimeout);
            GenocideArbiter.Release(Plugin.DepartureOwner, Team.SCPs);
            PluginBus.Publish(BusTopics.TeamCheckRequested, Plugin.DepartureOwner, Team.SCPs);
        }

        public bool TryVolunteer(Player player, ScpToReplace entry, out string error)
        {
            error = null;

            if (entry.Volunteers.Contains(player.UserId))
            {
                error = plugin.Translation.AlreadyVolunteeredMessage;
                return false;
            }

            entry.Volunteers.Add(player.UserId);

            if (entry.LotteryRunning)
                return true;

            entry.LotteryRunning = true;
            entry.LotteryTimeout = Timing.CallDelayed(plugin.Config.LotteryPeriodSeconds, () => RunLottery(entry));
            return true;
        }

        private void RunLottery(ScpToReplace entry)
        {
            try
            {
                entry.LotteryRunning = false;

                if (!pending.Contains(entry))
                    return;

                Player winner = IsSuspended ? null : PickWinner(entry);
                pending.Remove(entry);

                if (winner is null)
                {
                    Logger.Info($"Loterie SCP-{entry.Name} annulee : aucun volontaire valide.");
                }
                else
                {
                    ApplyRole(winner, entry);
                    replacementsThisRound++;
                }

                if (pending.Count == 0)
                    ReleaseExtinctionAnnouncement();
            }
            catch (Exception e)
            {
                Logger.Error($"RunLottery: {e}");
            }
        }

        private Player PickWinner(ScpToReplace entry)
        {
            candidateBuffer.Clear();

            foreach (string userId in entry.Volunteers)
            {
                Player candidate = Player.Get(userId);

                if (candidate is null || candidate.IsDestroyed || candidate.IsSCP)
                    continue;

                if (!plugin.Config.AllowAliveVolunteers && candidate.Role != RoleTypeId.Spectator)
                    continue;

                candidateBuffer.Add(candidate);
            }

            if (candidateBuffer.Count == 0)
                return null;

            return candidateBuffer[UnityEngine.Random.Range(0, candidateBuffer.Count)];
        }

        private void ApplyRole(Player winner, ScpToReplace entry)
        {
            winner.DisableAllEffects();
            winner.SetRole(entry.Role, RoleChangeReason.LateJoin, RoleSpawnFlags.All);

            PluginBus.Publish(BusTopics.PlayerReplaced, Plugin.DepartureOwner, new PlayerReplacement(winner, entry.Size));

            foreach (Player player in Player.ReadyList)
            {
                string message = player == winner
                    ? plugin.Translation.ChangedSuccessfullySelfBroadcast
                    : plugin.Translation.ChangedSuccessfullyEveryoneBroadcast;

                player.SendBroadcast(
                    plugin.Translation.BroadcastHeader + message.Replace("%NUMBER%", entry.Name),
                    plugin.Config.ResultBroadcastDuration);
            }

            Logger.Info($"{winner.Nickname} ({winner.UserId}) a remplace SCP-{entry.Name}.");
        }

        private void Announce(string scpNumber)
        {
            string message = plugin.Translation.ReplaceBroadcast.Replace("%NUMBER%", scpNumber);

            foreach (Player player in Player.ReadyList)
            {
                if (player.IsSCP)
                    continue;

                player.SendBroadcast(
                    plugin.Translation.BroadcastHeader + message,
                    plugin.Config.AnnounceBroadcastDuration,
                    Broadcast.BroadcastFlags.Normal,
                    true);

                if (plugin.Config.AnnounceInConsole)
                    player.SendConsoleMessage(message, "yellow");
            }
        }
    }
}
