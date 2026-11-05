using System;
using System.Collections.Generic;
using System.Linq;
using AugatonLib.Arbitration;
using AugatonLib.Bus;
using AugatonLib.Effects;
using AugatonLib.Runtime;
using AugatonLib.Text;
using CustomPlayerEffects;
using InventorySystem.Disarming;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using PlayerStatsSystem;
using Replacer.API;

namespace Replacer.Handlers
{
    public sealed class PlayerHandlers
    {
        private const float PendingTickSeconds = 1f;

        private readonly Plugin plugin;
        private readonly SpectatorQueue queue;
        private readonly List<CoroutineHandle> pendingSnapshots = new List<CoroutineHandle>(4);
        private readonly List<PendingRole> pendingRoles = new List<PendingRole>(4);

        private CoroutineHandle pendingWatcher;
        private bool watchingPendingRoles;

        public PlayerHandlers(Plugin plugin, SpectatorQueue queue)
        {
            this.plugin = plugin;
            this.queue = queue;
        }

        public void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            try
            {
                if (ev?.Player is null)
                    return;

                if (ev.NewRole == RoleTypeId.Spectator)
                    queue.MarkSpectating(ev.Player);
                else
                    queue.MarkAlive(ev.Player);
            }
            catch (Exception e)
            {
                Logger.Error($"OnChangingRole: {e}");
            }
        }

        public void OnDeparting(Player leaver)
        {
            try
            {
                queue.Remove(leaver);
                plugin.Hints.Remove(leaver);

                if (!IsReplaceable(leaver))
                    return;

                if (!DepartureArbiter.IsOwner(leaver, Plugin.DepartureOwner))
                    return;

                PlayerSnapshot snapshot = CaptureSnapshot(leaver);
                Player replacement = queue.Pick(leaver, plugin.Config);

                if (replacement is null)
                {
                    Defer(leaver, snapshot);
                    return;
                }

                queue.MarkReplaced(replacement);
                ApplySnapshot(replacement, snapshot);

                if (plugin.Config.TransferInventory && !leaver.IsSCP && replacement.Role == snapshot.Role)
                    TransferInventory(leaver, replacement);
            }
            catch (Exception e)
            {
                Logger.Error($"OnDeparting: {e}");
            }
        }

        private bool IsReplaceable(Player player)
        {
            if (player.IsDummy || !Round.IsRoundInProgress)
                return false;

            if (plugin.Config.PauseDuringGamemodes && WorldEvents.IsPaused)
                return false;

            if (plugin.Config.MaxRoundSeconds > 0f && Round.Duration.TotalSeconds > plugin.Config.MaxRoundSeconds)
                return false;

            int remainingPlayers = CountRealPlayers() - 1;

            if (remainingPlayers < plugin.Config.MinimumPlayers)
                return false;

            RoleTypeId role = player.Role;

            if (role == RoleTypeId.None || role == RoleTypeId.Spectator || player.Team == Team.Dead)
                return false;

            if (plugin.Config.IgnoredRoles.Contains(role))
                return false;

            if (plugin.Config.ReplaceableRoles.Count > 0 && !plugin.Config.ReplaceableRoles.Contains(role))
                return false;

            return true;
        }

        private PlayerSnapshot CaptureSnapshot(Player player)
        {
            PlayerSnapshot snapshot = new PlayerSnapshot
            {
                Nickname = player.Nickname,
                Role = player.Role,
                Position = player.Position,
                Rotation = player.Rotation,
                Health = player.Health,
                MaxHealth = player.MaxHealth,
                HumeShield = player.HumeShield,
                Team = player.Team,
                Size = ScaleArbiter.BaselineOf(player),
                ArtificialHealth = player.ArtificialHealth,
                IsDisarmed = player.IsDisarmed,
                Disarmer = player.DisarmedBy,
            };

            if (plugin.Config.TransferCuffs && plugin.Config.TransferPosition)
                CaptureCaptives(player, snapshot);

            if (plugin.Config.TransferEffects)
            {
                foreach (StatusEffectBase effect in player.ActiveEffects)
                {
                    if (effect is null || !effect.IsEnabled)
                        continue;

                    snapshot.Effects.Add(new EffectSnapshot(effect.GetType().Name, effect.Intensity, effect.Duration == 0f ? 0f : effect.TimeLeft));
                }
            }

            if (player.RoleBase is Scp079Role scp079
                && scp079.SubroutineModule.TryGetSubroutine(out Scp079TierManager tier)
                && scp079.SubroutineModule.TryGetSubroutine(out Scp079AuxManager aux))
            {
                snapshot.IsScp079 = true;
                snapshot.Scp079Experience = tier.TotalExp;
                snapshot.Scp079Energy = aux.CurrentAux;
            }

            return snapshot;
        }

