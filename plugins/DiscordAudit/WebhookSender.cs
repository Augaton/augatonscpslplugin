using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AugatonLib.Bus;
using AugatonLib.Text;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace DiscordAudit
{
    // L'envoi part d'une tache du pool de threads, et seule la boucle du thread principal lit son resultat :
    // un Discord lent ne peut pas bloquer le tick, et rien ne touche au jeu depuis un autre thread.
    // L'adresse du webhook est un secret : elle n'apparait dans aucun log, pas plus que le texte des exceptions.
    public sealed class WebhookSender
    {
        private const int MaxAttempts = 3;
        private const double DefaultRetrySeconds = 5d;

        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(10d) };

        private readonly Plugin plugin;
        private readonly Queue<string> pending = new Queue<string>();

        private Task<HttpResponseMessage> inFlight;
        private CoroutineHandle loop;
        private bool running;
        private string payload;
        private int attempts;
        private float retryAt;

        public WebhookSender(Plugin plugin) => this.plugin = plugin;

        public int QueuedLines => pending.Count;

        public string LastFailure { get; private set; }

        public bool IsConfigured => DiscordMessage.IsWebhookUrl(plugin.Config.WebhookUrl);

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

        public void OnStaffAction(BusMessage message)
        {
            if (!IsConfigured || !message.TryGet(out StaffAction action))
                return;

            if (pending.Count >= plugin.Config.MaxQueuedLines)
                pending.Dequeue();

            pending.Enqueue(action.ToString());
        }

        private IEnumerator<float> Loop()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(plugin.Config.SendIntervalSeconds);

                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"WebhookSender: {e.GetType().Name}");
                }
            }
        }

        private void Tick()
        {
            if (inFlight is not null)
            {
                if (!inFlight.IsCompleted)
                    return;

                Conclude(inFlight);
                inFlight = null;
            }

            if (!IsConfigured || Time.realtimeSinceStartup < retryAt)
                return;

            if (payload is null)
            {
                if (pending.Count == 0)
                    return;

                payload = DiscordMessage.BuildPayload(DiscordMessage.TakeBatch(pending, DiscordMessage.MaxContentLength));
                attempts = 0;
            }

            string url = plugin.Config.WebhookUrl;
            string body = payload;

            inFlight = Task.Run(() => Client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json")));
        }

        private void Conclude(Task<HttpResponseMessage> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Fail(task.Exception?.GetBaseException().GetType().Name ?? "delai depasse", true);
                return;
            }

            using (HttpResponseMessage response = task.Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    payload = null;
                    LastFailure = null;
                    return;
                }

                int status = (int)response.StatusCode;

                if (status == 429)
                {
                    double wait = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? DefaultRetrySeconds;
                    retryAt = Time.realtimeSinceStartup + (float)wait;
                    return;
                }

                Fail($"HTTP {status}", status >= 500);
            }
        }

        private void Fail(string reason, bool mayRetry)
        {
            attempts++;

            if (reason != LastFailure)
                Logger.Warn($"Envoi vers Discord en echec : {reason}.");

            LastFailure = reason;

            if (!mayRetry || attempts >= MaxAttempts)
                payload = null;
        }
    }
}
