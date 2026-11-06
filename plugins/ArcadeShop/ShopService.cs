using System;
using System.Collections.Generic;
using System.Text;
using AugatonLib.Arcade;
using AugatonLib.Bus;
using AugatonLib.Caching;
using AugatonLib.Runtime;
using AugatonLib.World;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace ArcadeShop
{
    public sealed class ShopService
    {
        private const int MaxKeyLength = 32;

        private readonly Plugin plugin;
        private readonly CooldownTracker cooldowns = new CooldownTracker();
        private readonly HashSet<Generator> paidGenerators = new HashSet<Generator>();

        public ShopService(Plugin plugin) => this.plugin = plugin;

        public Wallet Wallet => plugin.Wallet;

        private Config Config => plugin.Config;

        private Translation Text => plugin.Translation;

        private bool IsOpen => Round.IsRoundInProgress && !(Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses);

        public void OnDeath(PlayerDeathEventArgs ev)
        {
            try
            {
                Player killer = ev?.Attacker;

                if (killer is null || ev.Player is null || killer == ev.Player || !IsOpen)
                    return;

                if (!killer.IsHuman || killer.IsTutorial)
                    return;

                if (ev.OldRole == RoleTypeId.Tutorial || ev.OldRole.GetFaction() == killer.Faction)
                    return;

                Award(killer, ev.OldRole.GetTeam() == Team.SCPs ? Config.PointsPerScpKill : Config.PointsPerKill);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeath: {e}");
            }
        }

        // ActivatedGenerator repart a chaque remise en marche : sans cette garde, allumer et couper le meme
        // generateur rapporterait des points sans fin.
        public void OnActivatedGenerator(PlayerActivatedGeneratorEventArgs ev)
        {
            try
            {
                if (ev?.Player is null || ev.Generator is null || !IsOpen || !paidGenerators.Add(ev.Generator))
                    return;

                Award(ev.Player, Config.PointsPerGenerator);
            }
            catch (Exception e)
            {
                Logger.Error($"OnActivatedGenerator: {e}");
            }
        }

        public void OnEscaped(PlayerEscapedEventArgs ev)
        {
            try
            {
                if (ev?.Player is not null && IsOpen)
                    Award(ev.Player, Config.PointsPerEscape);
            }
            catch (Exception e)
            {
                Logger.Error($"OnEscaped: {e}");
            }
        }

        public void OnContractCompleted(BusMessage message)
        {
            if (message.TryGet(out Player player) && IsOpen)
                Award(player, Config.PointsPerContract);
        }

        public void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                string userId = ev?.Player?.UserId;

                Wallet.Forget(userId);
                cooldowns.Remove(userId);
            }
            catch (Exception e)
            {
                Logger.Error($"OnLeft: {e}");
            }
        }

        public void Reset()
        {
            Wallet.Clear();
            cooldowns.Clear();
            paidGenerators.Clear();
        }

        public string Describe(Player player)
        {
            StringBuilder builder = new StringBuilder(512);

            builder.AppendLine(Text.CatalogHeader.Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString()));

            foreach (ShopItem item in Config.Items)
            {
                builder.Append(Text.CatalogLine
                    .Replace("%KEY%", item.Key)
                    .Replace("%LABEL%", item.Label)
                    .Replace("%PRICE%", item.Price.ToString()));

                if (item.Limit > 0)
                    builder.Append(Text.CatalogLimit.Replace("%LEFT%", Math.Max(0, item.Limit - Wallet.BoughtCount(player.UserId, item.Key)).ToString()));

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public bool TryBuy(Player player, string key, out string response)
        {
            if (!IsOpen)
            {
                response = Text.Closed;
                return false;
            }

            if (!player.IsAlive || !player.IsHuman || player.IsTutorial || player.IsDisarmed)
            {
                response = Text.NotEligible;
                return false;
            }

            if (cooldowns.IsOnCooldown(player.UserId, Config.BuyCooldownSeconds, out _))
            {
                response = Text.TooFast;
                return false;
            }

            cooldowns.Register(player.UserId);

            ShopItem item = Find(key);

            if (item is null)
            {
                response = Text.UnknownItem;
                return false;
            }

            switch (Wallet.TryBuy(player.UserId, item.Key, item.Price, item.Limit))
            {
                case PurchaseResult.Done:
                    ItemGrant.Give(player, item.Item, item.Quantity);
                    response = Text.Bought
                        .Replace("%LABEL%", item.Label)
                        .Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString());
                    return true;

                case PurchaseResult.LimitReached:
                    response = Text.LimitReached;
                    return false;

                default:
                    response = Text.TooExpensive
                        .Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString())
                        .Replace("%PRICE%", item.Price.ToString());
                    return false;
            }
        }

        private ShopItem Find(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length > MaxKeyLength)
                return null;

            foreach (ShopItem item in Config.Items)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }

        private void Award(Player player, int amount)
        {
            int gained = Wallet.Earn(player.UserId, amount);

            if (gained <= 0)
                return;

            plugin.Hints.Show(
                player,
                Text.Earned.Replace("%AMOUNT%", gained.ToString()).Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString()),
                Config.HintDuration);
        }
    }
}
