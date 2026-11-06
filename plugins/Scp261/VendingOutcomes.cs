using System;
using System.Collections.Generic;
using System.Linq;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.Effects;
using AugatonLib.Random;
using AugatonLib.World;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Utils;
using Logger = LabApi.Features.Console.Logger;

namespace Scp261
{
    public sealed class VendingOutcomes
    {
        private const int SafetySteps = 20;

        private readonly Plugin plugin;

        public VendingOutcomes(Plugin plugin) => this.plugin = plugin;

        private Config Config => plugin.Config;

        private Translation Text => plugin.Translation;

        public void Roll(Player player)
        {
            List<Outcome> outcomes = Build(player);

            Chance.PickWeighted(outcomes, outcome => outcome.Weight)?.Run();
        }

        private List<Outcome> Build(Player player)
        {
            return new List<Outcome>(7)
            {
                new Outcome(Config.ItemChance, true, () => GiveItem(player), player, this),
                new Outcome(Config.EffectChance, Config.EnableEffects, () => ApplyEffect(player), player, this),
                new Outcome(Config.SizeChangeChance, Config.EnableSizeChange, () => Shrink(player), player, this),
                new Outcome(Config.ExplodeChance, Config.EnableExplode, () => Explode(player), player, this),
                new Outcome(Config.FlashbangChance, Config.EnableFlashbang, () => Flashbang(player), player, this),
                new Outcome(Config.TantrumChance, Config.EnableTantrum, () => Tantrum(player), player, this),
                new Outcome(Config.RandomTeleportChance, Config.RandomTeleport, () => Teleport(player), player, this),
            };
        }

        private void GiveItem(Player player)
        {
            List<ItemType> stock = Config.VendingMachineStock;

            if (stock is null || stock.Count == 0)
                return;

            ItemType type = Chance.Pick(stock);

            // Avec SCP500s, la machine donne une vraie pilule. Sans lui, personne ne repond a la demande.
            if (type == ItemType.SCP500 && PluginBus.Publish(BusTopics.PillGiveRequested, Plugin.Owner, player) > 0)
            {
                Show(player, Text.ItemDispensed);
                return;
            }

            Item item = player.AddItem(type);

            if (item is null)
                Pickup.Create(type, player.Position, Quaternion.identity);
            else if (!Config.SpawnWeaponsWithAmmo && item is FirearmItem firearm)
                EmptyFirearm(firearm);

            Show(player, Text.ItemDispensed);
        }

        private static void EmptyFirearm(FirearmItem firearm)
        {
            firearm.StoredAmmo = 0;
            firearm.ChamberedAmmo = 0;
        }

        private void ApplyEffect(Player player)
        {
            string name = Chance.Pick(Config.EffectList);

            if (string.IsNullOrEmpty(name) || !EffectLookup.TryGet(player, name, out StatusEffectBase effect))
            {
                GiveItem(player);
                return;
            }

            player.EnableEffect(effect, 1, UnityEngine.Random.Range(Config.MinEffectDuration, Config.MaxEffectDuration + 1));
            Show(player, Text.EffectApplied);
        }

        private void Shrink(Player player)
        {
            ScaleArbiter.Request(player, Plugin.Owner, Config.GnomedSize);
            Show(player, Text.Shrunk);
        }

        private void Explode(Player player)
        {
            ExplosionUtils.ServerExplode(player.ReferenceHub, ExplosionType.Grenade);
            Show(player, Text.Exploded);
        }

        private void Flashbang(Player player)
        {
            // Sans lanceur, un flash ne touche personne si le tir allie est coupe : l'effet s'applique a part.
            TimedGrenadeProjectile.SpawnActive(player.Position, ItemType.GrenadeFlash, null, 0f);
            player.EnableEffect<Flashed>(1, Config.FlashbangSeconds);
            Show(player, Text.Flashbang);
        }

        private void Tantrum(Player player)
        {
            if (Tantrums.TrySpawn(player.Position))
                Show(player, Text.Tantrum);
            else
                GiveItem(player);
        }

        private void Teleport(Player player)
        {
            bool avoidLightContainment = Decontamination.IsDecontaminating;

            List<Door> doors = Door.List
                .Where(door => !door.IsLocked && (!avoidLightContainment || door.Zone != FacilityZone.LightContainment))
                .ToList();

            Door door = Chance.Pick(doors);

            if (door is null)
            {
                GiveItem(player);
                return;
            }

            player.Position = SafePositionNear(door.Transform);
            Show(player, Text.Teleported);
        }

        private static Vector3 SafePositionNear(Transform door)
        {
            Vector3 position = door.position + Vector3.up;

            for (int step = 0; step < SafetySteps && Physics.CheckSphere(position, 0.35f, FpcStateProcessor.Mask); step++)
                position += door.forward * 0.35f;

            return position;
        }

        private void Show(Player player, string text) => plugin.Hints.Show(player, text, Config.HintDuration);

        private sealed class Outcome
        {
            private readonly Action action;
            private readonly Action fallback;

            public Outcome(int weight, bool enabled, Action action, Player player, VendingOutcomes outcomes)
            {
                Weight = weight;
                this.action = enabled ? action : () => outcomes.GiveItem(player);
                fallback = () => outcomes.GiveItem(player);
            }

            public int Weight { get; }

            public void Run()
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Logger.Error($"SCP-261 : {e}");
                    fallback();
                }
            }
        }
    }
}
