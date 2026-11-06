using System;
using System.Collections.Generic;
using System.Text;
using AugatonLib.Arcade;
using AugatonLib.Bus;
using AugatonLib.Caching;
using AugatonLib.Runtime;
using AugatonLib.World;
using CustomPlayerEffects;
using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;

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
            if (message.TryGet(out Player player) && player.IsHuman && IsOpen)
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

        public void OnWarmupChampion(BusMessage message)
        {
            if (message.TryGet(out Player player))
                Award(player, Config.PointsForWarmupChampion);
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
                AppendLine(builder, player, item.Key, item.Label, item.Price, item.Limit);

            foreach (ShopPerk perk in Config.Perks)
                AppendLine(builder, player, perk.Key, perk.Label, perk.Price, perk.Limit);

            if (Config.ContractRerollPrice > 0 && PluginDirectory.IsRegistered(ContractsPlugin))
            {
                builder.AppendLine(Text.CatalogLine
                    .Replace("%KEY%", Config.ContractRerollKey)
                    .Replace("%LABEL%", Text.ContractRerollLabel)
                    .Replace("%PRICE%", Config.ContractRerollPrice.ToString()));
            }

            return builder.ToString();
        }

        private void AppendLine(StringBuilder builder, Player player, string key, string label, int price, int limit)
        {
            builder.Append(Text.CatalogLine
                .Replace("%KEY%", key)
                .Replace("%LABEL%", label)
                .Replace("%PRICE%", price.ToString()));

            if (limit > 0)
                builder.Append(Text.CatalogLimit.Replace("%LEFT%", Math.Max(0, limit - Wallet.BoughtCount(player.UserId, key)).ToString()));

            builder.AppendLine();
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

            ShopPerk perk = FindPerk(key);

            if (perk is not null)
                return TryBuyPerk(player, perk, out response);

            ShopItem item = Find(key);

            if (item is null)
            {
                response = Text.UnknownItem;
                return false;
            }

            if (!TryPay(player, item.Key, item.Label, item.Price, item.Limit, out response))
                return false;

            ItemGrant.Give(player, item.Item, item.Quantity);
            return true;
        }

        private bool TryBuyPerk(Player player, ShopPerk perk, out string response)
        {
            if (perk.Kind == PerkKind.Heal && player.Health >= player.MaxHealth)
            {
                response = Text.AlreadyHealthy;
                return false;
            }

            if (!TryPay(player, perk.Key, perk.Label, perk.Price, perk.Limit, out response))
                return false;

            switch (perk.Kind)
            {
                case PerkKind.Heal:
                    player.Heal(perk.Amount);
                    break;

                case PerkKind.Shield:
                    player.CreateAhpProcess(perk.Amount, perk.Amount, AhpStat.DefaultDecay, AhpStat.DefaultEfficacy, 0f, false);
                    break;

                case PerkKind.Speed:
                    player.EnableEffect<MovementBoost>((byte)Math.Min(perk.Amount, 100f), perk.Seconds);
                    break;
            }

            return true;
        }

        private bool TryPay(Player player, string key, string label, int price, int limit, out string response)
        {
            switch (Wallet.TryBuy(player.UserId, key, price, limit))
            {
                case PurchaseResult.Done:
                    response = Text.Bought
                        .Replace("%LABEL%", label)
                        .Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString());
                    return true;

                case PurchaseResult.LimitReached:
                    response = Text.LimitReached;
                    return false;

                default:
                    response = Text.TooExpensive
                        .Replace("%BALANCE%", Wallet.BalanceOf(player.UserId).ToString())
                        .Replace("%PRICE%", price.ToString());
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

            if (!PluginBus.Request(BusTopics.ContractRerollRequested, plugin.Name, player))
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

        private ShopPerk FindPerk(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length > MaxKeyLength)
                return null;

            foreach (ShopPerk perk in Config.Perks)
            {
                if (string.Equals(perk.Key, key, StringComparison.OrdinalIgnoreCase))
                    return perk;
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
