using AugatonLib.World;
using System.Collections.Generic;
using System.Text;
using AugatonLib.Arbitration;
using AugatonLib.Random;
using AugatonLib.Text;
using Cassie;
using Footprinting;
using Hazards;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using UnityEngine;
using Utils;
using BaseThrowableItem = InventorySystem.Items.ThrowableProjectiles.ThrowableItem;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public static class PillBehaviours
    {
        private static readonly RoleTypeId[] Disguises =
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp096,
            RoleTypeId.Scp173,
            RoleTypeId.Scp3114,
            RoleTypeId.Scp106,
        };

        private static readonly Dictionary<RoleTypeId, RoleTypeId> Betrayals = new Dictionary<RoleTypeId, RoleTypeId>
        {
            { RoleTypeId.ClassD, RoleTypeId.Scientist },
            { RoleTypeId.Scientist, RoleTypeId.ClassD },
            { RoleTypeId.FacilityGuard, RoleTypeId.ChaosConscript },
            { RoleTypeId.NtfPrivate, RoleTypeId.ChaosRifleman },
            { RoleTypeId.NtfSergeant, RoleTypeId.ChaosMarauder },
            { RoleTypeId.NtfSpecialist, RoleTypeId.ChaosConscript },
            { RoleTypeId.NtfCaptain, RoleTypeId.ChaosRepressor },
            { RoleTypeId.ChaosRifleman, RoleTypeId.NtfPrivate },
            { RoleTypeId.ChaosConscript, RoleTypeId.NtfSpecialist },
            { RoleTypeId.ChaosMarauder, RoleTypeId.NtfSergeant },
            { RoleTypeId.ChaosRepressor, RoleTypeId.NtfCaptain },
        };

        private static readonly ItemType[] AmmoTypes =
        {
            ItemType.Ammo9x19,
            ItemType.Ammo556x45,
            ItemType.Ammo762x39,
            ItemType.Ammo12gauge,
            ItemType.Ammo44cal,
        };

        private static Config Config => Plugin.Instance.Config;

        private static Translation Text => Plugin.Instance.Translation;

        public static void Run(Player player, EffectPayload payload, string sourceKey)
        {
            if (payload is null || player is null || !player.IsAlive)
                return;

            switch (payload.Behaviour)
            {
                case PillBehaviour.Shrink: Shrink(player); return;
                case PillBehaviour.Enlarge: Enlarge(player); return;
                case PillBehaviour.RandomTeleport: RandomTeleport(player, sourceKey); return;
                case PillBehaviour.Betrayal: Betrayal(player, sourceKey); return;
                case PillBehaviour.Disguise: Disguise(player); return;
                case PillBehaviour.SkyLaunch: SkyLaunch(player, sourceKey); return;
                case PillBehaviour.Swap: Swap(player, sourceKey); return;
                case PillBehaviour.Summon: Summon(player, sourceKey); return;
                case PillBehaviour.ForceWave: ForceWave(player, sourceKey); return;
                case PillBehaviour.RandomEffect: RandomEffect(player); return;
                case PillBehaviour.Blackout: Blackout(player); return;
                case PillBehaviour.DiscoLights: DiscoLights(player); return;
                case PillBehaviour.Mice: Mice(); return;
                case PillBehaviour.Tantrum: Tantrum(player); return;
                case PillBehaviour.Fireworks: Fireworks(player); return;
                case PillBehaviour.Contagion: Contagion(player, payload); return;
                case PillBehaviour.CassiePrank: CassiePrank(player, sourceKey); return;
                case PillBehaviour.AmmoRefill: AmmoRefill(player); return;
                case PillBehaviour.Census: Census(player); return;
                case PillBehaviour.ScpRadar: ScpRadar(player); return;
                case PillBehaviour.KeycardUpgrade: KeycardUpgrade(player, sourceKey); return;
            }
        }

        private static void AmmoRefill(Player player)
        {
            bool armed = false;

            foreach (Item item in player.Items)
            {
                if (item is not FirearmItem firearm)
                    continue;

                armed = true;
                FillAmmo(player, firearm.AmmoType);
            }

            if (armed)
                return;

            foreach (ItemType type in AmmoTypes)
                FillAmmo(player, type);
        }

        internal static void FillAmmo(Player player, ItemType type)
        {
            if (type == ItemType.None)
                return;

            ushort limit = InventoryLimits.GetAmmoLimit(type, player.ReferenceHub);

            if (player.GetAmmo(type) < limit)
                player.SetAmmo(type, limit);
        }

        private static void Census(Player player)
        {
            int scps = 0;
            int foundation = 0;
            int chaos = 0;
            int scientists = 0;
            int classD = 0;

            foreach (Player other in Player.ReadyList)
            {
                if (other is null || other.IsDummy || !other.IsAlive)
                    continue;

                switch (other.Team)
                {
                    case Team.SCPs: scps++; break;
                    case Team.FoundationForces: foundation++; break;
                    case Team.ChaosInsurgency: chaos++; break;
                    case Team.Scientists: scientists++; break;
                    case Team.ClassD: classD++; break;
                }
            }

            string text = Text.Census
                .Replace("{scp}", scps.ToString())
                .Replace("{foundation}", foundation.ToString())
                .Replace("{chaos}", chaos.ToString())
                .Replace("{scientists}", scientists.ToString())
                .Replace("{classd}", classD.ToString());

            Plugin.Instance.Hints.Show(player, text, Config.HintDuration);
        }

        private static void ScpRadar(Player player)
        {
            List<Player> scps = new List<Player>(8);

            foreach (Player other in Player.ReadyList)
            {
                if (other is null || other == player || other.IsDummy || !other.IsAlive)
                    continue;

                if (other.Team != Team.SCPs || other.Role == RoleTypeId.Scp079)
                    continue;

                scps.Add(other);
            }

            if (scps.Count == 0)
            {
                Plugin.Instance.Hints.Show(player, Text.RadarEmpty, Config.HintDuration);
                return;
            }

            Vector3 origin = player.Position;
            scps.Sort((left, right) => (left.Position - origin).sqrMagnitude.CompareTo((right.Position - origin).sqrMagnitude));

            StringBuilder builder = new StringBuilder(Text.RadarHeader);
            int shown = Mathf.Min(scps.Count, Config.RadarMaxEntries);

            for (int i = 0; i < shown; i++)
            {
                Player scp = scps[i];
                string line = Text.RadarLine
                    .Replace("{role}", RoleNames.Full(scp.Role))
                    .Replace("{distance}", Mathf.RoundToInt(Vector3.Distance(origin, scp.Position)).ToString());

                builder.Append('\n').Append(line);
            }

            Plugin.Instance.Hints.Show(player, builder.ToString(), Config.HintDuration);
        }

        private static void KeycardUpgrade(Player player, string sourceKey)
        {
            List<ItemType> path = Config.KeycardUpgradePath;

            if (path is null || path.Count == 0)
                return;

            Item best = null;
            int bestTier = -1;

            foreach (Item item in player.Items)
            {
                if (item is null || Plugin.Instance.Registry.TryGet(item, out _))
                    continue;

                int tier = path.IndexOf(item.Type);

                if (tier <= bestTier)
                    continue;

                best = item;
                bestTier = tier;
            }

            if (bestTier >= path.Count - 1)
                return;

            if (best is null && player.Inventory.UserInventory.Items.Count >= Config.MaxInventorySlots)
                return;

            if (best is not null)
                player.RemoveItem(best);

            ItemType upgraded = path[bestTier + 1];
            player.AddItem(upgraded);

            Audit(player, $"a obtenu {upgraded} par {sourceKey}");
        }

        private static void Shrink(Player player)
        {
            float scale = Config.RakunScale;

            Plugin.Instance.Scales.Apply(player, new Vector3(scale, scale, scale));
        }

        private static void Enlarge(Player player)
        {
            float scale = Config.GiantScale;

            Plugin.Instance.Scales.Apply(player, new Vector3(scale, scale, scale));
        }

        private static void RandomTeleport(Player player, string sourceKey)
        {
            Room destination = Plugin.Instance.Rooms.PickRandom();

            if (destination is null)
                return;

            player.Position = destination.Position + Vector3.up;
            Audit(player, $"teleporte en {destination.Name} par {sourceKey}");
        }

        private static void Betrayal(Player player, string sourceKey)
        {
            RoleTypeId current = player.Role;

            if (!Betrayals.TryGetValue(current, out RoleTypeId newRole) || newRole == current)
                return;

            player.SetRole(newRole, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
            Audit(player, $"passe de {current} a {newRole} par {sourceKey}");
        }

        private static void Disguise(Player player)
        {
            if (!(player.RoleBase is FpcStandardRoleBase role))
                return;

            RoleTypeId disguise = Disguises[UnityEngine.Random.Range(0, Disguises.Length)];

            role.FpcModule.MouseLook.GetSyncValues(0, out ushort horizontal, out _);

            // Donnees de spawn du role imite, lues par le client avant celles du vrai role, qu'il ignore.
            NetworkWriterPooled spawnData = NetworkWriterPool.Get();
            spawnData.WriteRelativePosition(new RelativePosition(player.ReferenceHub));
            spawnData.WriteUShort(horizontal);

            foreach (Player viewer in Player.ReadyList)
            {
                if (viewer != player)
                    viewer.Connection.Send(new RoleSyncInfo(player.ReferenceHub, disguise, viewer.ReferenceHub, spawnData));
            }

            NetworkWriterPool.Return(spawnData);

            // Sans deplacement, le joueur reste invisible chez les autres tant qu'il ne bouge pas.
            player.Position += Vector3.up * 0.25f;
        }

        private static void SkyLaunch(Player player, string sourceKey)
        {
            string userId = player.UserId;
            int lifeId = player.LifeId;

            player.Position += Vector3.up * Config.SkyLaunchHeight;
            Audit(player, $"projete en l'air par {sourceKey}");

            Plugin.Instance.Delayed.Run(Config.SkyLaunchFuse, () =>
            {
                Player target = Player.Get(userId);

                if (target is null || target.IsDestroyed || !target.IsAlive || target.LifeId != lifeId)
                    return;

                Explode(target);
            });
        }

        private static void Swap(Player player, string sourceKey)
        {
            if (TargetPicker.IsInDecontaminatedLcz(player))
                return;

            Player partner = Plugin.Instance.Targets.PickRandomAlive(player, true);

            if (partner is null)
                return;

            Vector3 origin = player.Position;

            player.Position = partner.Position;
            partner.Position = origin;

            Plugin.Instance.Hints.Show(partner, Text.SwapVictim, Config.HintDuration);
            Audit(player, $"a echange sa position avec {partner.Nickname} ({partner.UserId}) par {sourceKey}");
        }

        private static void Summon(Player player, string sourceKey)
        {
            if (TargetPicker.IsInDecontaminatedLcz(player))
                return;

            Player victim = Plugin.Instance.Targets.PickRandomAlive(player);

            if (victim is null)
                return;

            victim.Position = player.Position + (Vector3.up * 0.5f);

            Plugin.Instance.Hints.Show(victim, Text.SummonVictim, Config.HintDuration);
            Audit(player, $"a invoque {victim.Nickname} ({victim.UserId}) par {sourceKey}");
        }

        private static void ForceWave(Player player, string sourceKey)
        {
            List<SpawnableFaction> factions = Config.ForceWaveFactions;

            if (factions is null || factions.Count == 0)
                return;

            SpawnableFaction faction = Chance.Pick(factions);

            if (faction == SpawnableFaction.None)
                return;

            FindWave(faction)?.InstantRespawn();

            if (Config.ForceWaveBroadcastDuration > 0)
                Server.SendBroadcast(Text.ForceWaveBroadcast, Config.ForceWaveBroadcastDuration, global::Broadcast.BroadcastFlags.Normal, true);

            Audit(player, $"a force la vague {faction} par {sourceKey}");
        }

        private static RespawnWave FindWave(SpawnableFaction faction)
        {
            switch (faction)
            {
                case SpawnableFaction.NtfWave: return RespawnWaves.PrimaryMtfWave;
                case SpawnableFaction.ChaosWave: return RespawnWaves.PrimaryChaosWave;
                case SpawnableFaction.NtfMiniWave: return RespawnWaves.MiniMtfWave;
                case SpawnableFaction.ChaosMiniWave: return RespawnWaves.MiniChaosWave;
                default: return null;
            }
        }

        private static void RandomEffect(Player player)
        {
            EffectCatalog.ApplyRandom(player, Config.RandomEffectIntensity, Config.RandomEffectDuration);
        }

        private static void Blackout(Player player)
        {
            FacilityZone zone = player.Zone;

            if (zone == FacilityZone.None)
                return;

            LightArbiter.Blackout(Interop.Owner, Config.BlackoutDuration, new[] { zone });
        }

        private static void DiscoLights(Player player)
        {
            Room room = player.Room;

            if (room is null)
                return;

            Color color = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

            int version = Plugin.Instance.Tints.Tint(room, color);

            Plugin.Instance.Delayed.Run(Config.DiscoDuration, () => Plugin.Instance.Tints.Reset(room, version));
        }

        private static void Mice()
        {
            if (!AugatonLib.World.Mice.TrySpawn(Config.MiceType))
                Logger.Warn($"Souris de type {Config.MiceType} impossibles a faire apparaitre, mice_type hors bornes ou refuge absent.");
        }

        private static void Tantrum(Player player)
        {
            if (!Tantrums.TrySpawn(player.Position))
                Logger.Warn("Modele de tantrum introuvable.");
        }

        private static void Explode(Player target)
        {
            if (!InventoryItemLoader.TryGetItem(ItemType.GrenadeHE, out BaseThrowableItem item))
                return;

            if (!(UnityEngine.Object.Instantiate(item.Projectile) is TimeGrenade grenade))
                return;

            ExplosionUtils.ServerSpawnEffect(target.Position, ItemType.GrenadeHE);

            grenade.Position = target.Position;
            grenade.PreviousOwner = new Footprint(target.ReferenceHub);
            grenade.ServerFuseEnd();
        }

        private static void Fireworks(Player player)
        {
            ExplosionUtils.ServerSpawnEffect(player.Position, ItemType.GrenadeFlash);
            ExplosionUtils.ServerSpawnEffect(player.Position, ItemType.SCP2176);
        }

        private static void Contagion(Player player, EffectPayload payload)
        {
            if (payload.Effects is null || payload.Effects.Count == 0)
                return;

            float squaredRadius = Config.ContagionRadius * Config.ContagionRadius;
            Vector3 origin = player.Position;
            int touched = 0;

            foreach (Player other in Player.ReadyList)
            {
                if (other == player || !Plugin.Instance.Targets.IsEligible(other))
                    continue;

                if ((other.Position - origin).sqrMagnitude > squaredRadius)
                    continue;

                PayloadExecutor.ApplyEffects(other, payload);
                Plugin.Instance.Hints.Show(other, Text.ContagionVictim, Config.HintDuration);
                touched++;
            }

            if (touched > 0)
                Audit(player, $"a contamine {touched} joueur(s)");
        }

        private static void CassiePrank(Player player, string sourceKey)
        {
            List<string> lines = Config.CassieLines;

            if (lines is null || lines.Count == 0)
                return;

            string line = Chance.Pick(lines);

            if (string.IsNullOrEmpty(line) || Announcer.CalculateDuration(line, new CassiePlaybackModifiers()) <= 0d)
                return;

            Announcer.GlitchyMessage(line, Config.CassieGlitchChance, Config.CassieJamChance);
            Audit(player, $"a declenche une annonce par {sourceKey}");
        }

        private static void Audit(Player player, string action)
        {
            Logger.Info($"[SCP500s] {player.Nickname} ({player.UserId}) : {action}");
        }
    }
}
