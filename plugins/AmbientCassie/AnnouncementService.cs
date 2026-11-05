using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public string CheckCassieWords()
        {
            string unknown = CassieLines.FindUnknownWord(plugin.Translation.Lines.Where(line => line is not null).Select(line => line.Cassie));

            return unknown is null ? null : $"mot inconnu : {unknown}";
        }

        public bool Announce()
        {
            AmbientLine line = Pick();

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
            if (!Round.IsRoundInProgress || Player.Count < Config.MinPlayers || Announcer.IsSpeaking)
                return;

            if (Warhead.IsDetonationInProgress || Warhead.IsDetonated)
                return;

            if (Config.PauseDuringGamemodes && WorldEvents.IsPaused)
                return;

            Announce();
        }

        private AmbientLine Pick()
        {
            AmbientPhase phase = Round.Duration.TotalSeconds >= Config.LateRoundSeconds ? AmbientPhase.Late : AmbientPhase.Early;

            eligible.Clear();

            foreach (AmbientLine line in plugin.Translation.Lines)
            {
                if (line is null || string.IsNullOrEmpty(line.Cassie) || line.Weight <= 0)
                    continue;

                if (line.Phase == AmbientPhase.Any || line.Phase == phase)
                    eligible.Add(line);
            }

            return Chance.PickWeighted(eligible, line => line.Weight);
        }
    }
}
