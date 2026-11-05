using System;
using AugatonLib.Hints;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// Socle des plugins : la validation et les reglages de hints sont rejoues a chaque rechargement,
    /// car LoadConfigs est le seul point d'entree de <c>labapi reload configs</c>.
    /// </summary>
    public abstract class AugatonPlugin<TConfig> : Plugin<TConfig>
        where TConfig : class, new()
    {
        private const float DefaultHintYCoordinate = 600f;

        private HintChannel hints;

        public override Version RequiredApiVersion => BuildInfo.LabApiVersion;

        public HintChannel Hints => hints ??= new HintChannel(Name.ToLowerInvariant(), DefaultHintYCoordinate);

        public override void LoadConfigs()
        {
            base.LoadConfigs();
            ValidateConfig();

            if (Config is IHintSettings settings)
            {
                Hints.YCoordinate = settings.HintYCoordinate;
                Hints.FontSize = settings.HintFontSize;
            }
        }

        protected virtual void ValidateConfig()
        {
        }
    }

    public abstract class AugatonPlugin<TConfig, TTranslation> : AugatonPlugin<TConfig>
        where TConfig : class, new()
        where TTranslation : class, new()
    {
        private const string TranslationFileName = "translation.yml";

        public TTranslation Translation { get; private set; } = new TTranslation();

        public override void LoadConfigs()
        {
            if (this.TryLoadConfig(TranslationFileName, out TTranslation loaded) && loaded is not null)
            {
                Translation = loaded;
            }
            else
            {
                Logger.Warn($"{Name}: {TranslationFileName} illisible, textes par defaut utilises.");
                Translation = new TTranslation();
            }

            base.LoadConfigs();
        }
    }
}
