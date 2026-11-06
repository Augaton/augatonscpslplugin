using System;
using System.Collections.Generic;
using System.Linq;
using AugatonLib.Arcade;
using AugatonLib.Bus;
using AugatonLib.Random;
using AugatonLib.Runtime;
using AugatonLib.World;
using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using Logger = LabApi.Features.Console.Logger;

namespace Contracts
{
    public sealed class ContractService
    {
        private const float TickSeconds = 5f;
        private const float SecondsPerMinute = 60f;

        private readonly Plugin plugin;
        // Rien n'est retire au depart d'un joueur : quitter et revenir lui rendrait un contrat et sa recompense.
        // Stop vide ces deux tables, donc elles restent bornees par les joueurs du round.
        private readonly ObjectiveBook<ContractKind> book = new ObjectiveBook<ContractKind>();
        private readonly Dictionary<string, ContractOffer> offers = new Dictionary<string, ContractOffer>();

        private CoroutineHandle loop;
        private bool running;
        private float aliveSeconds;

        public ContractService(Plugin plugin) => this.plugin = plugin;

        public int ActiveCount => offers.Count;

        private Config Config => plugin.Config;

        private Translation Text => plugin.Translation;

        private bool IsPaused => Config.PauseDuringGamemodes && GamemodeState.BlocksBonuses;

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
            aliveSeconds = 0f;
            book.Clear();
            offers.Clear();
        }

        public string CheckRewards()
        {
            foreach (ContractOffer offer in Config.Offers)
            {
                if (offer is null || (offer.Reward != ItemType.None && !InventoryItemLoader.AvailableItems.ContainsKey(offer.Reward)))
                    return $"objet inconnu : {offer?.Reward}";
            }

            return null;
        }

        public string Describe(Player player)
        {
            string userId = player.UserId;

            if (!book.TryGet(userId, out ContractKind kind, out int progress, out int target) || !offers.TryGetValue(userId, out ContractOffer offer))
                return Text.NoContract;

            if (progress >= target)
                return Text.AlreadyDone;

            return Text.Status
                .Replace("%OBJECTIVE%", ObjectiveOf(kind, target))
                .Replace("%PROGRESS%", progress.ToString())
                .Replace("%TARGET%", target.ToString())
                .Replace("%REWARD%", offer.RewardLabel);
        }

        public void OnPlayerReplaced(BusMessage message)
        {
            if (!message.TryGet(out PlayerReplacement replacement) || replacement.Replacement is null)
                return;

            string from = replacement.ReplacedUserId;
            string to = replacement.Replacement.UserId;

            if (!book.Transfer(from, to) || !offers.TryGetValue(from, out ContractOffer offer))
                return;

            offers.Remove(from);
            offers[to] = offer;
            ShowContract(replacement.Replacement, offer);
        }

        public void OnDeath(PlayerDeathEventArgs ev)
        {
            try
            {
                Player killer = ev?.Attacker;

                if (killer is null || killer == ev.Player || !IsHolder(killer))
                    return;

                if (ev.OldRole != RoleTypeId.Tutorial && ev.OldRole.GetFaction() != killer.Faction)
                    Advance(killer, ContractKind.KillEnemies);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeath: {e}");
            }
        }

        public void OnEscaped(PlayerEscapedEventArgs ev)
        {
            try
            {
                Advance(ev?.Player, ContractKind.Escape);
            }
            catch (Exception e)
            {
                Logger.Error($"OnEscaped: {e}");
            }
        }

        public void OnActivatedGenerator(PlayerActivatedGeneratorEventArgs ev)
        {
            try
            {
                Advance(ev?.Player, ContractKind.ActivateGenerators);
            }
            catch (Exception e)
            {
                Logger.Error($"OnActivatedGenerator: {e}");
            }
        }

        // La demande n'est acceptee qu'une fois le nouveau contrat certain : celui qui vend ce changement ne debite
        // qu'apres, et un joueur sans contrat a changer ne perd ni ses points ni son objectif.
        public void OnRerollRequested(BusMessage message)
        {
            if (!message.TryGet(out Player player) || IsPaused || !offers.TryGetValue(player.UserId, out ContractOffer current))
                return;

            ContractOffer next = Draw(player, current.Kind);

            if (next is null || !book.Discard(player.UserId))
                return;

            offers.Remove(player.UserId);
            Assign(player, next);
            message.Accept();
        }

        public void CountAction(BusMessage message, ContractKind kind)
        {
            if (message.TryGet(out Player player))
                Advance(player, kind);
        }

        public void OnUsedItem(PlayerUsedItemEventArgs ev)
        {
            try
            {
                ItemType? type = ev?.UsableItem?.Type;

                if (type == ItemType.Medkit || type == ItemType.Painkillers || type == ItemType.Adrenaline || type == ItemType.SCP500)
                    Advance(ev.Player, ContractKind.UseMedical);
            }
            catch (Exception e)
            {
                Logger.Error($"OnUsedItem: {e}");
            }
        }

