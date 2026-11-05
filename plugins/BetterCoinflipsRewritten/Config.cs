using AugatonLib.Hints;
using System.Collections.Generic;
using System.ComponentModel;

namespace BetterCoinflipsRewritten
{
    public sealed class Config : IHintSettings
    {
        [Description("Whether additional debug logs should be displayed.")]
        public bool Debug { get; set; } = false;

        [Description("Whether every coin flip is written to the server log. Costs one disk write per flip.")]
        public bool LogFlips { get; set; } = false;

        [Description("Server-wide cooldown in seconds for facility-wide effects such as lights out, so chained flips cannot keep the facility dark.")]
        public float FacilityEffectCooldown { get; set; } = 60f;

        [Description("Cooldown between coin flips in seconds.")]
        public float CoinCooldown { get; set; } = 5f;

        [Description("Whether the coin is destroyed after being flipped. This is the main anti-abuse measure: one coin equals one flip.")]
        public bool ConsumeCoinOnFlip { get; set; } = true;

        [Description("Delay in seconds before the flipped coin is destroyed. Removing it during the event itself breaks the client flip animation, so keep it above one second.")]
        public float CoinConsumeDelay { get; set; } = 1.5f;

        [Description("Duration of the effect message in seconds.")]
        public float EffectMessageDuration { get; set; } = 5f;

        [Description("Duration of the cooldown message in seconds.")]
        public float CooldownMessageDuration { get; set; } = 3f;


        [Description("Vertical position of this plugin hint inside HintServiceMeow. Must differ from other plugins.")]
        public float HintYCoordinate { get; set; } = 750f;

        [Description("Font size of this plugin hint.")]
        public int HintFontSize { get; set; } = 20;

        [Description("Weight of the random keycard effect.")]
        public int KeycardChance { get; set; } = 20;

        [Description("Weight of the medical kit effect.")]
        public int MedicalKitChance { get; set; } = 35;

        [Description("Weight of the teleport-to-escape effect.")]
        public int TpToEscapeChance { get; set; } = 5;

        [Description("Weight of the healing effect.")]
        public int HealChance { get; set; } = 10;

        [Description("Weight of the maximum health increase effect.")]
        public int MoreHpChance { get; set; } = 10;

        [Description("Weight of the SCP-268 reward effect.")]
        public int HatChance { get; set; } = 10;

        [Description("Weight of the one-ammo Logicer reward effect.")]
        public int OneAmmoLogicerChance { get; set; } = 1;

        [Description("Weight of the SCP-2176 reward effect.")]
        public int LightbulbChance { get; set; } = 15;

        [Description("Weight of the pink candy reward effect.")]
        public int PinkCandyChance { get; set; } = 10;

        [Description("Weight of the random player size effect.")]
        public int SizeChangeChance { get; set; } = 20;

        [Description("Weight of the artificial health shield effect.")]
        public int AhpShieldChance { get; set; } = 12;

        [Description("Amount of artificial health granted by the shield effect.")]
        public float AhpShieldAmount { get; set; } = 40f;

        [Description("Weight of the ghost effect.")]
        public int GhostlyChance { get; set; } = 8;

        [Description("Duration of the ghost effect in seconds.")]
        public float GhostlyDuration { get; set; } = 8f;

        [Description("Weight of the sugar rush effect.")]
        public int SugarRushChance { get; set; } = 12;

        [Description("Duration of the sugar rush effect in seconds.")]
        public float SugarRushDuration { get; set; } = 15f;

        [Description("Weight of the effect that hands two coins back. It is the only way to end a flip with more coins than you started with, so keep its weight low.")]
        public int DoubleCoinChance { get; set; } = 4;

        [Description("Number of inventory slots before an effect drops its reward on the ground instead of adding it to the inventory.")]
        public int MaxInventorySlots { get; set; } = 8;

        [Description("Amount of health restored by the healing effect.")]
        public float HealAmount { get; set; } = 25f;

        [Description("Percentage by which the player's maximum health is increased.")]
        public float MoreHpPercent { get; set; } = 10f;

