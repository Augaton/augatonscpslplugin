using System;
using System.Collections.Generic;

namespace AugatonLib.Arcade
{
    public enum PurchaseResult
    {
        Done,
        TooExpensive,
        LimitReached,
    }

    /// <summary>
    /// Points de chaque joueur pendant un round. Le solde ne devient jamais negatif et ne depasse pas
    /// <see cref="MaxBalance"/>, et un achat ne debite rien s'il est refuse.
    /// </summary>
    public sealed class Wallet
    {
        private sealed class Account
        {
            public int Balance { get; set; }

            public Dictionary<string, int> Bought { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        private readonly Dictionary<string, Account> accounts = new Dictionary<string, Account>(StringComparer.Ordinal);

        public int MaxBalance { get; set; } = int.MaxValue;

        public int BalanceOf(string player)
        {
            return Find(player)?.Balance ?? 0;
        }

        public int BoughtCount(string player, string item)
        {
            Account account = Find(player);

            return account is not null && !string.IsNullOrEmpty(item) && account.Bought.TryGetValue(item, out int count) ? count : 0;
        }

        public int Earn(string player, int amount)
        {
            if (amount <= 0 || string.IsNullOrEmpty(player))
                return 0;

            Account account = Ensure(player);
            int gained = Math.Min(amount, Math.Max(0, MaxBalance - account.Balance));

            account.Balance += gained;
            return gained;
        }

        // Une limite de zero ou moins veut dire sans limite. Le prix ne peut pas etre negatif : un prix
        // mal regle ne doit pas se transformer en credit.
        public PurchaseResult TryBuy(string player, string item, int price, int limit)
        {
            int cost = Math.Max(0, price);

            if (limit > 0 && BoughtCount(player, item) >= limit)
                return PurchaseResult.LimitReached;

            if (string.IsNullOrEmpty(player) || string.IsNullOrEmpty(item) || BalanceOf(player) < cost)
                return PurchaseResult.TooExpensive;

            Account account = Ensure(player);

            account.Balance -= cost;
            account.Bought[item] = BoughtCount(player, item) + 1;

            return PurchaseResult.Done;
        }

        public void Transfer(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || from == to)
                return;

            if (!accounts.TryGetValue(from, out Account account))
                return;

            accounts.Remove(from);

            if (accounts.ContainsKey(to))
                Earn(to, account.Balance);
            else
                accounts[to] = account;
        }

        public void Clear() => accounts.Clear();

        private Account Find(string player)
        {
            return !string.IsNullOrEmpty(player) && accounts.TryGetValue(player, out Account account) ? account : null;
        }

        private Account Ensure(string player)
        {
            if (accounts.TryGetValue(player, out Account existing))
                return existing;

            Account created = new Account();

            accounts[player] = created;
            return created;
        }
    }
}
