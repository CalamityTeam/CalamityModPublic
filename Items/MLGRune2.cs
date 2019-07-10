using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityMod.World;

namespace CalamityMod.Items
{
	public class MLGRune2 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Celestial Onion");
			Tooltip.SetDefault("Doesn't do anything currently...or does it!?\n" +
			                   "Consuming it does something that cannot be reversed");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.rare = 10;
			item.maxStack = 99;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.extraAccessoryML)
			{
				return false;
			}
			return true;
		}

		public override bool UseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.itemAnimation > 0 && !modPlayer.extraAccessoryML && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				modPlayer.extraAccessoryML = true;
				if (!CalamityWorld.onionMode)
				{
					CalamityWorld.onionMode = true;
				}
			}
			return true;
		}
	}
}