        [Description("Weight of the Janitor Keycard in the random keycard pool.")]
        public int JanitorKeycardChance { get; set; } = 40;

        [Description("Weight of the Scientist Keycard in the random keycard pool.")]
        public int ScientistKeycardChance { get; set; } = 25;

        [Description("Weight of the Zone Manager Keycard in the random keycard pool.")]
        public int ZoneManagerKeycardChance { get; set; } = 18;

        [Description("Weight of the Facility Guard Keycard in the random keycard pool.")]
        public int GuardKeycardChance { get; set; } = 10;

        [Description("Weight of the Research Coordinator Keycard in the random keycard pool.")]
        public int ResearchCoordinatorKeycardChance { get; set; } = 6;

        [Description("Weight of the Containment Engineer Keycard in the random keycard pool.")]
        public int ContainmentEngineerKeycardChance { get; set; } = 1;

        [Description("Weight of the current health reduction effect.")]
        public int HpReductionChance { get; set; } = 20;

        [Description("Percentage of the player's current health removed by the effect.")]
        public float HpReductionPercent { get; set; } = 30f;

        [Description("Weight of the teleport-to-Class-D-cells effect.")]
        public int TpToClassDCellsChance { get; set; } = 5;

        [Description("Weight of the facility lights-out effect.")]
        public int LightsOutChance { get; set; } = 20;

        [Description("Duration of the lights-out effect in seconds.")]
        public float LightsOutDuration { get; set; } = 10f;

        [Description("Weight of the active flashbang-at-player effect.")]
        public int TrollFlashChance { get; set; } = 50;

        [Description("Fuse time of the flashbang in seconds.")]
        public float TrollFlashFuseTime { get; set; } = 0.8f;

        [Description("Weight of the effect that leaves the player with 1 HP.")]
        public int OneHpLeftChance { get; set; } = 15;

        [Description("Weight of the inventory reset effect.")]
        public int InventoryResetChance { get; set; } = 20;

        [Description("Weight of the instant grenade effect.")]
        public int InstantExplosionChance { get; set; } = 10;

        [Description("Fuse time of the instant grenade in seconds.")]
        public float InstantExplosionFuseTime { get; set; } = 0.1f;

        [Description("Weight of the random room teleport effect.")]
        public int RandomTeleportChance { get; set; } = 15;

        [Description("Weight of the drunk effect.")]
        public int DrunkChance { get; set; } = 15;

        [Description("Duration of the drunk effect in seconds.")]
        public float DrunkDuration { get; set; } = 20f;

        [Description("Weight of the prismatic trip effect.")]
        public int PrismaticTripChance { get; set; } = 15;

        [Description("Duration of the prismatic trip effect in seconds.")]
        public float PrismaticTripDuration { get; set; } = 12f;

        [Description("Weight of the amnesia effect.")]
        public int AmnesiaChance { get; set; } = 15;

        [Description("Duration of the amnesia effect in seconds.")]
        public float AmnesiaDuration { get; set; } = 10f;

        [Description("Weight of the sky launch effect.")]
        public int SkyLaunchChance { get; set; } = 8;

        [Description("Height in metres the player is thrown up by the sky launch effect. Landing damage is the point of the effect.")]
        public float SkyLaunchHeight { get; set; } = 25f;

        [Description("Weight of the position swap effect. It moves a second player, so it stays rare on purpose.")]
        public int PositionSwapChance { get; set; } = 8;

        [Description("Weight of the fake warhead effect. Visual only, it deals no damage.")]
        public int FakeWarheadChance { get; set; } = 12;

        [Description("Duration in seconds of the black screen that follows the fake warhead effect.")]
        public float FakeWarheadDimDuration { get; set; } = 3f;

        [Description("Weight of the SCP-173 tantrum trap effect.")]
        public int TantrumTrapChance { get; set; } = 15;

        [Description("Weight of the petrification effect.")]
        public int PetrifiedChance { get; set; } = 10;

        [Description("Duration of the petrification effect in seconds. The player cannot move during that time, so keep it short.")]
        public float PetrifiedDuration { get; set; } = 4f;

        [Description("Weight of the disco lights effect. Facility-wide, so it goes through facility_effect_cooldown.")]
        public int DiscoLightsChance { get; set; } = 10;

