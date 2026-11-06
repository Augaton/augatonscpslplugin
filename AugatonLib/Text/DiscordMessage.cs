using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AugatonLib.Text
{
    public static class DiscordMessage
    {
        // Discord refuse un message de plus de 2000 caracteres, bloc de code compris.
        public const int MaxContentLength = 1900;

        private const string CodeFence = "```";

        private static readonly Regex Tags = new Regex("<[^>]*>", RegexOptions.Compiled);

        private static readonly string[] Hosts =
        {
            "discord.com",
            "discordapp.com",
            "ptb.discord.com",
            "canary.discord.com",
        };

        public static bool IsWebhookUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
                && uri.Scheme == Uri.UriSchemeHttps
                && Array.IndexOf(Hosts, uri.Host.ToLowerInvariant()) >= 0
                && uri.AbsolutePath.StartsWith("/api/webhooks/", StringComparison.Ordinal);
        }

        public static string StripTags(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : Tags.Replace(text, string.Empty);
        }

        public static List<string> TakeBatch(Queue<string> pending, int maxLength)
        {
            List<string> batch = new List<string>(8);
            int length = 0;

            while (pending.Count > 0 && length < maxLength)
            {
                string line = pending.Peek() ?? string.Empty;

                if (line.Length + 1 > maxLength - length)
                {
                    if (batch.Count > 0)
                        break;

                    line = line.Substring(0, maxLength - 1);
                }

                pending.Dequeue();
                batch.Add(line);
                length += line.Length + 1;
            }

            return batch;
        }

        // Le bloc de code neutralise la mise en forme d'un pseudo. La liste vide d'allowed_mentions
        // empeche tout @everyone glisse dans un pseudo de sonner le serveur Discord.
        public static string BuildPayload(IEnumerable<string> lines)
        {
            StringBuilder builder = new StringBuilder(MaxContentLength + 64);

            builder.Append("{\"content\":\"").Append(CodeFence).Append("\\n");

            foreach (string line in lines)
            {
                AppendEscaped(builder, line.Replace('`', '\''));
                builder.Append("\\n");
            }

            builder.Append(CodeFence).Append("\",\"allowed_mentions\":{\"parse\":[]}}");
            return builder.ToString();
        }

        private static void AppendEscaped(StringBuilder builder, string text)
        {
            foreach (char character in text)
            {
                switch (character)
                {
                    case '"':
                        builder.Append("\\\"");
                        break;

                    case '\\':
                        builder.Append("\\\\");
                        break;

                    case '\n':
                        builder.Append("\\n");
                        break;

                    case '\r':
                        builder.Append("\\r");
                        break;

                    case '\t':
                        builder.Append("\\t");
                        break;

                    default:
                        if (character < ' ')
                            builder.Append("\\u").Append(((int)character).ToString("x4"));
                        else
                            builder.Append(character);

                        break;
                }
            }
        }
    }
}
