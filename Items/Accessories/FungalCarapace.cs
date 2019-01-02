using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
	public class FungalCarapace : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fungal Carapace");
			Tooltip.SetDefault("You emit a mushroom spore explosion when you are hit");
		}
		
		public override void SetDefaults()
		{
			item.defense = 2;
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 1;
			item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.fCarapace = true;
		}
	}
}