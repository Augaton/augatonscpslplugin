using System;
using System.Collections.Generic;
using System.Text;
using AugatonLib.Bus;
using AugatonLib.Caching;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.Voting;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;

namespace SpectatorVote
{
    public sealed class VoteService
    {
        private const string IncidentPlugin = "SiteIncidents";
        private const int MinChoices = 2;

        private readonly Plugin plugin;
        private readonly BallotBox box = new BallotBox();
        private readonly List<IncidentOption> current = new List<IncidentOption>(4);
        private readonly List<IncidentOption> pool = new List<IncidentOption>(8);
        private readonly List<Player> spectators = new List<Player>(16);

        private CoroutineHandle loop;
        private CoroutineHandle closing;
        private bool running;
        private bool closingScheduled;

        public VoteService(Plugin plugin) => this.plugin = plugin;

        public CooldownTracker Cooldowns { get; } = new CooldownTracker();

        public bool IsOpen => box.IsOpen;

        private Config Config => plugin.Config;

        private Translation Text => plugin.Translation;

        public void Start()
        {
            Stop();

            running = true;
            loop = Timing.RunCoroutine(Loop());
        }

        public void Stop()
        {
            if (running)
                Timing.KillCoroutines(loop);

            running = false;
            CancelClosing();
            box.Close();
            current.Clear();
            Cooldowns.Clear();
        }

        public void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                string userId = ev?.Player?.UserId;

                box.Retract(userId);
                Cooldowns.Remove(userId);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        // Le staff peut ouvrir un vote sans le nombre de spectateurs requis, pour tester seul.
        public string TryOpen(bool forced)
        {
            if (box.IsOpen)
                return "Un vote est deja ouvert.";

            if (!Round.IsRoundInProgress)
                return "Aucun round en cours.";

            if (Warhead.IsDetonated)
                return "Plus aucun incident possible apres l'ogive.";

            if (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses)
                return "Suspendu pendant un mode de jeu.";

            if (!PluginDirectory.IsRegistered(IncidentPlugin))
                return $"{IncidentPlugin} n'est pas charge.";

            CollectSpectators();

            if (!forced && spectators.Count < Config.MinSpectators)
                return "Pas assez de spectateurs.";

            DrawOptions();

            if (current.Count < MinChoices)
                return "Pas assez d'incidents disponibles.";

            box.Open(current.Count);

            closingScheduled = true;
            closing = Timing.CallDelayed(Config.VoteDurationSeconds, Close);

            string text = Text.VoteOpened
                .Replace("%SECONDS%", Config.VoteDurationSeconds.ToString("0"))
                .Replace("%OPTIONS%", DescribeOptions());

            foreach (Player player in spectators)
                plugin.Hints.Show(player, text, Config.VoteDurationSeconds);

            return null;
        }

        public string DescribeOptions()
        {
            StringBuilder builder = new StringBuilder(128);

            for (int i = 0; i < current.Count; i++)
            {
                if (i > 0)
                    builder.Append('\n');

                builder.Append(Text.OptionLine.Replace("%NUMBER%", (i + 1).ToString()).Replace("%LABEL%", current[i].Label));
            }

            return builder.ToString();
        }

        public bool Cast(Player player, int choice, out string label)
        {
            label = null;

            if (!box.Cast(player.UserId, choice - 1))
                return false;

            label = current[choice - 1].Label;
            return true;
        }

        private IEnumerator<float> Loop()
        {
            yield return Timing.WaitForSeconds(Config.FirstVoteSeconds);

            while (running)
            {
                try
                {
                    TryOpen(false);
                }
                catch (Exception e)
                {
                    Logger.Error($"VoteService.Tick: {e}");
                }

                yield return Timing.WaitForSeconds(Config.VoteIntervalSeconds);
            }
        }

        private void CollectSpectators()
        {
            spectators.Clear();

            foreach (Player player in Player.ReadyList)
            {
                if (!player.IsDummy && player.Role == RoleTypeId.Spectator)
                    spectators.Add(player);
            }
        }

        private void DrawOptions()
        {
            current.Clear();
            pool.Clear();

            foreach (IncidentOption option in Config.Options)
            {
                if (option is not null && option.Weight > 0 && !string.IsNullOrEmpty(option.Incident))
                    pool.Add(option);
            }

            while (current.Count < Config.OptionCount && pool.Count > 0)
            {
                IncidentOption picked = Chance.PickWeighted(pool, option => option.Weight);

                pool.Remove(picked);
                current.Add(picked);
            }
        }

        private void Close()
        {
            closingScheduled = false;

            try
            {
                List<int> leaders = box.Leaders();
                IncidentOption winner = leaders.Count == 0 ? null : current[Chance.Pick(leaders)];

                box.Close();

                if (winner is null || (Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses))
                    return;

                if (PluginBus.Publish(BusTopics.IncidentRequested, Plugin.Owner, winner.Incident) == 0)
                {
                    Logger.Warn("Personne n'a recu l'incident choisi par les spectateurs.");
                    return;
                }

                plugin.Hints.ShowAll(Text.VoteResult.Replace("%LABEL%", winner.Label), Config.HintDuration);
            }
            catch (Exception e)
            {
                Logger.Error($"VoteService.Close: {e}");
            }
        }

        private void CancelClosing()
        {
            if (closingScheduled)
                Timing.KillCoroutines(closing);

            closingScheduled = false;
        }
    }
}
