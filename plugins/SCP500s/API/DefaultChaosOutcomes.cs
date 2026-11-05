using System.Collections.Generic;

namespace SCP500s.API
{
    public static class DefaultChaosOutcomes
    {
        public static List<ChaosOutcome> Create()
        {
            List<ChaosOutcome> outcomes = new List<ChaosOutcome>(48);

            AddGood(outcomes);
            AddBad(outcomes);
            AddNeutral(outcomes);

            return outcomes;
        }

        private static void AddGood(List<ChaosOutcome> outcomes)
        {
            outcomes.Add(new ChaosOutcome
            {
                Key = "soin",
                Category = ChaosCategory.Good,
                Weight = 12,
                Message = Messages.Panacea,
                CureNegativeEffects = true,
                Heal = 200f,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "vigueur",
                Category = ChaosCategory.Good,
                Weight = 10,
                Message = Messages.Yamato,
                MinimumMaxHealth = 150f,
                Heal = 150f,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "sprint",
                Category = ChaosCategory.Good,
                Weight = 12,
                Message = Messages.Sonic,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "MovementBoost", Intensity = 30, Duration = 10f },
                    new EffectEntry { Type = "Invigorated", Intensity = 1, Duration = 10f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "voile",
                Category = ChaosCategory.Good,
                Weight = 10,
                Message = Messages.Shadow,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Ghostly", Intensity = 1, Duration = 8f },
                    new EffectEntry { Type = "SilentWalk", Intensity = 10, Duration = 8f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "blindage",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = Messages.Armor,
                GrantedItems = new List<ItemType> { ItemType.ArmorHeavy },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "arsenal",
                Category = ChaosCategory.Good,
                Weight = 6,
                Message = Messages.Arsenal,
                GrantOneRandomItem = true,
                GrantedItems = new List<ItemType>
                {
                    ItemType.GunCOM18,
                    ItemType.GunFSP9,
                    ItemType.GunCrossvec,
                    ItemType.GunE11SR,
                    ItemType.GunAK,
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "nyctalope",
                Category = ChaosCategory.Good,
                Weight = 10,
                Message = Messages.NightVision,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "NightVision", Intensity = 50, Duration = 60f },
                    new EffectEntry { Type = "FocusedVision", Intensity = 1, Duration = 60f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "passepartout",
                Category = ChaosCategory.Good,
                Weight = 4,
                Message = Messages.Passepartout,
                GrantedItems = new List<ItemType> { ItemType.KeycardO5 },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "cuirasse",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = Messages.Juggernaut,
                MinimumMaxHealth = 200f,
                Heal = 200f,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "BodyshotReduction", Intensity = 4, Duration = 45f },
                    new EffectEntry { Type = "DamageReduction", Intensity = 40, Duration = 45f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "regeneration",
                Category = ChaosCategory.Good,
                Weight = 10,
                Message = Messages.Regeneration,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "AnomalousRegeneration", Intensity = 1, Duration = 20f },
                    new EffectEntry { Type = "Vitality", Intensity = 1, Duration = 20f },
                },
                RegenerationPerSecond = 3f,
                RegenerationSeconds = 20f,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "cola",
                Category = ChaosCategory.Good,
                Weight = 6,
                Message = Messages.Cola,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Scp207", Intensity = 1, Duration = 0f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "carapace",
                Category = ChaosCategory.Good,
                Weight = 10,
                Message = Messages.Shield,
                ArtificialHealth = 50f,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "ravitaillement",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = Messages.Ammo,
                Behaviour = PillBehaviour.AmmoRefill,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "radar",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = string.Empty,
                Behaviour = PillBehaviour.ScpRadar,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "recensement",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = string.Empty,
                Behaviour = PillBehaviour.Census,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "promotion",
                Category = ChaosCategory.Good,
                Weight = 5,
                Message = Messages.Promotion,
                Behaviour = PillBehaviour.KeycardUpgrade,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "sourdine",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = Messages.Silence,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "SilentWalk", Intensity = 10, Duration = 45f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "camouflage",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = Messages.Camouflage,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Fade", Intensity = 180, Duration = 25f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "marathon",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = Messages.Marathon,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Invigorated", Intensity = 1, Duration = 45f },
                    new EffectEntry { Type = "MovementBoost", Intensity = 10, Duration = 45f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "clairvoyance",
                Category = ChaosCategory.Good,
                Weight = 8,
                Message = Messages.Clairvoyance,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "FogControl", Intensity = 1, Duration = 45f },
                },
            });
        }

        private static void AddBad(List<ChaosOutcome> outcomes)
        {
            outcomes.Add(new ChaosOutcome
            {
                Key = "poison",
                Category = ChaosCategory.Bad,
                Weight = 12,
                Message = Messages.Poison,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Poisoned", Intensity = 1, Duration = 10f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "cecite",
                Category = ChaosCategory.Bad,
                Weight = 12,
                Message = Messages.Blind,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Blindness", Intensity = 1, Duration = 8f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "surdite",
                Category = ChaosCategory.Bad,
                Weight = 12,
                Message = Messages.Deaf,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Deafened", Intensity = 1, Duration = 12f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "commotion",
                Category = ChaosCategory.Bad,
                Weight = 10,
                Message = Messages.Concussion,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Concussed", Intensity = 1, Duration = 6f },
                    new EffectEntry { Type = "Flashed", Intensity = 1, Duration = 4f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "hemorragie",
                Category = ChaosCategory.Bad,
                Weight = 10,
                Message = Messages.Bleed,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Bleeding", Intensity = 1, Duration = 15f },
                    new EffectEntry { Type = "Hemorrhage", Intensity = 1, Duration = 15f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "brulure",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Burn,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Burned", Intensity = 1, Duration = 10f },
                },
                Damage = 20f,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "asphyxie",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Asphyxia,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Asphyxiated", Intensity = 1, Duration = 12f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "corrosion",
                Category = ChaosCategory.Bad,
                Weight = 6,
                Message = Messages.Corrosion,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Corroding", Intensity = 1, Duration = 8f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "hypothermie",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Hypothermia,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Hypothermia", Intensity = 1, Duration = 20f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "traumatisme",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Trauma,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Traumatized", Intensity = 1, Duration = 25f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "lenteur",
                Category = ChaosCategory.Bad,
                Weight = 10,
                Message = Messages.Sluggish,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Disabled", Intensity = 1, Duration = 20f },
                    new EffectEntry { Type = "Exhausted", Intensity = 1, Duration = 20f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "maladresse",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Butterfingers,
                DropInventory = true,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "entrave",
                Category = ChaosCategory.Bad,
                Weight = 10,
                Message = Messages.Ensnared,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Ensnared", Intensity = 1, Duration = 5f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "amnesie",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Amnesia,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "AmnesiaItems", Intensity = 1, Duration = 15f },
                    new EffectEntry { Type = "AmnesiaVision", Intensity = 1, Duration = 8f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "strangulation",
                Category = ChaosCategory.Bad,
                Weight = 6,
                Message = Messages.Strangled,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Strangled", Intensity = 1, Duration = 6f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "anticola",
                Category = ChaosCategory.Good,
                Weight = 6,
                Message = Messages.AntiCola,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "AntiScp207", Intensity = 1, Duration = 0f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "souillure",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Stain,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Stained", Intensity = 1, Duration = 20f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "gouffre",
                Category = ChaosCategory.Bad,
                Weight = 8,
                Message = Messages.Sinkhole,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Sinkhole", Intensity = 1, Duration = 10f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "dimension",
                Category = ChaosCategory.Bad,
                Weight = 3,
                Message = Messages.Pocket,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "PocketCorroding", Intensity = 1, Duration = 1f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "godkill",
                Category = ChaosCategory.Bad,
                Weight = 3,
                Message = Messages.SkyLaunch,
                Behaviour = PillBehaviour.SkyLaunch,
            });
        }

        private static void AddNeutral(List<ChaosOutcome> outcomes)
        {
            outcomes.Add(new ChaosOutcome
            {
                Key = "teleportation",
                Category = ChaosCategory.Neutral,
                Weight = 12,
                Message = Messages.Xerneas,
                Behaviour = PillBehaviour.RandomTeleport,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "permutation",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Swap,
                Behaviour = PillBehaviour.Swap,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "invocation",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Summon,
                Behaviour = PillBehaviour.Summon,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "retrecissement",
                Category = ChaosCategory.Neutral,
                Weight = 10,
                Message = Messages.Rakun,
                Behaviour = PillBehaviour.Shrink,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "agrandissement",
                Category = ChaosCategory.Neutral,
                Weight = 10,
                Message = Messages.Titan,
                Behaviour = PillBehaviour.Enlarge,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "deguisement",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Disguise,
                Behaviour = PillBehaviour.Disguise,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "trahison",
                Category = ChaosCategory.Neutral,
                Weight = 4,
                Message = Messages.Lumien,
                Behaviour = PillBehaviour.Betrayal,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "renfort",
                Category = ChaosCategory.Neutral,
                Weight = 3,
                Message = Messages.ForceWave,
                Behaviour = PillBehaviour.ForceWave,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "flamant",
                Category = ChaosCategory.Neutral,
                Weight = 0,
                Message = Messages.Flamingo,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "BecomingFlamingo", Intensity = 1, Duration = 0f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "chamallow",
                Category = ChaosCategory.Neutral,
                Weight = 0,
                Message = Messages.Marshmallow,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "MarshmallowEffect", Intensity = 1, Duration = 0f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "ferraille",
                Category = ChaosCategory.Neutral,
                Weight = 0,
                Message = Messages.Metal,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Metal", Intensity = 1, Duration = 0f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "prisme",
                Category = ChaosCategory.Neutral,
                Weight = 10,
                Message = Messages.Prism,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "RainbowTaste", Intensity = 2, Duration = 45f },
                    new EffectEntry { Type = "Blurred", Intensity = 1, Duration = 45f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "blizzard",
                Category = ChaosCategory.Neutral,
                Weight = 0,
                Message = Messages.Snow,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Snowed", Intensity = 1, Duration = 40f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "sucre",
                Category = ChaosCategory.Neutral,
                Weight = 0,
                Message = Messages.Sugar,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "SugarRush", Intensity = 1, Duration = 25f },
                    new EffectEntry { Type = "SugarCrave", Intensity = 1, Duration = 25f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "brouillard",
                Category = ChaosCategory.Bad,
                Weight = 10,
                Message = Messages.Fog,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "FogControl", Intensity = 5, Duration = 30f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "spectre",
                Category = ChaosCategory.Neutral,
                Weight = 10,
                Message = Messages.Spectre,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "Scp1576", Intensity = 1, Duration = 30f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "disco",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Disco,
                Behaviour = PillBehaviour.DiscoLights,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "panne",
                Category = ChaosCategory.Neutral,
                Weight = 6,
                Message = Messages.Blackout,
                Behaviour = PillBehaviour.Blackout,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "souris",
                Category = ChaosCategory.Neutral,
                Weight = 6,
                Message = Messages.Mice,
                Behaviour = PillBehaviour.Mice,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "tantrum",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Tantrum,
                Behaviour = PillBehaviour.Tantrum,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "feudartifice",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Fireworks,
                Behaviour = PillBehaviour.Fireworks,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "annonce",
                Category = ChaosCategory.Neutral,
                Weight = 6,
                Message = Messages.Cassie,
                Behaviour = PillBehaviour.CassiePrank,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "contagion",
                Category = ChaosCategory.Neutral,
                Weight = 6,
                Message = Messages.Contagion,
                Behaviour = PillBehaviour.Contagion,
                Effects = new List<EffectEntry>
                {
                    new EffectEntry { Type = "RainbowTaste", Intensity = 2, Duration = 20f },
                    new EffectEntry { Type = "Blurred", Intensity = 1, Duration = 20f },
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "roulette",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Roulette,
                Behaviour = PillBehaviour.RandomEffect,
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "tirelire",
                Category = ChaosCategory.Neutral,
                Weight = 8,
                Message = Messages.Coins,
                GrantedItems = new List<ItemType>
                {
                    ItemType.Coin,
                    ItemType.Coin,
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "friandise",
                Category = ChaosCategory.Neutral,
                Weight = 6,
                Message = Messages.Candy,
                GrantedItems = new List<ItemType>
                {
                    ItemType.SCP330,
                },
            });
            outcomes.Add(new ChaosOutcome
            {
                Key = "talkie",
                Category = ChaosCategory.Neutral,
                Weight = 6,
                Message = Messages.Radio,
                GrantedItems = new List<ItemType>
                {
                    ItemType.Radio,
                },
            });
        }

        public static List<ChaosOutcome> AddMissing(List<ChaosOutcome> current)
        {
            if (current is null)
                return null;

            HashSet<string> keys = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (ChaosOutcome outcome in current)
            {
                if (outcome is not null && !string.IsNullOrEmpty(outcome.Key))
                    keys.Add(outcome.Key);
            }

            foreach (ChaosOutcome outcome in Create())
            {
                if (keys.Contains(outcome.Key))
                    continue;

                current.Add(outcome);
            }

            return current;
        }
    }
}

#pragma warning restore CS0618