        private void ApplySnapshot(Player replacement, PlayerSnapshot snapshot)
        {
            string userId = replacement.UserId;

            RoleSpawnFlags flags = RoleSpawnFlags.None;

            if (!plugin.Config.TransferPosition)
                flags |= RoleSpawnFlags.UseSpawnpoint;

            // Un remplacant differe arrive apres coup : les objets du joueur parti l'attendent au sol.
            if (!plugin.Config.TransferInventory || (snapshot.IsDeferred && !plugin.Config.TransferPosition))
                flags |= RoleSpawnFlags.AssignInventory;

            replacement.SetRole(snapshot.Role, RoleChangeReason.Respawn, flags);

            pendingSnapshots.Add(Timing.CallDelayed(plugin.Config.ReplacementDelay, () =>
            {
                try
                {
                    Player target = Player.Get(userId);

                    if (target is null || target.IsDestroyed || target.Role != snapshot.Role)
                        return;

                    if (plugin.Config.TransferPosition)
                    {
                        target.Position = snapshot.Position;
                        target.Rotation = snapshot.Rotation;
                    }

                    if (plugin.Config.TransferHealth)
                    {
                        target.MaxHealth = snapshot.MaxHealth;
                        target.Health = Math.Min(snapshot.Health, snapshot.MaxHealth);
                        target.HumeShield = snapshot.HumeShield;
                        RestoreArtificialHealth(target, snapshot.ArtificialHealth);
                    }

                    if (plugin.Config.TransferCuffs && snapshot.IsDisarmed)
                        RestoreCuffs(target, snapshot.Disarmer);

                    RestoreCaptives(target, snapshot);

                    if (plugin.Config.TransferEffects)
                    {
                        foreach (EffectSnapshot effect in snapshot.Effects)
                        {
                            if (EffectLookup.TryGet(target, effect.Name, out StatusEffectBase status))
                                target.EnableEffect(status, effect.Intensity, effect.Duration);
                        }
                    }

                    if (snapshot.IsScp079
                        && target.RoleBase is Scp079Role scp079
                        && scp079.SubroutineModule.TryGetSubroutine(out Scp079TierManager tier)
                        && scp079.SubroutineModule.TryGetSubroutine(out Scp079AuxManager aux))
                    {
                        tier.TotalExp = snapshot.Scp079Experience;
                        aux.CurrentAux = snapshot.Scp079Energy;
                    }

                    Announce(target, snapshot);

                    PluginBus.Publish(BusTopics.PlayerReplaced, Plugin.DepartureOwner, new PlayerReplacement(target, snapshot.Size));

                    Logger.Info($"{target.Nickname} ({target.UserId}) remplace {snapshot.Nickname} en tant que {snapshot.Role}.");
                }
                catch (Exception e)
                {
                    Logger.Error($"ApplySnapshot: {e}");
                }
            }));
        }

        // Passer par un pickup reprend le chemin du ramassage : le numero de serie survit, et avec lui
        // les accessoires, le chargeur, l'usure et le suivi des objets personnalises d'autres plugins.
        private static void TransferInventory(Player leaver, Player target)
        {
            target.ClearInventory();

            foreach (Item item in leaver.Items.ToArray())
                TransferItem(leaver, target, item);

            foreach (KeyValuePair<ItemType, ushort> ammo in leaver.Ammo)
                target.SetAmmo(ammo.Key, ammo.Value);

            leaver.ClearInventory();
        }

        private static void TransferItem(Player leaver, Player target, Item item)
        {
            if (item is null)
                return;

            ItemType type = item.Type;
            Pickup pickup = leaver.DropItem(item);

            if (pickup is null)
            {
                target.AddItem(type);
                return;
            }

            if (target.AddItem(pickup) is not null)
                pickup.Destroy();
        }

        private static void RestoreArtificialHealth(Player target, float amount)
        {
            if (amount <= 0f)
                return;

            float limit = Math.Max(amount, target.MaxArtificialHealth);

            target.CreateAhpProcess(amount, limit, AhpStat.DefaultDecay, AhpStat.DefaultEfficacy, 0f, false);
        }

        private static void RestoreCuffs(Player target, Player disarmer)
        {
            if (disarmer is null)
                target.IsDisarmed = true;
            else if (!disarmer.IsDestroyed)
                target.DisarmedBy = disarmer;
        }

