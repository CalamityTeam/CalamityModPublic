using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class StatMeter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stat Meter");
			Tooltip.SetDefault("Displays info about most of your stats");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.value = Item.buyPrice(0, 6, 0, 0);
			item.rare = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> list)
		{
			CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
			int defense = modPlayer.defenseStat;
			int DR = modPlayer.DRStat;
			int meleeSpeed = modPlayer.meleeSpeedStat;
			int manaCost = modPlayer.manaCostStat;
			int rogueVelocity = modPlayer.rogueVelocityStat;
			int minionSlots = modPlayer.minionSlotStat;
			int lifeRegen = modPlayer.lifeRegenStat;
			int manaRegen = modPlayer.manaRegenStat;
			int armorPenetration = modPlayer.armorPenetrationStat;
			int wingFlightTime = modPlayer.wingFlightTimeStat;
			int moveSpeed = modPlayer.moveSpeedStat;
			int lightLevel = modPlayer.abyssLightLevelStat;
			int breathLoss = modPlayer.abyssBreathLossStat;
			int breathLossRate = modPlayer.abyssBreathLossRateStat;
			int lifeLost = modPlayer.abyssLifeLostAtZeroBreathStat;

			if (CalamityWorld.revenge && Config.AdrenalineAndRage)
			{
				int adrenalineChargeTime = modPlayer.adrenalineChargeStat;
				int rageDamage = modPlayer.rageDamageStat;

				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Adrenaline Charge Time: " + adrenalineChargeTime + " seconds | Rage Damage Boost: " + rageDamage + "%\n\n" +
							"Melee Damage: " + modPlayer.damageStats[0] + "% | Melee Crit Chance: " + modPlayer.critStats[0] + "%\n" +
							"Melee Speed Boost: " + meleeSpeed + "%\n\n" +
							"Ranged Damage: " + modPlayer.damageStats[1] + "% | Ranged Crit Chance: " + modPlayer.critStats[1] + "%\n\n" +
							"Magic Damage: " + modPlayer.damageStats[2] + "% | Magic Crit Chance: " + modPlayer.critStats[2] + "%\n" +
							"Mana Usage: " + manaCost + "% | Mana Regen: " + manaRegen + "\n\n" +
							"Minion Damage: " + modPlayer.damageStats[3] + "% | Minion Slots: " + minionSlots + "\n\n" +
							"Rogue Damage: " + modPlayer.damageStats[4] + "% | Rogue Crit Chance: " + modPlayer.critStats[3] + "%\n" +
							"Rogue Velocity Boost: " + rogueVelocity + "%\n\n" +
							"Defense: " + defense + " | DR: " + DR + "%\n" +
							"Life Regen: " + lifeRegen + " | Armor Penetration: " + armorPenetration + "\n" +
							"Wing Flight Time: " + wingFlightTime + " | Movement Speed: " + moveSpeed + "\n\n" +
							"Abyss Light Level: " + lightLevel + " | Abyss Breath Loss Per Tick: " + breathLoss + "\n" +
							"Abyss Breath Loss Rate: " + breathLossRate + " | Abyss Life Lost Per Tick At Zero Breath: " + lifeLost;
					}
				}
			}
			else
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Melee Damage: " + modPlayer.damageStats[0] + "% | Melee Crit Chance: " + modPlayer.critStats[0] + "%\n" +
							"Melee Speed Boost: " + meleeSpeed + "%\n\n" +
							"Ranged Damage: " + modPlayer.damageStats[1] + "% | Ranged Crit Chance: " + modPlayer.critStats[1] + "%\n\n" +
							"Magic Damage: " + modPlayer.damageStats[2] + "% | Magic Crit Chance: " + modPlayer.critStats[2] + "%\n" +
							"Mana Usage: " + manaCost + "% | Mana Regen: " + manaRegen + "\n\n" +
							"Minion Damage: " + modPlayer.damageStats[3] + "% | Minion Slots: " + minionSlots + "\n\n" +
							"Rogue Damage: " + modPlayer.damageStats[4] + "% | Rogue Crit Chance: " + modPlayer.critStats[3] + "%\n" +
							"Rogue Velocity Boost: " + rogueVelocity + "%\n\n" +
							"Defense: " + defense + " | DR: " + DR + "%\n" +
							"Life Regen: " + lifeRegen + " | Armor Penetration: " + armorPenetration + "\n" +
							"Wing Flight Time: " + wingFlightTime + " | Movement Speed: " + moveSpeed + "\n\n" +
							"Abyss Light Level: " + lightLevel + " | Abyss Breath Loss Per Tick: " + breathLoss + "\n" +
							"Abyss Breath Loss Rate: " + breathLossRate + " | Abyss Life Lost Per Tick At Zero Breath: " + lifeLost;
					}
				}
			}
		}
	}
}
