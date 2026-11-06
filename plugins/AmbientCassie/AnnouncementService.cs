using System;
using System.Collections.Generic;
using System.Linq;
using AugatonLib.Chaos;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.Text;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace AmbientCassie
{
    public sealed class AnnouncementService
    {
        private readonly Plugin plugin;
        private readonly List<AmbientLine> eligible = new List<AmbientLine>(8);

        private CoroutineHandle loop;
        private bool running;
        private float nextReaction;

        public AnnouncementService(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

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
            nextReaction = 0f;
        }

        public string CheckCassieWords()
        {
            string unknown = CassieLines.FindUnknownWord(plugin.Translation.Lines.Where(line => line is not null).Select(line => line.Cassie));

            return unknown is null ? null : $"mot inconnu : {unknown}";
        }

        public bool Announce() => Play(Pick(CurrentPhase(), true));

        // Une reaction cede la place comme les autres annonces, et ne revient pas a chaque niveau gagne.
        public void React(AmbientPhase phase)
        {
            try
            {
                if (!running || Time.time < nextReaction || !CanSpeak())
                    return;

                if (Play(Pick(phase, false)))
                    nextReaction = Time.time + Config.ReactionCooldownSeconds;
            }
            catch (Exception e)
            {
                Logger.Error($"AnnouncementService.React: {e}");
            }
        }

        private static bool Play(AmbientLine line)
        {
            if (line is null)
                return false;

            Announcer.Message(line.Cassie, line.Subtitles, false, 0f, 0f);
            return true;
        }

        private IEnumerator<float> Loop()
        {
            yield return Timing.WaitForSeconds(Config.FirstAnnouncementSeconds);

            while (true)
            {
                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"AnnouncementService: {e}");
                }

                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(Config.MinIntervalSeconds, Config.MaxIntervalSeconds));
            }
        }

        // Une annonce d'ambiance cede toujours la place : a un compte a rebours, a une autre annonce, a un mode.
        private void Tick()
        {
            if (CanSpeak())
                Announce();
        }

        private bool CanSpeak()
        {
            if (!Round.IsRoundInProgress || Player.Count < Config.MinPlayers || Announcer.IsSpeaking)
                return false;

            if (Warhead.IsDetonationInProgress || Warhead.IsDetonated)
                return false;

            return !(Config.PauseDuringGamemodes && WorldEvents.IsPaused);
        }

        private AmbientPhase CurrentPhase()
        {
            bool chaotic = Config.ChaosLevelForLate > 0 && ChaosState.BonusLevel >= Config.ChaosLevelForLate;

            return chaotic || Round.Duration.TotalSeconds >= Config.LateRoundSeconds ? AmbientPhase.Late : AmbientPhase.Early;
        }

        private AmbientLine Pick(AmbientPhase phase, bool withAny)
        {
            eligible.Clear();

            foreach (AmbientLine line in plugin.Translation.Lines)
            {
                if (line is null || string.IsNullOrEmpty(line.Cassie) || line.Weight <= 0)
                    continue;

                if (line.Phase == phase || (withAny && line.Phase == AmbientPhase.Any))
                    eligible.Add(line);
            }

            return Chance.PickWeighted(eligible, line => line.Weight);
        }
    }
}