        private static void CaptureCaptives(Player guard, PlayerSnapshot snapshot)
        {
            foreach (DisarmedPlayers.DisarmedEntry entry in DisarmedPlayers.Entries)
            {
                if (entry.Disarmer == guard.NetworkId && Player.TryGet(entry.DisarmedPlayer, out Player captive))
                    snapshot.Captives.Add(captive);
            }
        }

        // Le depart du gardien libere ses prisonniers le temps que le remplacant soit en place.
        private static void RestoreCaptives(Player target, PlayerSnapshot snapshot)
        {
            if (snapshot.IsDeferred)
                return;

            foreach (Player captive in snapshot.Captives)
            {
                if (!captive.IsDestroyed && captive.IsAlive && !captive.IsDisarmed)
                    captive.DisarmedBy = target;
            }
        }

        private void Defer(Player leaver, PlayerSnapshot snapshot)
        {
            float seconds = plugin.Config.PendingRoleSeconds;

            if (seconds <= 0f)
            {
                Logger.Debug($"Aucun spectateur eligible pour remplacer {snapshot.Nickname}.", plugin.Config.Debug);
                return;
            }

            if (plugin.Config.TransferInventory && !leaver.IsSCP)
                leaver.DropEverything();

            snapshot.IsDeferred = true;
            pendingRoles.Add(new PendingRole(snapshot, UnityEngine.Time.realtimeSinceStartup + seconds));
            GenocideArbiter.Hold(Plugin.DepartureOwner, snapshot.Team, seconds + PendingTickSeconds);

            if (watchingPendingRoles)
                return;

            watchingPendingRoles = true;
            pendingWatcher = Timing.RunCoroutine(WatchPendingRoles());
        }

        private IEnumerator<float> WatchPendingRoles()
        {
            while (pendingRoles.Count > 0)
            {
                yield return Timing.WaitForSeconds(PendingTickSeconds);

                try
                {
                    AssignPendingRoles();
                }
                catch (Exception e)
                {
                    Logger.Error($"AssignPendingRoles: {e}");
                }
            }

            watchingPendingRoles = false;
        }

        private void AssignPendingRoles()
        {
            float now = UnityEngine.Time.realtimeSinceStartup;
            bool inProgress = Round.IsRoundInProgress && !(plugin.Config.PauseDuringGamemodes && WorldEvents.IsPaused);

            for (int i = 0; i < pendingRoles.Count;)
            {
                PendingRole pending = pendingRoles[i];
                bool open = inProgress && now < pending.ExpiresAt;
                Player replacement = open ? queue.Pick(null, plugin.Config) : null;

                if (open && replacement is null)
                {
                    i++;
                    continue;
                }

                pendingRoles.RemoveAt(i);

                if (replacement is not null)
                {
                    queue.MarkReplaced(replacement);
                    ApplySnapshot(replacement, pending.Snapshot);
                }

                ReleaseTeamHold(pending.Snapshot.Team);
            }
        }

        private void ReleaseTeamHold(Team team)
        {
            foreach (PendingRole other in pendingRoles)
            {
                if (other.Snapshot.Team == team)
                    return;
            }

            GenocideArbiter.Release(Plugin.DepartureOwner, team);
            PluginBus.Publish(BusTopics.TeamCheckRequested, Plugin.DepartureOwner, team);
        }

        private static int CountRealPlayers()
        {
            int count = 0;

            foreach (Player player in Player.ReadyList)
            {
                if (!player.IsDummy)
                    count++;
            }

            return count;
        }

        public void Reset()
        {
            foreach (CoroutineHandle handle in pendingSnapshots)
                Timing.KillCoroutines(handle);

            pendingSnapshots.Clear();

            if (watchingPendingRoles)
                Timing.KillCoroutines(pendingWatcher);

            watchingPendingRoles = false;

            foreach (PendingRole pending in pendingRoles)
                GenocideArbiter.Release(Plugin.DepartureOwner, pending.Snapshot.Team);

            pendingRoles.Clear();
        }

        private void Announce(Player target, PlayerSnapshot snapshot)
        {
            string nickname = SafeText.Sanitize(snapshot.Nickname, SafeText.DefaultMaxLength, plugin.Translation.UnknownNickname);

            string hint = plugin.Translation.ReplacedHint
                .Replace("{nickname}", nickname)
                .Replace("{role}", RoleNames.Full(snapshot.Role));

            plugin.Hints.Show(target, hint, plugin.Config.HintDuration);

            if (!plugin.Config.AnnounceToServer)
                return;

            Server.SendBroadcast(plugin.Translation.ReplacedBroadcast, plugin.Config.BroadcastDuration);
        }
    }
}
