using System;
using AugatonLib.Bus;
using Xunit;

namespace AugatonLib.Tests
{
    public class PluginBusTests : IDisposable
    {
        public PluginBusTests() => PluginBus.Clear();

        public void Dispose() => PluginBus.Clear();

        [Fact]
        public void DeliversOnlyToMatchingTopics()
        {
            int calls = 0;
            PluginBus.Subscribe("a", "round.reset", _ => calls++);
            PluginBus.Subscribe("b", "coin.flipped", _ => calls += 10);

            Assert.Equal(1, PluginBus.Publish("ROUND.RESET", "test"));
            Assert.Equal(1, calls);
        }

        [Fact]
        public void HandlerReceivesSourceAndPayload()
        {
            BusMessage received = null;
            PluginBus.Subscribe("a", "topic", message => received = message);

            PluginBus.Publish("topic", "source", 42);

            Assert.Equal("source", received.Source);
            Assert.True(received.TryGet(out int value));
            Assert.Equal(42, value);
            Assert.False(received.TryGet(out string _));
        }

        [Fact]
        public void UnsubscribeOwnerRemovesAllItsTopics()
        {
            PluginBus.Subscribe("a", "one", _ => { });
            PluginBus.Subscribe("a", "two", _ => { });
            PluginBus.Subscribe("b", "one", _ => { });

            PluginBus.UnsubscribeOwner("a");

            Assert.Equal(1, PluginBus.SubscriptionCount);
        }

        [Fact]
        public void HandlerMayUnsubscribeDuringDelivery()
        {
            PluginBus.Subscribe("a", "topic", _ => PluginBus.UnsubscribeOwner("a"));
            PluginBus.Subscribe("b", "topic", _ => { });

            Assert.Equal(2, PluginBus.Publish("topic", "test"));
        }

        [Fact]
        public void PublishWithoutSubscriberDeliversNothing()
        {
            Assert.Equal(0, PluginBus.Publish("topic", "test"));
        }

        [Fact]
        public void ARequestIsRefusedUnlessAHandlerAccepts()
        {
            PluginBus.Subscribe("a", "topic", _ => { });

            Assert.False(PluginBus.Request("topic", "test"));
        }

        [Fact]
        public void ARequestIsAcceptedByAnyHandler()
        {
            PluginBus.Subscribe("a", "topic", _ => { });
            PluginBus.Subscribe("b", "topic", message => message.Accept());

            Assert.True(PluginBus.Request("topic", "test"));
        }

        [Fact]
        public void ARequestWithoutSubscriberIsRefused()
        {
            Assert.False(PluginBus.Request("topic", "test"));
        }

        [Fact]
        public void InvalidSubscriptionsAreIgnored()
        {
            PluginBus.Subscribe(null, "topic", _ => { });
            PluginBus.Subscribe("a", null, _ => { });
            PluginBus.Subscribe("a", "topic", null);

            Assert.Equal(0, PluginBus.SubscriptionCount);
        }
    }
}
