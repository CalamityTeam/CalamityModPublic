using CalamityMod.UI;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
		}

		[Header("Graphics Changes")]

		[Label("$Mods.CalamityMod.Afterimages")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables rendering afterimages for Calamity NPCs, projectiles, etc.\nDisable to improve performance.")]
		public bool Afterimages { get; set; }

		[Label("$Mods.CalamityMod.MaxParticles")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0, 1000)]
		[DefaultValue(500)]
		[Tooltip("Sets the maximum of particle effects that can exist at once.\nParticles are separate from dust and gores.\nTurn down to improve performance.")]
		public int ParticleLimit { get; set; }

		[Label("$Mods.CalamityMod.ScreenshakeOff")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Disables all screen-shaking effects.")]
		public bool DisableScreenShakes { get; set; }

		[Label("$Mods.CalamityMod.StealthInvisibility")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables players gradually turning invisible as their Rogue Stealth increases.\nThis effect is visually similar to Shroomite armor's stealth.")]
		public bool StealthInvisbility { get; set; }

		[Label("$Mods.CalamityMod.ShopAlert")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds an icon that appears over Town NPCs when they have new items in their shops.")]
		public bool ShopNewAlert { get; set; }

		[Header("UI Changes")]

		[Label("$Mods.CalamityMod.BossHealthBar")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables Calamity's boss health bar in the bottom right corner of the screen.")]
		public bool BossHealthBar { get; set; }

		[Label("$Mods.CalamityMod.BossHealthBarExtra")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds extra info to the Calamity boss health bar.\nThis displays either the boss's exact health or number of remaining parts or segments.")]
		public bool BossHealthBarExtraInfo { get; set; }

		[Label("$Mods.CalamityMod.DebuffDisplay")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds an array of debuff icons above all bosses and minibosses.")]
		public bool DebuffDisplay { get; set; }

		[Label("$Mods.CalamityMod.CooldownDisplay")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f,2f)]
		[DefaultValue(2f)]
		[Increment(1f)]
		[DrawTicks]
		[Tooltip("Displays all the important cooldowns on your UI under your buffs and debuffs. Set this to 1 to have it display in a more compact way, and to 0 to entirely disable the UI")]
		public float CooldownDisplay { get; set; }

		[Label("$Mods.CalamityMod.VanillaCooldownDisplay")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Adds custom cooldown displays for Chaos State and Potion Sickness.\nThis doesn't remove them from your buff list, but can help with visibility nontheless")]
		public bool VanillaCooldownDisplay { get; set; }

		[BackgroundColor(192, 54, 64, 192)]
		[Label("$Mods.CalamityMod.MeterLock")]
		[DefaultValue(true)]
		[Tooltip("Prevents clicking on the Stealth, Charge, Rage, and Adrenaline Meters.\nThis stops them from being dragged around with the mouse.")]
		public bool MeterPosLock { get; set; }

		[Label("$Mods.CalamityMod.StealthMeter")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables the Stealth Meter UI, which shows the player's current stealth level.\nThe Stealth Meter is always hidden if not wearing Rogue armor.")]
		public bool StealthBar { get; set; }

		[Label("$Mods.CalamityMod.StealthMeterX")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(820f)]
		[Tooltip("The X position of the Stealth Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float StealthMeterPosX { get; set; }

		[Label("$Mods.CalamityMod.StealthMeterY")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(43f)]
		[Tooltip("The Y position of the Stealth Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float StealthMeterPosY { get; set; }

		[Label("$Mods.CalamityMod.ChargeMeter")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables the Charge Meter UI, which shows the charge level of the player's currently held Arsenal item.\nThe Charge Meter is always hidden if not holding a chargable arsenal item.")]
		public bool ChargeMeter { get; set; }

		[Label("$Mods.CalamityMod.ChargeMeterX")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(950f)]
		[Tooltip("The X position of the Charge Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float ChargeMeterPosX { get; set; }

		[Label("$Mods.CalamityMod.ChargeMeterY")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(43f)]
		[Tooltip("The Y position of the Charge Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float ChargeMeterPosY { get; set; }

		[Label("$Mods.CalamityMod.SpeedrunTimer")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Enables a Speedrun Timer.")]
		public bool SpeedrunTimer { get; set; }

		[Label("$Mods.CalamityMod.SpeedrunTimerX")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(-800f, 800f)]
		[DefaultValue(68f)]
		[Tooltip("The X position of the Speedrun Timer.")]
		public float SpeedrunTimerPosX { get; set; }

		[Label("$Mods.CalamityMod.SpeedrunTimerY")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(16f, 1000f)]
		[DefaultValue(16f)]
		[Tooltip("The Y position of the Speedrun Timer.")]
		public float SpeedrunTimerPosY { get; set; }

		[Header("General Gameplay Changes")]

		[Label("$Mods.CalamityMod.EarlyHMRework")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Demon Altars no longer spawn ores and crimson/corruption blocks when broken.\n" +
			"Wall of Flesh spawns Cobalt and Palladium ore on first kill.\n" +
			"The first mech boss you fight has 20% less HP and damage and spawns Mythril and Orichalcum ore on first kill.\n" +
			"The second mech boss you fight has 10% less HP and damage and spawns Adamantite and Titanium ore on first kill.\n" +
			"The third mech boss spawns Hallowed Ore on first kill")]
		public bool EarlyHardmodeProgressionRework { get; set; }

		[Label("$Mods.CalamityMod.LethalLava")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Makes lava significantly more deadly by adding a new debuff.\nPermanent lava immunity does not provide immunity to this debuff.\nThis setting is ignored in Death Mode, which always has Lethal Lava.")]
		public bool LethalLava { get; set; }

		[Label("$Mods.CalamityMod.Proficiency")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Enables the Proficiency system which allows the player to gain slight stat bonuses by persistently using one damage class.\nDisabling the system does not remove levels players already have, but disables their stat bonuses and prevents experience gain.")]
		public bool Proficiency { get; set; }

		[Label("$Mods.CalamityMod.BossZen")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("While a boss is alive, all players near a boss receive the Boss Effects buff, which drastically reduces enemy spawn rates.")]
		public bool BossZen { get; set; }

		[Label("$Mods.CalamityMod.NPCNightSpawn")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Allows you to determine if town NPCs (including the Old Man) can spawn at night.")]
		public bool CanTownNPCsSpawnAtNight { get; set; }

		private const int MinTownNPCSpawnMultiplier = 1;
		private const int MaxTownNPCSpawnMultiplier = 10;

		[Label("$Mods.CalamityMod.NPCExtraSpawn")]
		[BackgroundColor(192, 54, 64, 192)]
		[Range(MinTownNPCSpawnMultiplier, MaxTownNPCSpawnMultiplier)]
		[Increment(1)]
		[DrawTicks]
		[DefaultValue(MinTownNPCSpawnMultiplier)]
		[Tooltip("Makes town NPCs spawn more quickly, the higher this value is.")]
		public int TownNPCSpawnRateMultiplier { get; set; }

		private const float MinBossHealthBoost = 0f;
		private const float MaxBossHealthBoost = 900f;

		[Label("$Mods.CalamityMod.BossHPBoost")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(MinBossHealthBoost, MaxBossHealthBoost)]
		[Increment(50f)]
		[DrawTicks]
		[DefaultValue(MinBossHealthBoost)]
		[Tooltip("Globally boosts the health of all bosses.\nDoes not affect bosses that are already spawned.")]
		public float BossHealthBoost { get; set; }

		[Header("Expert Mode Changes")]

		[Label("$Mods.CalamityMod.ExpertDebuffReduction")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("Disables Expert Mode doubling the duration of all debuffs inflicted on the player.\nCalamity is balanced with the assumption that this setting is enabled.")]
		public bool NerfExpertDebuffs { get; set; }

		[Label("$Mods.CalamityMod.ChillWaterRework")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(true)]
		[Tooltip("When enabled, water in the Snow and Ice biomes will rapidly drain the player's breath instead of inflicting Chilled.")]
		public bool ReworkChilledWater { get; set; }

		[Label("$Mods.CalamityMod.ExpertSafeTowns")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Counteracts Expert Mode allowing enemies to spawn near towns by vastly decreasing spawn rates.\nThis can have unintended side effects such as making critters difficult to find.")]
		public bool DisableExpertTownSpawns { get; set; }

		[Header("Revengeance Mode Changes")]

		private const float MinMeterShake = 0f;
		private const float MaxMeterShake = 4f;

		[Label("$Mods.CalamityMod.RipperBarShake")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(MinMeterShake, MaxMeterShake)]
		[Increment(1f)]
		[DrawTicks]
		[DefaultValue(2f)]
		[Tooltip("How much the Rage and Adrenaline Meters shake while in use.\nSet to zero to disable the shaking entirely.")]
		public float MeterShake { get; set; }

		[Label("$Mods.CalamityMod.RageX")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(RipperUI.DefaultRagePosX)]
		[Tooltip("The X position of the Rage Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float RageMeterPosX { get; set; }

		[Label("$Mods.CalamityMod.RageY")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(RipperUI.DefaultRagePosY)]
		[Tooltip("The Y position of the Rage Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float RageMeterPosY { get; set; }

		[Label("$Mods.CalamityMod.AdrenalineX")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 3840f)]
		[DefaultValue(RipperUI.DefaultAdrenPosX)]
		[Tooltip("The X position of the Adrenaline Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float AdrenalineMeterPosX { get; set; }

		[Label("$Mods.CalamityMod.AdrenalineY")]
		[BackgroundColor(192, 54, 64, 192)]
		[SliderColor(224, 165, 56, 128)]
		[Range(0f, 2160f)]
		[DefaultValue(RipperUI.DefaultAdrenPosY)]
		[Tooltip("The Y position of the Adrenaline Meter.\nThe meter can be dragged with the mouse if Lock Meter Positions is disabled.")]
		public float AdrenalineMeterPosY { get; set; }

		[Header("Boss Rush Curses")]

		[Label("$Mods.CalamityMod.BRCurseAccessory")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Limits the player to five accessories during the Boss Rush.")]
		public bool BossRushAccessoryCurse { get; set; }

		[Label("$Mods.CalamityMod.BRCurseHealth")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Disables all health regeneration during the Boss Rush.")]
		public bool BossRushHealthCurse { get; set; }

		[Label("$Mods.CalamityMod.BRCurseDash")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("Disables all dashes during the Boss Rush.")]
		public bool BossRushDashCurse { get; set; }

		[Label("$Mods.CalamityMod.BRCurseImmunity")]
		[BackgroundColor(192, 54, 64, 192)]
		[DefaultValue(false)]
		[Tooltip("During the Boss Rush, being hit twice within three seconds will cause instant death.\nThis effect ignores revives.")]
		public bool BossRushImmunityFrameCurse { get; set; }

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) => true;




		public static void LoadConfigLabels(Mod instance)
		{
			string[][] _configLabels = new[]
			{
				new [] { "Afterimages", ItemID.SteampunkGoggles.ToString(), "Afterimages" },
				new [] { "MaxParticles", ItemID.FragmentStardust.ToString(), "Maximum particles" },
				new [] { "ScreenshakeOff", instance.ItemType("WavePounder").ToString(), "Disable Screenshakes" },
				new [] { "StealthInvisibility", instance.ItemType("StealthHairDye").ToString(), "Stealth Invisibility" },
				new [] { "ShopAlert", ItemID.GoldChest.ToString(), "New Shop Inventory Alert" },

				new [] { "BossHealthBar", instance.ItemType("BloodOrange").ToString(), "Boss Health Bars" },
				new [] { "BossHealthBarExtra", instance.ItemType("EncryptedSchematicPlanetoid").ToString(), "Boss Health Bar Extra Info" },
				new [] { "DebuffDisplay", ItemID.FlaskofIchor.ToString(), "Boss and Miniboss Debuff Display" },
				new [] { "CooldownDisplay", instance.ItemType("NebulousCore").ToString(), "Cooldown Display" },
				new [] { "VanillaCooldownDisplay", ItemID.HealingPotion.ToString(), "Vanilla Cooldowns Display" },
				new [] { "MeterLock", ItemID.GemLockTopaz.ToString(), "Lock Meter Positions" },

				new [] { "StealthMeter", instance.ItemType("EclipseMirror").ToString(), "Stealth Meter" },
				new [] { "StealthMeterX", ItemID.LaserRuler.ToString(), "Stealth Meter X Position" },
				new [] { "StealthMeterY", ItemID.LaserRuler.ToString(), "Stealth Meter Y Position" },

				new [] { "ChargeMeter", instance.ItemType("PowerCell").ToString(), "Charge Meter" },
				new [] { "ChargeMeterX", ItemID.LaserRuler.ToString(), "Charge Meter X Position" },
				new [] { "ChargeMeterY", ItemID.LaserRuler.ToString(), "Charge Meter Y Position" },

				new [] { "SpeedrunTimer", ItemID.Stopwatch.ToString(), "Speedrun Timer" },
				new [] { "SpeedrunTimerX", ItemID.LaserRuler.ToString(), "Speedrun Timer X Position" },
				new [] { "SpeedrunTimerY", ItemID.LaserRuler.ToString(), "Speedrun Timer Y Position" },

				new [] { "EarlyHMRework", ItemID.Pwnhammer.ToString(), "Early Hardmode Progression Rework" },
				new [] { "LethalLava", ItemID.LavaCharm.ToString(), "Lethal Lava" },
				new [] { "Proficiency", instance.ItemType("MagicLevelMeter").ToString(), "Proficiency" },
				new [] { "BossZen", instance.ItemType("ZenPotion").ToString(), "Boss Zen" },
				new [] { "NPCNightSpawn", ItemID.ClothierVoodooDoll.ToString(), "Let Town NPCs spawn at night" },
				new [] { "NPCExtraSpawn", ItemID.GuideVoodooDoll.ToString(), "Town NPC Spawn Rate Multiplier" },
				new [] { "BossHPBoost", ItemID.LifeCrystal.ToString(), "Boss Health Percentage Boost" },

				new [] { "ExpertDebuffReduction", ItemID.AnkhCharm.ToString(), "Reduce Expert Debuff Durations" },
				new [] { "ChillWaterRework", ItemID.ArcticDivingGear.ToString(), "Rework Chilled Water" },
				new [] { "ExpertSafeTowns", ItemID.Sunflower.ToString(), "Disable Expert Enemy Spawns in Towns" },
				new [] { "RipperBarShake", instance.ItemType("Revenge").ToString(), "Rage and Adrenaline Meter Shake" },
				new [] { "RageX", ItemID.LaserRuler.ToString(), "Rage Meter X Position" },
				new [] { "RageY", ItemID.LaserRuler.ToString(), "Rage Meter Y Position" },
				new [] { "AdrenalineX", ItemID.LaserRuler.ToString(), "Adrenaline Meter X Position" },
				new [] { "AdrenalineY", ItemID.LaserRuler.ToString(), "Adrenaline Meter Y Position" },

				new [] { "BRCurseAccessory", ItemID.Shackle.ToString(), "Accessory Curse" },
				new [] { "BRCurseHealth", ItemID.Shackle.ToString(), "Health Curse" },
				new [] { "BRCurseDash", ItemID.Shackle.ToString(), "Dash Curse" },
				new [] { "BRCurseImmunity", ItemID.Shackle.ToString(), "Immunity Frame Curse" }
			};

			foreach (string[] label in _configLabels)
            {
				ModTranslation text = instance.CreateTranslation(label[0]);
				text.SetDefault($"[i:" + label[1] + "]  "+ label[2]);
				instance.AddTranslation(text);
			}
		}
	}
}