using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;
using System.Text;

namespace CalamityMod.Items.Accessories
{
	public class StatMeter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stat Meter");
			Tooltip.SetDefault("Displays info about most of your stats\n" +
			"Offensive stats displayed vary with held item");
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
			Player player = Main.player[Main.myPlayer];
			if (player is null)
				return;
			CalamityPlayer modPlayer = player.Calamity();

			Item heldItem = null;
			if (player.selectedItem >= 0 && player.selectedItem < player.inventory.Length)
				heldItem = player.inventory[player.selectedItem];

			foreach (TooltipLine line2 in list)
				if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					line2.text = CreateStatMeterTooltip(player, modPlayer, heldItem);
		}

		private string CreateStatMeterTooltip(Player player, CalamityPlayer modPlayer, Item heldItem)
		{
			int defense = modPlayer.defenseStat;
			int DR = modPlayer.DRStat;
			int meleeSpeed = modPlayer.meleeSpeedStat;
			int manaCost = modPlayer.manaCostStat;
			int rogueVelocity = modPlayer.rogueVelocityStat;
			int minionSlots = modPlayer.minionSlotStat;
			int ammoConsumption = modPlayer.ammoReductionRanged;
			int rogueConsumption = modPlayer.ammoReductionRogue;
			int lifeRegen = modPlayer.lifeRegenStat;
			int manaRegen = modPlayer.manaRegenStat;
			int armorPenetration = modPlayer.armorPenetrationStat;
			int wingFlightTime = modPlayer.wingFlightTimeStat;
			int moveSpeed = modPlayer.moveSpeedStat;
			int lightLevel = modPlayer.abyssLightLevelStat;
			int breathLossRate = modPlayer.abyssBreathLossRateStat;

			// The notice about held item mattering is always displayed first.
			StringBuilder sb = new StringBuilder("Offensive stats displayed vary with held item\n\n", 1024);

			// Only append rippers stats in Rev+, if rippers are enabled.
			if (CalamityWorld.revenge && CalamityMod.CalamityConfig.AdrenalineAndRage)
			{
				sb.Append("Adrenaline Charge Time: ").Append(modPlayer.adrenalineChargeStat)
					.Append(" seconds | Rage Damage Boost: ").Append(modPlayer.rageDamageStat)
					.Append("%\n\n");
			}

			// Append item stats only if the held item isn't null, and base it off of the item's damage type.
			if(heldItem != null)
			{
				if(heldItem.melee)
				{
					sb.Append("Melee Damage: ").Append(modPlayer.damageStats[0])
						.Append("% | Melee Crit Chance: ").Append(modPlayer.critStats[0])
						.Append("%\nMeleeSpeed Boost: ").Append(meleeSpeed).Append("%\n\n");
				}
				else if(heldItem.ranged)
				{
					sb.Append("Ranged Damage: ").Append(modPlayer.damageStats[1])
						.Append("% | Ranged Crit Chance: ").Append(modPlayer.critStats[1])
						.Append("%\nAmmo Consumption Chance: ").Append(ammoConsumption).Append("%\n\n");
				}
				else if(heldItem.magic)
				{
					sb.Append("Magic Damage: ").Append(modPlayer.damageStats[2])
						.Append("% | Magic Crit Chance: ").Append(modPlayer.critStats[2])
						.Append("%\nMana Usage: ").Append(manaCost)
						.Append("% | Mana Regen").Append(manaRegen).Append("\n\n");
				}
				else if(heldItem.summon)
				{
					sb.Append("Minion Damage: ").Append(modPlayer.damageStats[3])
						.Append("% | Minion Slots: ").Append(minionSlots).Append("\n\n");
				}
				else if(heldItem.Calamity().rogue)
				{
					sb.Append("Rogue Damage: ").Append(modPlayer.damageStats[4])
						.Append("% | Rogue Crit Chance: ").Append(modPlayer.critStats[3])
						.Append("%\nRogue Velocity Boost: ").Append(rogueVelocity)
						.Append("% | Rogue Weapon Consumption Chance: ").Append(rogueConsumption).Append("%\n\n");
				}
			}

			// Generic stats always render.
			sb.Append("Defense: ").Append(defense);
			sb.Append(" | DR: ").Append(DR).Append("%\n");
			sb.Append("Life Regen: ").Append(lifeRegen);
			sb.Append(" | Armor Penetration: ").Append(armorPenetration).Append("\n");
			sb.Append("Wing Flight Time: ").Append(wingFlightTime);
			sb.Append(" | Movement Speed Boost: ").Append(moveSpeed).Append("%\n\n");

			// Abyss stats always render.
			sb.Append(CalamityWorld.death ? "Abyss/Cave Light Strength: " : "Abyss Light Strength: ").Append(lightLevel).Append("\n");
			sb.Append("Breath Lost Per Tick:\nLayer 1: ").Append(modPlayer.abyssBreathLossStats[0]);
			sb.Append(" | Layer 2: ").Append(modPlayer.abyssBreathLossStats[1]).Append("\n");
			sb.Append("Layer 3: ").Append(modPlayer.abyssBreathLossStats[2]);
			sb.Append(" | Layer 4: ").Append(modPlayer.abyssBreathLossStats[3]).Append("\n");
			sb.Append("Breath Loss Rate: ").Append(breathLossRate).Append("\n");
			sb.Append("Life Lost Per Tick at Zero Breath:\nLayer 1: ").Append(modPlayer.abyssLifeLostAtZeroBreathStats[0]);
			sb.Append(" | Layer 2: ").Append(modPlayer.abyssLifeLostAtZeroBreathStats[1]).Append("\n");
			sb.Append("Layer 3: ").Append(modPlayer.abyssLifeLostAtZeroBreathStats[2]);
			sb.Append(" | Layer 4: ").Append(modPlayer.abyssLifeLostAtZeroBreathStats[3]);

			return sb.ToString();
		}
	}
}
