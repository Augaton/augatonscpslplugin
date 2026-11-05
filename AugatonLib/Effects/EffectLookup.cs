using System;
using System.Collections.Generic;
using System.Reflection;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace AugatonLib.Effects
{
    /// <summary>
    /// Recherche d'un effet par nom de classe exact. Player.TryGetEffect(string) compare le nom du
    /// GameObject avec StartsWith : Scp1344 pourrait y trouver Scp1344Detected.
    /// </summary>
    public static class EffectLookup
    {
        private static HashSet<string> knownNames;

        public static bool IsKnown(string className)
        {
            knownNames ??= LoadKnownNames();

            return !string.IsNullOrEmpty(className) && knownNames.Contains(className);
        }

        public static bool TryGet(Player player, string className, out StatusEffectBase effect)
        {
            foreach (StatusEffectBase candidate in player.ReferenceHub.playerEffectsController.AllEffects)
            {
                if (!string.Equals(candidate.GetType().Name, className, StringComparison.OrdinalIgnoreCase))
                    continue;

                effect = candidate;
                return true;
            }

            effect = null;
            return false;
        }

        private static HashSet<string> LoadKnownNames()
        {
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Type type in LoadableTypes(typeof(StatusEffectBase).Assembly))
            {
                if (type is not null && !type.IsAbstract && type.IsSubclassOf(typeof(StatusEffectBase)))
                    names.Add(type.Name);
            }

            return names;
        }

        private static Type[] LoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types;
            }
        }
    }
}