        [Description("Duration of the disco lights effect in seconds.")]
        public float DiscoLightsDuration { get; set; } = 20f;

        [Description("Interval in seconds between two colour changes of the disco lights effect.")]
        public float DiscoLightsInterval { get; set; } = 1.5f;

        [Description("Weight of the mice invasion effect. Facility-wide, so it goes through facility_effect_cooldown.")]
        public int MiceInvasionChance { get; set; } = 8;

        [Description("Type of mice spawned in the shelter by the mice invasion effect.")]
        public byte MiceInvasionType { get; set; } = 1;

        [Description("Weight of the mocking C.A.S.S.I.E. announcement effect. Facility-wide, so it goes through facility_effect_cooldown.")]
        public int CassieMockChance { get; set; } = 8;

        [Description("Announcements the mocking C.A.S.S.I.E. effect can play. A line the game refuses is reported in the server log on the first coin flip of the session and never played.")]
        public List<string> CassieMockLines { get; set; } = new List<string>
        {
            "ATTENTION ALL PERSONNEL . AN UNKNOWN OBJECT HAS BEEN DETECTED IN LIGHT CONTAINMENT ZONE",
            "WARNING . CONTAINMENT FAILURE DETECTED . ALL REMAINING PERSONNEL ARE ADVISED TO EVACUATE IMMEDIATELY",
            "ATTENTION ALL PERSONNEL . SEVERAL SUBJECTS ARE IN DANGER . GOOD LUCK",
        };

        [Description("Probability that the mocking announcement glitches (0-1).")]
        public float CassieMockGlitchChance { get; set; } = 0.2f;

        [Description("Probability that the mocking announcement is jammed (0-1).")]
        public float CassieMockJamChance { get; set; } = 0.1f;

        [Description("Weight of the harmless fake explosion effect.")]
        public int FakeExplosionChance { get; set; } = 12;

        [Description("Duration in seconds of the deafness and concussion left by the fake explosion effect.")]
        public float FakeExplosionStunDuration { get; set; } = 5f;

        [Description("Whether this plugin replaces map loot with coins. Leave false when SCP500s owns loot replacement, otherwise both plugins compete for the same pickups and neither ratio holds.")]
        public bool SpawnCoinsOnMap { get; set; } = false;

        [Description("Number of coins to spawn across the map.")]
        public int MapCoinAmount { get; set; } = 6;

        [Description("Delay before spawning coins after the round starts, in seconds.")]
        public float CoinSpawnDelay { get; set; } = 4f;

        [Description("Whether medkits can be replaced with coins.")]
        public bool ReplaceMedkitsWithCoins { get; set; } = true;

        [Description("Whether painkillers can be replaced with coins.")]
        public bool ReplacePainkillersWithCoins { get; set; } = true;

        [Description("Whether low-level keycards can be replaced with coins.")]
        public bool ReplaceWeakKeycardsWithCoins { get; set; } = true;

        [Description("Whether flashlights can be replaced with coins.")]
        public bool ReplaceFlashlightsWithCoins { get; set; } = true;

        [Description("Whether an extra coin is dropped next to some map loot. Unlike spawn_coins_on_map this destroys nothing, so it can run alongside SCP500s without either plugin fighting for the same pickups.")]
        public bool AddBonusCoinsToLoot { get; set; } = true;

        [Description("Number of extra coins added across the map. Coins are consumed on use, so this is what keeps them available.")]
        public int BonusCoinAmount { get; set; } = 10;

        [Description("Item types whose spawn pool gets an extra coin. One coin is dropped next to a pickup of that type, the pickup itself is left alone.")]
        public List<ItemType> BonusCoinSourceItems { get; set; } = new List<ItemType>
        {
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Flashlight,
            ItemType.Radio,
            ItemType.KeycardJanitor,
            ItemType.KeycardScientist,
        };

        [Description("Maximum number of extra coins dropped next to pickups of the same item type. Prevents every coin from landing in the same loot pool.")]
        public int BonusCoinsPerSourceType { get; set; } = 3;
    }
}