        private IEnumerator<float> Loop()
        {
            yield return Timing.WaitForSeconds(Config.FirstOfferSeconds);

            while (true)
            {
                try
                {
                    Tick();
                }
                catch (Exception e)
                {
                    Logger.Error($"ContractService: {e}");
                }

                yield return Timing.WaitForSeconds(TickSeconds);
            }
        }

        // Une boucle plutot que ChangedRole : les roles du debut de round arrivent avant le depart d'un mode,
        // et un joueur revenu par une vague recoit ainsi son contrat comme les autres.
        private void Tick()
        {
            if (IsPaused)
                return;

            aliveSeconds += TickSeconds;

            bool minutePassed = aliveSeconds >= SecondsPerMinute;

            if (minutePassed)
                aliveSeconds = 0f;

            foreach (Player player in Player.ReadyList)
            {
                if (!IsHolder(player))
                    continue;

                if (!book.Has(player.UserId))
                    Offer(player);
                else if (minutePassed)
                    Advance(player, ContractKind.Survive);
            }
        }

        private void Offer(Player player) => Assign(player, Draw(player, null));

        private ContractOffer Draw(Player player, ContractKind? excluded)
        {
            List<ContractOffer> available = Config.Offers.Where(offer => IsAvailableTo(offer, player) && offer.Kind != excluded).ToList();

            return Chance.PickWeighted(available, candidate => candidate.Weight);
        }

        private void Assign(Player player, ContractOffer offer)
        {
            if (offer is null || !book.Assign(player.UserId, offer.Kind, offer.Target))
                return;

            offers[player.UserId] = offer;

            ShowContract(player, offer);
        }

        private void ShowContract(Player player, ContractOffer offer)
        {
            plugin.Hints.Show(
                player,
                Text.Assigned.Replace("%OBJECTIVE%", ObjectiveOf(offer.Kind, offer.Target)).Replace("%REWARD%", offer.RewardLabel),
                Config.HintDuration);
        }

        // Le jeu ne laisse s'evader que les Classe-D et les scientifiques.
        private static bool IsAvailableTo(ContractOffer offer, Player player)
        {
            if (offer is null || offer.Weight <= 0 || offer.ForScps != player.IsSCP)
                return false;

            string source = SourcePlugin(offer.Kind);

            if (source is not null && !PluginDirectory.IsRegistered(source))
                return false;

            return offer.Kind != ContractKind.Escape || player.Role == RoleTypeId.ClassD || player.Role == RoleTypeId.Scientist;
        }

        private bool IsHolder(Player player)
        {
            return player.IsSCP ? !Config.ExcludedScps.Contains(player.Role) : player.IsHuman && !player.IsTutorial;
        }

        private static string SourcePlugin(ContractKind kind)
        {
            switch (kind)
            {
                case ContractKind.FlipCoins: return "BetterCoinflipsRewritten";
                case ContractKind.SwallowPills: return "SCP500s";
                case ContractKind.UseVending: return "Scp261";
                case ContractKind.ExchangeItem: return "SCP1162";
                default: return null;
            }
        }

        private void Advance(Player player, ContractKind kind)
        {
            if (player is null || IsPaused || !book.Advance(player.UserId, kind))
                return;

            if (!offers.TryGetValue(player.UserId, out ContractOffer offer))
                return;

            if (offer.Reward != ItemType.None)
                ItemGrant.Give(player, offer.Reward, offer.RewardQuantity);

            if (offer.RewardHealth > 0f)
                player.Heal(offer.RewardHealth);

            plugin.Hints.Show(player, Text.Completed.Replace("%REWARD%", offer.RewardLabel), Config.HintDuration);
            PluginBus.Publish(BusTopics.ContractCompleted, plugin.Name, player);
        }

        private string ObjectiveOf(ContractKind kind, int target)
        {
            switch (kind)
            {
                case ContractKind.KillEnemies: return Text.KillEnemies.Replace("%TARGET%", target.ToString());
                case ContractKind.ActivateGenerators: return Text.ActivateGenerators.Replace("%TARGET%", target.ToString());
                case ContractKind.Escape: return Text.Escape;
                case ContractKind.UseMedical: return Text.UseMedical.Replace("%TARGET%", target.ToString());
                case ContractKind.FlipCoins: return Text.FlipCoins.Replace("%TARGET%", target.ToString());
                case ContractKind.SwallowPills: return Text.SwallowPills.Replace("%TARGET%", target.ToString());
                case ContractKind.UseVending: return Text.UseVending.Replace("%TARGET%", target.ToString());
                case ContractKind.ExchangeItem: return Text.ExchangeItem.Replace("%TARGET%", target.ToString());
                default: return Text.Survive.Replace("%TARGET%", target.ToString());
            }
        }
    }
}
