using System.Collections.Generic;
using System.Text.Json;
using AugatonLib.Text;
using Xunit;

namespace AugatonLib.Tests
{
    public class DiscordMessageTests
    {
        private static string ContentOf(string payload)
        {
            using JsonDocument document = JsonDocument.Parse(payload);
            return document.RootElement.GetProperty("content").GetString();
        }

        [Theory]
        [InlineData("https://discord.com/api/webhooks/123/abc", true)]
        [InlineData("https://discordapp.com/api/webhooks/123/abc", true)]
        [InlineData("http://discord.com/api/webhooks/123/abc", false)]
        [InlineData("https://discord.com.evil.example/api/webhooks/123/abc", false)]
        [InlineData("https://evil.example/api/webhooks/123/abc", false)]
        [InlineData("https://discord.com/channels/123", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void OnlyDiscordWebhooksAreAccepted(string url, bool expected)
        {
            Assert.Equal(expected, DiscordMessage.IsWebhookUrl(url));
        }

        [Fact]
        public void HostileTextStaysValidJson()
        {
            string payload = DiscordMessage.BuildPayload(new[] { "a\"b\\c\nd\te\u0001f" });

            Assert.Contains("a\"b\\c\nd\te\u0001f", ContentOf(payload));
        }

        [Fact]
        public void BackticksCannotCloseTheCodeBlock()
        {
            string content = ContentOf(DiscordMessage.BuildPayload(new[] { "```@everyone" }));

            Assert.DoesNotContain("```@", content);
            Assert.StartsWith("```", content);
            Assert.EndsWith("```", content);
        }

        [Fact]
        public void MentionsAreDisabled()
        {
            using JsonDocument document = JsonDocument.Parse(DiscordMessage.BuildPayload(new[] { "@everyone" }));

            Assert.Equal(0, document.RootElement.GetProperty("allowed_mentions").GetProperty("parse").GetArrayLength());
        }

        [Fact]
        public void ABatchStopsBeforeTheLimit()
        {
            Queue<string> pending = new Queue<string>(new[] { "aaaa", "bbbb", "cccc" });

            List<string> batch = DiscordMessage.TakeBatch(pending, 10);

            Assert.Equal(new[] { "aaaa", "bbbb" }, batch);
            Assert.Single(pending);
        }

        [Fact]
        public void ALineTooLongIsCutRatherThanStuckForever()
        {
            Queue<string> pending = new Queue<string>(new[] { new string('a', 50), "suite" });

            List<string> batch = DiscordMessage.TakeBatch(pending, 10);

            Assert.Equal(new string('a', 9), Assert.Single(batch));
            Assert.Equal("suite", pending.Peek());
        }

        [Fact]
        public void AFullBatchFitsDiscordLimit()
        {
            Queue<string> pending = new Queue<string>();

            for (int i = 0; i < 500; i++)
                pending.Enqueue(new string('"', 20));

            string content = ContentOf(DiscordMessage.BuildPayload(DiscordMessage.TakeBatch(pending, DiscordMessage.MaxContentLength)));

            Assert.True(content.Length <= 2000);
        }
    }
}
