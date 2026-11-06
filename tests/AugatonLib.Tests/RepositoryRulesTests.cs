using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit;

namespace AugatonLib.Tests
{
    /// <summary>
    /// Les regles de CLAUDE.md qui se verifient sur le texte du code. Le jeu ne peut pas tourner ici :
    /// ces tests evitent qu'une erreur deja corrigee revienne sans bruit.
    /// </summary>
    public class RepositoryRulesTests
    {
        // Ces reglages ne limitent pas une action gratuite : une pilule se consomme, et le delai de
        // Replacer espace deux remplacements du meme joueur. Zero y reste un choix valable.
        private static readonly string[] UnboundedCooldowns = { "PerPlayerCooldownSeconds", "PillCooldownSeconds" };

        private static readonly Regex EventSubscription = new Regex(
            @"^\s*((?:[A-Za-z]+Events|CharacterClassManager|PlayerDeparture|ItemPickupBase|ServerSpecificSettingsSync|RoundRestart)\.\w+)\s*(\+=|-=)\s*([\w.]+)\s*;",
            RegexOptions.Multiline);

        private static readonly Regex ForbiddenWrite = new Regex(
            @"Server\.FriendlyFire\s*=[^=]|\b[pP]layer\.Scale\s*=[^=]|Map\.TurnOffLights|Map\.SetColorOfLights|OverrideLightsColor");

        private static string Root([CallerFilePath] string path = null)
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..", ".."));
        }

        private static IEnumerable<string> PluginFolders()
        {
            return Directory.GetDirectories(Path.Combine(Root(), "plugins"));
        }

        private static IEnumerable<string> SourcesOf(string folder)
        {
            return Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories)
                .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                    && !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"));
        }

        [Fact]
        public void NoRealWebhookIsCommitted()
        {
            string[] ignored = { ".git", ".references", "bin", "obj", "out" };
            string[] extensions = { ".cs", ".md", ".yml", ".yaml", ".json", ".sh", ".txt", ".csproj", ".props", ".targets" };
            List<string> pending = new List<string> { Root() };
            List<string> offenders = new List<string>();

            while (pending.Count > 0)
            {
                string folder = pending[pending.Count - 1];
                pending.RemoveAt(pending.Count - 1);

                pending.AddRange(Directory.GetDirectories(folder).Where(path => !ignored.Contains(Path.GetFileName(path))));

                offenders.AddRange(Directory.GetFiles(folder)
                    .Where(file => extensions.Contains(Path.GetExtension(file)))
                    .Where(file => Regex.IsMatch(File.ReadAllText(file), @"discord(app)?\.com/api/webhooks/\d{15,}/")));
            }

            Assert.Empty(offenders);
        }

        [Fact]
        public void EveryEventSubscriptionIsUndone()
        {
            List<string> unbalanced = new List<string>();

            foreach (string folder in PluginFolders())
            {
                List<Match> matches = SourcesOf(folder)
                    .SelectMany(file => EventSubscription.Matches(File.ReadAllText(file)).Cast<Match>())
                    .ToList();

                foreach (var pair in matches.GroupBy(match => $"{match.Groups[1].Value} {match.Groups[3].Value}"))
                {
                    if (pair.Count(match => match.Groups[2].Value == "+=") != pair.Count(match => match.Groups[2].Value == "-="))
                        unbalanced.Add($"{Path.GetFileName(folder)} : {pair.Key}");
                }
            }

            Assert.Empty(unbalanced);
        }

        [Fact]
        public void SharedResourcesGoThroughTheirArbiter()
        {
            List<string> offenders = PluginFolders()
                .SelectMany(SourcesOf)
                .Where(file => ForbiddenWrite.IsMatch(File.ReadAllText(file)))
                .Select(file => Path.GetRelativePath(Root(), file))
                .ToList();

            Assert.Empty(offenders);
        }

        [Fact]
        public void EveryHintHasItsOwnLine()
        {
            List<string> coordinates = PluginFolders()
                .Select(folder => Path.Combine(folder, "Config.cs"))
                .Where(File.Exists)
                .Select(file => Regex.Match(File.ReadAllText(file), @"HintYCoordinate\s*\{[^}]*\}\s*=\s*([\d.]+)").Groups[1].Value)
                .Where(value => value.Length > 0)
                .ToList();

            Assert.Equal(coordinates.Count, coordinates.Distinct().Count());
        }

        [Fact]
        public void PlayerCooldownsCannotBeTurnedOffByTheConfig()
        {
            List<string> unbounded = new List<string>();

            foreach (string folder in PluginFolders())
            {
                string config = Path.Combine(folder, "Config.cs");

                if (!File.Exists(config))
                    continue;

                string sources = string.Concat(SourcesOf(folder).Select(File.ReadAllText));

                foreach (Match match in Regex.Matches(File.ReadAllText(config), @"public float (\w*Cooldown(?:Seconds)?)\b"))
                {
                    string name = match.Groups[1].Value;

                    if (UnboundedCooldowns.Contains(name))
                        continue;

                    if (!Regex.IsMatch(sources, $@"{name}\s*=\s*(AtLeast|Clamp)\("))
                        unbounded.Add($"{Path.GetFileName(folder)}.{name}");
                }
            }

            Assert.Empty(unbounded);
        }

        [Fact]
        public void NoCatchBlockIsEmpty()
        {
            List<string> offenders = PluginFolders()
                .Concat(new[] { Path.Combine(Root(), "AugatonLib") })
                .SelectMany(SourcesOf)
                .Where(file => Regex.IsMatch(File.ReadAllText(file), @"catch(\s*\([^)]*\))?\s*\{\s*\}"))
                .Select(file => Path.GetRelativePath(Root(), file))
                .ToList();

            Assert.Empty(offenders);
        }

        [Fact]
        public void OnlyAntiSlCrushPatchesTheGame()
        {
            List<string> offenders = PluginFolders()
                .Where(folder => Path.GetFileName(folder) != "AntiSLCrush")
                .SelectMany(SourcesOf)
                .Where(file => File.ReadAllText(file).Contains("using HarmonyLib"))
                .Select(file => Path.GetRelativePath(Root(), file))
                .ToList();

            Assert.Empty(offenders);
        }

        [Fact]
        public void AntiSlCrushKeepsItsCopyrightHeaders()
        {
            List<string> offenders = SourcesOf(Path.Combine(Root(), "plugins", "AntiSLCrush"))
                .Where(file => !File.ReadAllText(file).Contains("Copyright (c) 2026 ui_2506."))
                .Select(file => Path.GetRelativePath(Root(), file))
                .ToList();

            Assert.Empty(offenders);
        }

        [Fact]
        public void EveryPluginDocumentsItselfAndItsLicence()
        {
            List<string> offenders = PluginFolders()
                .Where(folder => !File.Exists(Path.Combine(folder, "README.md"))
                    || !new[] { "LICENSE", "LICENSE.md", "NOTICE.md" }.Any(name => File.Exists(Path.Combine(folder, name))))
                .Select(Path.GetFileName)
                .ToList();

            Assert.Empty(offenders);
        }
    }
}
