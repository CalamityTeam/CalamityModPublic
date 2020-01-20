using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CalamityMod
{
	[Label("Configs")]
	public class Configs : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;
		public override void OnLoaded() => CalamityMod.CalamityConfig = this;

		[Header("Expert Changes")]

		[DefaultValue(true)]
		[Label("Expert Debuff Duration Reduction")]
		[Tooltip("Debuffs in Expert Mode are no longer doubled")]
		public bool ExpertDebuffDurationReduction { get; set; }

		[DefaultValue(true)]
		[Label("Expert Chilled Water Removal")]
		[Tooltip("Water in the Ice and Snow biome in Expert Mode rapidly drain your breath rather than slowing your movement")]
		public bool ExpertChilledWaterRemoval { get; set; }

		[DefaultValue(true)]
		[Label("Expert Pillar Enemy Kill Count Reduction")]
		[Tooltip("Only 100 enemies are required to destroy the shields of the Celestial Pillars in Expert Mode")]
		public bool ExpertPillarEnemyKillCountReduction { get; set; }

		[DefaultValue(false)]
		[Label("Expert Enemy Spawns in Villages Reduction")]
		[Tooltip("Enemy spawns are drastically reduced in the presence of town NPCs in Expert Mode")]
		public bool DisableExpertEnemySpawnsNearHouse { get; set; }

		[Header("Revengeance Changes")]

		[DefaultValue(true)]
		[Label("Adrenaline and Rage")]
		[Tooltip("Enables the Adrenaline and Rage mechanics")]
		public bool AdrenalineAndRage { get; set; }

		[Label("Rage Meter Position X")]
		[Tooltip("Changes the X position of the Rage Meter")]
		[Range(0f, 1000f)]
		[DefaultValue(500f)]
		public float RageMeterPosX { get; set; }

		[Label("Rage Meter Position Y")]
		[Tooltip("Changes the Y position of the Rage Meter")]
		[Range(0f, 1000f)]
		[DefaultValue(30f)]
		public float RageMeterPosY { get; set; }

		[Label("Adrenaline Meter Position X")]
		[Tooltip("Changes the X position of the Adrenaline Meter")]
		[Range(0f, 1000f)]
		[DefaultValue(650f)]
		public float AdrenalineMeterPosX { get; set; }

		[Label("Adrenaline Meter Position Y")]
		[Tooltip("Changes the Y position of the Adrenaline Meter")]
		[Range(0f, 1000f)]
		[DefaultValue(30f)]
		public float AdrenalineMeterPosY { get; set; }

		[DefaultValue(false)]
		[Label("Revengeance and Death Thorium Boss buff")]
		[Tooltip("Buffs the health of Thorium bosses if Revengeance or Death is enabled")]
		public bool RevengeanceAndDeathThoriumBossBuff { get; set; }

		[Header("Boss Rush Curses")]

		[DefaultValue(false)]
		[Label("Boss Rush Accessory Curse")]
		[Tooltip("Accessories are limited to a maximum of five while the Boss Rush is active")]
		public bool BossRushAccessoryCurse { get; set; }

		[DefaultValue(false)]
		[Label("Boss Rush Health Curse")]
		[Tooltip("Life regeneration is disabled while the Boss Rush is active")]
		public bool BossRushHealthCurse { get; set; }

		[DefaultValue(false)]
		[Label("Boss Rush Dash Curse")]
		[Tooltip("Dashes are disabled while the Boss Rush is active")]
		public bool BossRushDashCurse { get; set; }

		[DefaultValue(false)]
		[Label("Boss Rush Xeroc Curse")]
		[Tooltip("All bosses are permanently enraged while the Boss Rush is active")]
		public bool BossRushXerocCurse { get; set; }

		[DefaultValue(false)]
		[Label("Boss Rush Immunity Frame Curse")]
		[Tooltip("Getting hit more than once in the span of five seconds will instantly kill you if the Boss Rush is active")]
		public bool BossRushImmunityFrameCurse { get; set; }

		[Header("Other")]

		[DefaultValue(true)]
		[Label("Proficiency")]
		[Tooltip("Enables the Proficiency stat that rewards the player based on the attack type they use")]
		public bool ProficiencyEnabled { get; set; }

		[DefaultValue(true)]
		[Label("Lethal Lava")]
		[Tooltip("Increases the severity of lava with a new debuff to punish those who stay in lava for too long")]
		public bool LethalLava { get; set; }

		[DefaultValue(true)]
		[Label("Boss Health Bar")]
		[Tooltip("Enables the Boss Health Bar to show while a boss or miniboss is alive")]
		public bool DrawBossBar { get; set; }

		[DefaultValue(true)]
		[Label("Boss Health Bar Small Text")]
		[Tooltip("Enables the small text below the health bar\nThis displays the exact health amount or remaining boss parts")]
		public bool DrawSmallText { get; set; }

		[DefaultValue(false)]
		[Label("Mining Speed Boost")]
		[Tooltip("Boosts the player's mining speed by 75%")]
		public bool MiningSpeedBoost { get; set; }

		[Label("Boss Health Percentage Boost")]
		[Tooltip("Boosts the health of bosses to a maximum of +900% health")]
		[Range(0, 900)]
		[DefaultValue(0)]
		public int BossHealthPercentageBoost { get; set; }

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
		{
			return true;
		}
	}
}