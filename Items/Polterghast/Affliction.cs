using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Polterghast
{
	public class Affliction : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Affliction");
			Tooltip.SetDefault("Gives you and all other players on your team +1 life regen,\n" +
							   "+5% max life, 5% damage reduction, 15 defense, and 10% increased damage\n" +
							   "These effects are stronger on revengeance mode");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.value = Item.buyPrice(0, 60, 0, 0);
			item.accessory = true;
			item.expert = true;
			item.rare = 9;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.affliction = true;
			if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
			{
				int myPlayer = Main.myPlayer;
				if (Main.player[myPlayer].team == player.team && player.team != 0)
				{
					Main.player[myPlayer].AddBuff(mod.BuffType("Afflicted"), 20, true);
				}
			}
		}
	}
}