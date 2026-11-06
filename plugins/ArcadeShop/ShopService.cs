using System;
using System.Collections.Generic;
using System.Text;
using AugatonLib.Arcade;
using AugatonLib.Bus;
using AugatonLib.Caching;
using AugatonLib.Runtime;
using AugatonLib.World;
using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace ArcadeShop
{
    public sealed class ShopService
    {
        private const int MaxKeyLength = 32;
        private const string ContractsPlugin = "Contracts";

        private readonly Plugin plugin;
        private readonly CooldownTracker cooldowns = new CooldownTracker();
        private readonly CooldownTracker catalogCooldowns = new CooldownTracker();
        private readonly HashSet<Generator> paidGenerators = new HashSet<Generator>();
        private readonly Dictionary<string, int> carriedOver = new Dictionary<string, int>();

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

                // SCP-049 peut relever des corps : un zombie paie comme un kill normal, sinon deux complices en vivraient.
                bool isFullScp = ev.OldRole.GetTeam() == Team.SCPs && ev.OldRole != RoleTypeId.Scp0492;

                Award(killer, isFullScp ? Config.PointsPerScpKill : Config.PointsPerKill);
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

        public void OnPlayerReplaced(BusMessage message)
        {
            if (message.TryGet(out PlayerReplacement replacement))
                Wallet.Transfer(replacement.ReplacedUserId, replacement.Replacement?.UserId);
        }

        public void OnContractCompleted(BusMessage message)
        {
            if (message.TryGet(out Player player) && IsOpen)
                Award(player, Config.PointsPerContract);
        }

        // La boutique est fermee pendant un mode et le solde tombe avec le round : ce gain attend le suivant.
        public void OnGamemodeWon(BusMessage message)
        {
            if (!message.TryGet(out Player player) || Config.PointsForGamemodeWin <= 0)
                return;

            carriedOver[player.UserId] = Config.PointsForGamemodeWin;
            plugin.Hints.Show(player, Text.WinnerBonus.Replace("%AMOUNT%", Config.PointsForGamemodeWin.ToString()), Config.HintDuration);
        }

        public void OnStreakReached(BusMessage message)
        {
            if (message.TryGet(out PlayerStreak streak) && streak.IsMilestone && IsOpen)
                Award(streak.Player, Config.PointsPerStreak);
        }

        // Le compte reste jusqu'a la fin du round : l'effacer ici rendrait ses limites d'achat a un joueur
        // qui quitte et revient. Reset le vide, donc il reste borne par les joueurs du round.
        public void OnLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                string userId = ev?.Player?.UserId;

                cooldowns.Remove(userId);
                catalogCooldowns.Remove(userId);
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
            catalogCooldowns.Clear();
            paidGenerators.Clear();

            foreach (KeyValuePair<string, int> gain in carriedOver)
                Wallet.Earn(gain.Key, gain.Value);

            carriedOver.Clear();
        }

        public string CheckItems()
        {
            foreach (ShopItem item in Config.Items)
            {
                if (!InventoryItemLoader.AvailableItems.ContainsKey(item.Item))
                    return $"objet inconnu : {item.Item}";
            }

            return null;
        }

        public bool TryDescribe(Player player, out string response)
        {
            if (catalogCooldowns.IsOnCooldown(player.UserId, Config.CatalogCooldownSeconds, out _))
            {
                response = Text.TooFast;
                return false;
            }

            catalogCooldowns.Register(player.UserId);

            response = Describe(player);
            return true;
        }

        private string Describe(Player player)
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

            if (Config.ContractRerollPrice > 0 && PluginDirectory.IsRegistered(ContractsPlugin))
            {
                builder.AppendLine(Text.CatalogLine
                    .Replace("%KEY%", Config.ContractRerollKey)
                    .Replace("%LABEL%", Text.ContractRerollLabel)
                    .Replace("%PRICE%", Config.ContractRerollPrice.ToString()));
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

            if (IsContractReroll(key))
                return TryRerollContract(player, out response);

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

        private bool IsContractReroll(string key)
        {
            if (string.IsNullOrEmpty(key) || Config.ContractRerollPrice <= 0)
                return false;

            return string.Equals(key, Config.ContractRerollKey, StringComparison.OrdinalIgnoreCase);
        }

        private bool TryRerollContract(Player player, out string response)
        {
            int price = Config.ContractRerollPrice;

            if (Wallet.BalanceOf(player.UserId) < price)
            {
                response = Text.TooExpensive
                    .Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString())
                    .Replace("%PRICE%", price.ToString());
                return false;
            }

            ContractRerollRequest request = new ContractRerollRequest(player);

            PluginBus.Publish(BusTopics.ContractRerollRequested, plugin.Name, request);

            if (!request.Accepted)
            {
                response = Text.ContractRerollRefused;
                return false;
            }

            Wallet.TryBuy(player.UserId, Config.ContractRerollKey, price, 0);
            response = Text.ContractRerolled.Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString());
            return true;
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
