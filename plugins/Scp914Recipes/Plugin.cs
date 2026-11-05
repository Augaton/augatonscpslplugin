using System.Collections.Generic;
using AugatonLib.Runtime;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace Scp914Recipes
{
    public sealed class Plugin : AugatonPlugin<Config>
    {
        private RecipeBook book;

        public override string Name => "Scp914Recipes";

        public override string Description => "Ajoute des recettes a SCP-914, reglables en config.";

        public override string Author => "Augaton";

        public override void Enable()
        {
            book = new RecipeBook(this);

            Scp914Events.ProcessingPickup += book.OnProcessingPickup;
            Scp914Events.ProcessingInventoryItem += book.OnProcessingInventoryItem;

            SelfCheck.Register(Name, "recettes valides", book.CheckRecipes);

            PluginDirectory.Register(this, Capability.Bus);
        }

        public override void Disable()
        {
            Scp914Events.ProcessingPickup -= book.OnProcessingPickup;
            Scp914Events.ProcessingInventoryItem -= book.OnProcessingInventoryItem;

            PluginDirectory.Unregister(this);

            book = null;
        }

        protected override void ValidateConfig()
        {
            if (Config.Recipes is not null)
                return;

            Logger.Warn("recipes est nul dans la config, aucune recette active.");
            Config.Recipes = new List<Recipe>();
        }
    }
}
