using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class ElectrolyteGelPack : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electrolyte Gel Pack");
			Tooltip.SetDefault("Permanently makes Adrenaline Mode take 10 less seconds to charge\n" +
                "Revengeance drop");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.rare = 4;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item122;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.adrenalineBoostOne)
			{
				return false;
			}
			return true;
		}

		public override bool UseItem(Player player)
		{
			if (player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.adrenalineBoostOne = true;
			}
			return true;
		}
	}
}