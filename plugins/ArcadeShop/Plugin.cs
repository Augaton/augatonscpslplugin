using System.Collections.Generic;
using AugatonLib.Bus;
using AugatonLib.Arcade;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace ArcadeShop
{
    public sealed class Plugin : AugatonPlugin<Config, Translation>
    {
        public static Plugin Instance { get; private set; }

        public ShopService Service { get; private set; }

        public Wallet Wallet { get; } = new Wallet();

        public override string Name => "ArcadeShop";

        public override string Description => "Des points gagnes pendant le round, a depenser dans une boutique.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            Instance = this;
            Service = new ShopService(this);

            PlayerEvents.Death += Service.OnDeath;
            PlayerEvents.ActivatedGenerator += Service.OnActivatedGenerator;
            PlayerEvents.Escaped += Service.OnEscaped;
            PlayerEvents.Left += Service.OnLeft;
            ServerEvents.RoundRestarted += Service.Reset;

            PluginBus.Subscribe(Name, BusTopics.ContractCompleted, Service.OnContractCompleted);
            PluginBus.Subscribe(Name, BusTopics.StreakReached, Service.OnStreakReached);
            PluginBus.Subscribe(Name, BusTopics.GamemodeWon, Service.OnGamemodeWon);
            PluginBus.Subscribe(Name, BusTopics.PlayerReplaced, Service.OnPlayerReplaced);

            SelfCheck.Register(Name, "objets de la boutique", Service.CheckItems);

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            PlayerEvents.Death -= Service.OnDeath;
            PlayerEvents.ActivatedGenerator -= Service.OnActivatedGenerator;
            PlayerEvents.Escaped -= Service.OnEscaped;
            PlayerEvents.Left -= Service.OnLeft;
            ServerEvents.RoundRestarted -= Service.Reset;

            Service.Reset();
            PluginBus.UnsubscribeOwner(Name);
            PluginDirectory.Unregister(this);

            Service = null;
            Instance = null;
        }

        protected override void ValidateConfig()
        {
            Config.BuyCooldownSeconds = AtLeast("buy_cooldown_seconds", Config.BuyCooldownSeconds, 1f);
            Config.CatalogCooldownSeconds = AtLeast("catalog_cooldown_seconds", Config.CatalogCooldownSeconds, 0.5f);
            Config.MaxBalance = (int)AtLeast("max_balance", Config.MaxBalance, 1f);
            Wallet.MaxBalance = Config.MaxBalance;

            if (Config.Items is null)
            {
                Logger.Warn("items est nul dans config.yml, la boutique est vide.");
                Config.Items = new List<ShopItem>();
            }

            Config.Items.RemoveAll(IsInvalid);
        }

        private static bool IsInvalid(ShopItem item)
        {
            if (item is not null && !string.IsNullOrEmpty(item.Key) && item.Item != ItemType.None && item.Price >= 0)
                return false;

            Logger.Warn($"Objet de boutique ignore : {item?.Key ?? "sans nom"}.");
            return true;
        }
    }
}
