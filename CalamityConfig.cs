using CalamityMod.UI;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace CalamityMod
{
	[Label("Config")]
	[BackgroundColor(49, 32, 36, 216)]
	public class CalamityConfig : ModConfig
	{
		public static CalamityConfig Instance;
		public override ConfigScope Mode => ConfigScope.ClientSide;

		// Clamps values that would cause ugly problems if loaded directly without sanitization.
		[OnDeserialized]
		internal void ClampValues(StreamingContext context)
		{
			BossHealthBoost = Utils.Clamp(BossHealthBoost, MinBossHealthBoost, MaxBossHealthBoost);
			MeterShake = Utils.Clamp(MeterShake, MinMeterShake, MaxMeterShake);
			DeathWeatherMultiplier = Utils.Clamp(DeathWeatherMultiplier, MinWeatherMultiplier, MaxWeatherMultiplier);
		}

		[Header("Graphics Changes")]

		[Label("Afterimages")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables rendering afterimages for Calamity NPCs, projectiles, etc.\nDisable to improve performance.")]
		public bool Afterimages { get; set; }

		[Label("Stealth Invisibility")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables players gradually turning invisible as their Rogue Stealth increases.\nThis effect is visually similar to Shroomite armor's stealth.")]
		public bool StealthInvisbility { get; set; }

		[Label("New Shop Inventory Alert")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds an icon that appears over Town NPCs when they have new items in their shops.")]
		public bool ShopNewAlert { get; set; }

		[Header("UI Changes")]

		[Label("Boss Health Bars")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables Calamity's boss health bar in the bottom right corner of the screen.")]
		public bool BossHealthBar { get; set; }

		[Label("Boss Health Bar Extra Info")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds extra info to the Calamity boss health bar.\nThis displays either the boss's exact health or number of remaining parts or segments.")]
		public bool BossHealthBarExtraInfo { get; set; }

		[Label("Boss and Miniboss Debuff Display")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds an array of debuff icons above all bosses and minibosses.")]
		public bool DebuffDisplay { get; set; }

		[BackgroundColor(192, 54, 64, 192)]
		[Label("Lock Meter Positions")]
		[DefaultValue(true)]
		[Tooltip("Prevents clicking on the Stealth, Charge, Rage, and Adrenaline Meters.\nThis stops them from being dragged around with the mouse.")]
		public bool MeterPosLock { get; set; }

		[Label("Stealth Meter")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables the Stealth Meter UI, which shows the player's current stealth level.\nThe Stealth Meter is always hidden if not wearing Rogue armor.")]
		public bool StealthBar { get; set; }

		[Label("Stealth Meter X Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(820f)]
		[Tooltip("The X position of the Stealth Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float StealthMeterPosX { get; set; }

		[Label("Stealth Meter Y Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(43f)]
		[Tooltip("The Y position of the Stealth Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float StealthMeterPosY { get; set; }

		[Label("Charge Meter")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables the Charge Meter UI, which shows the charge level of the player's currently held Arsenal item.\nThe Charge Meter is always hidden if not holding a chargable arsenal item.")]
		public bool ChargeMeter { get; set; }

		[Label("Charge Meter X Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(950f)]
		[Tooltip("The X position of the Charge Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float ChargeMeterPosX { get; set; }

		[Label("Charge Meter Y Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(43f)]
		[Tooltip("The Y position of the Charge Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float ChargeMeterPosY { get; set; }

		[Header("General Gameplay Changes")]

		[Label("Lethal Lava")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Makes lava significantly more deadly by adding a new debuff.\nPermanent lava immunity does not provide immunity to this debuff.\nThis setting is ignored in Death Mode, which always has Lethal Lava.")]
		public bool LethalLava { get; set; }

		[Label("Proficiency")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables the Proficiency system which allows the player to gain slight stat bonuses by persistently using one damage class.\nDisabling the system does not remove levels players already have, but disables their stat bonuses and prevents experience gain.")]
		public bool Proficiency { get; set; }

		[Label("Sell Vanilla Boss Summons")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds vanilla boss summons to NPC shops after the corresponding boss is defeated.\nThis does not affect Calamity's boss summons, which are always sold.\nTo apply changes, close and reopen the shop.")]
		public bool SellVanillaSummons { get; set; }

		[Label("Boost Mining Speed")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Increases the player's mining speed by 75%.\nThis does not affect drills.")]
		public bool MiningSpeedBoost { get; set; }

		[Label("Boss Zen")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("While a boss is alive, all players receive the Boss Zen buff which drastically reduces enemy spawn rates.")]
		public bool BossZen { get; set; }

		[Label("Never Weaken Reactive Boss DR")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Sets Reactive Boss DR to always be full strength, even if the boss has already been defeated.\nIf disabled, the effect is only 66% as powerful after the boss has been defeated.\n\nReactive Boss DR makes bosses smoothly take less damage if they are being killed very quickly.\nIn most cases, the system has no noticeable effect.")]
		public bool FullPowerReactiveBossDR { get; set; }

		[Label("Let Town NPCs spawn at night.")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Allows you to determine if town NPCs (including the Old Man) can spawn at night.")]
		public bool CanTownNPCsSpawnAtNight { get; set; }

		private const int MinTownNPCSpawnMultiplier = 1;
		private const int MaxTownNPCSpawnMultiplier = 10;

		[Label("Town NPC Spawn Rate Multiplier")]
		[BackgroundColor(192, 54, 64, 192)]
		[Range(MinTownNPCSpawnMultiplier, MaxTownNPCSpawnMultiplier)]
		[Increment(1)]
		[DrawTicks]
		[DefaultValue(MinTownNPCSpawnMultiplier)]
		[Tooltip("Makes town NPCs spawn more quickly, the higher this value is.")]
		public int TownNPCSpawnRateMultiplier { get; set; }

		private const float MinBossHealthBoost = 0f;
		private const float MaxBossHealthBoost = 900f;

		[Label("Boss Health Percentage Boost")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(MinBossHealthBoost, MaxBossHealthBoost)]
		[Increment(50f)]
		[DrawTicks]
		[DefaultValue(MinBossHealthBoost)]
		[Tooltip("Globally boosts the health of all bosses.\nDoes not affect bosses that are already spawned.")]
		public float BossHealthBoost { get; set; }

		[Header("Expert Mode Changes")]

		[Label("Reduce Expert Debuff Durations")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Disables Expert Mode doubling the duration of all debuffs inflicted on the player.\nCalamity is balanced with the assumption that this setting is enabled.")]
		public bool NerfExpertDebuffs { get; set; }

		[Label("Rework Chilled Water")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("When enabled, water in the Snow and Ice biomes will rapidly drain the player's breath instead of inflicting Chilled.")]
		public bool ReworkChilledWater { get; set; }

		[Label("Reduce Celestial Pillar Kill Count")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Reduces the kills required to destroy a Celestial Pillar shield in Expert Mode from 150 to 100.\nThis makes the value equivalent to Normal Mode.")]
		public bool NerfExpertPillars { get; set; }

		[Label("Disable Expert Enemy Spawns in Towns")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Counteracts Expert Mode allowing enemies to spawn near towns by vastly decreasing spawn rates.\nThis can have unintended side effects such as making critters difficult to find.")]
		public bool DisableExpertTownSpawns { get; set; }

		[Header("Revengeance Mode Changes")]

		[Label("Rage and Adrenaline")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables Rage and Adrenaline, the two Revengeance Mode mechanics.")]
		public bool Rippers { get; set; }

		private const float MinMeterShake = 0f;
		private const float MaxMeterShake = 4f;

		[Label("Rage and Adrenaline Meter Shake")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(MinMeterShake, MaxMeterShake)]
		[Increment(1f)]
		[DrawTicks]
		[DefaultValue(2f)]
		[Tooltip("How much the Rage and Adrenaline Meters shake while in use.\nSet to zero to disable the shaking entirely.")]
		public float MeterShake { get; set; }

		[Label("Rage Meter X Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(RipperUI.DefaultRagePosX)]
		[Tooltip("The X position of the Rage Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float RageMeterPosX { get; set; }

		[Label("Rage Meter Y Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(RipperUI.DefaultRagePosY)]
		[Tooltip("The Y position of the Rage Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float RageMeterPosY { get; set; }

		[Label("Adrenaline Meter X Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(RipperUI.DefaultAdrenPosX)]
		[Tooltip("The X position of the Adrenaline Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float AdrenalineMeterPosX { get; set; }

		[Label("Adrenaline Meter Y Position")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(RipperUI.DefaultAdrenPosY)]
		[Tooltip("The Y position of the Adrenaline Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float AdrenalineMeterPosY { get; set; }

		[Header("Death Mode Changes")]

		private const float MinWeatherMultiplier = 0.5f;
		private const float MaxWeatherMultiplier = 3f;

		[Label("Weather Hazard Delay Multiplier")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(MinWeatherMultiplier, MaxWeatherMultiplier)]
		[Increment(0.25f)]
		[DrawTicks]
		[DefaultValue(1f)]
		[Tooltip("Adjusts the delay between Death Mode weather hazards such as lightning.\nDecreasing this value makes hazards more frequent.\nIncreasing this value makes hazards less frequent.")]
		public float DeathWeatherMultiplier { get; set; }

		[Header("Boss Rush Curses")]

		[Label("Accessory Curse")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Limits the player to five accessories during the Boss Rush.")]
		public bool BossRushAccessoryCurse { get; set; }

		[Label("Health Curse")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Disables all health regeneration during the Boss Rush.")]
		public bool BossRushHealthCurse { get; set; }

		[Label("Dash Curse")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Disables all dashes during the Boss Rush.")]
		public bool BossRushDashCurse { get; set; }

		[Label("Immunity Frame Curse")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("During the Boss Rush, being hit twice within three seconds will cause instant death.\nThis effect ignores revives.")]
		public bool BossRushImmunityFrameCurse { get; set; }

		[Label("Xeroc Curse")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Permanently enrages every boss in the Boss Rush.\nThis enrage is equivalent to that provided by Demonshade armor.")]
		public bool BossRushXerocCurse { get; set; }

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) => true;
	}
}