using AugatonLib.Arcade;
using Xunit;

namespace AugatonLib.Tests
{
    public class WalletTests
    {
        [Fact]
        public void AnUnknownPlayerHasNothing()
        {
            Wallet wallet = new Wallet();

            Assert.Equal(0, wallet.BalanceOf("a"));
            Assert.Equal(0, wallet.BalanceOf(null));
            Assert.Equal(0, wallet.BoughtCount("a", "soin"));
        }

        [Fact]
        public void EarningAddsUp()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 10);
            wallet.Earn("a", 5);

            Assert.Equal(15, wallet.BalanceOf("a"));
        }

        [Fact]
        public void ANegativeGainIsIgnored()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 10);

            Assert.Equal(0, wallet.Earn("a", -50));
            Assert.Equal(10, wallet.BalanceOf("a"));
        }

        [Fact]
        public void TheBalanceStopsAtItsMaximum()
        {
            Wallet wallet = new Wallet { MaxBalance = 100 };

            wallet.Earn("a", 90);

            Assert.Equal(10, wallet.Earn("a", 40));
            Assert.Equal(100, wallet.BalanceOf("a"));
            Assert.Equal(0, wallet.Earn("a", 1));
        }

        [Fact]
        public void TheDefaultMaximumCannotOverflow()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", int.MaxValue);
            wallet.Earn("a", int.MaxValue);

            Assert.Equal(int.MaxValue, wallet.BalanceOf("a"));
        }

        [Fact]
        public void BuyingDebitsAndCounts()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 50);

            Assert.Equal(PurchaseResult.Done, wallet.TryBuy("a", "soin", 30, 0));
            Assert.Equal(20, wallet.BalanceOf("a"));
            Assert.Equal(1, wallet.BoughtCount("a", "soin"));
        }

        [Fact]
        public void ARefusedPurchaseCostsNothing()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 20);

            Assert.Equal(PurchaseResult.TooExpensive, wallet.TryBuy("a", "soin", 30, 0));
            Assert.Equal(20, wallet.BalanceOf("a"));
            Assert.Equal(0, wallet.BoughtCount("a", "soin"));
        }

        [Fact]
        public void TheExactPriceIsEnough()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 30);

            Assert.Equal(PurchaseResult.Done, wallet.TryBuy("a", "soin", 30, 0));
            Assert.Equal(0, wallet.BalanceOf("a"));
        }

        [Fact]
        public void TheLimitIsPerItem()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 100);

            Assert.Equal(PurchaseResult.Done, wallet.TryBuy("a", "soin", 10, 1));
            Assert.Equal(PurchaseResult.LimitReached, wallet.TryBuy("a", "soin", 10, 1));
            Assert.Equal(90, wallet.BalanceOf("a"));
            Assert.Equal(PurchaseResult.Done, wallet.TryBuy("a", "grenade", 10, 1));
        }

        [Fact]
        public void ItemKeysIgnoreCase()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 100);
            wallet.TryBuy("a", "soin", 10, 1);

            Assert.Equal(PurchaseResult.LimitReached, wallet.TryBuy("a", "SOIN", 10, 1));
        }

        [Fact]
        public void ANegativePriceGivesNoCredit()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 10);

            Assert.Equal(PurchaseResult.Done, wallet.TryBuy("a", "soin", -500, 0));
            Assert.Equal(10, wallet.BalanceOf("a"));
        }

        [Fact]
        public void AnEmptyPlayerCannotBuy()
        {
            Wallet wallet = new Wallet();

            Assert.Equal(PurchaseResult.TooExpensive, wallet.TryBuy(null, "soin", 0, 0));
            Assert.Equal(PurchaseResult.TooExpensive, wallet.TryBuy("a", null, 0, 0));
        }

        [Fact]
        public void ClearWipesTheAccounts()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 10);
            wallet.TryBuy("a", "soin", 5, 1);

            wallet.Clear();

            Assert.Equal(0, wallet.BalanceOf("a"));
            Assert.Equal(0, wallet.BoughtCount("a", "soin"));
        }

        [Fact]
        public void ABalanceFollowsItsReplacement()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 30);
            wallet.Transfer("a", "b");

            Assert.Equal(0, wallet.BalanceOf("a"));
            Assert.Equal(30, wallet.BalanceOf("b"));
        }

        [Fact]
        public void ATransferAddsToAnExistingBalance()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 30);
            wallet.Earn("b", 5);
            wallet.Transfer("a", "b");

            Assert.Equal(35, wallet.BalanceOf("b"));
        }

        [Fact]
        public void ATransferToOneselfChangesNothing()
        {
            Wallet wallet = new Wallet();

            wallet.Earn("a", 30);
            wallet.Transfer("a", "a");

            Assert.Equal(30, wallet.BalanceOf("a"));
        }
    }
}
