using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Accessories
{
	public class FrostFlare : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frost Flare");
			Tooltip.SetDefault("All melee attacks and projectiles inflict frostburn\n" +
				"Immunity to frostburn, chilled, and frozen\n" +
				"Resistant to cold attacks and +1 life regen\n" +
				"Being above 75% life grants the player 10% increased damage\n" +
				"Being below 25% life grants the player 20 defense and 15% increased max movement speed and acceleration\n" +
                "Revengeance drop");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 10));
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 24;
			item.lifeRegen = 1;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = 5;
			item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.frostFlare = true;
			player.resistCold = true;
			player.buffImmune[44] = true;
			player.buffImmune[46] = true;
			player.buffImmune[47] = true;
			if (player.statLife > (player.statLifeMax2 * 0.75f))
			{
				player.meleeDamage += 0.1f;
				player.magicDamage += 0.1f;
				player.rangedDamage += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
				player.minionDamage += 0.1f;
			}
			if (player.statLife < (player.statLifeMax2 * 0.25f))
			{
				player.statDefense += 20;
			}
		}
	}
}