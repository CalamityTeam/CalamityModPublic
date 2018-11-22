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
	public class Knowledge25 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plantera");
			Tooltip.SetDefault("Well done, you killed a plant.\n" +
                "However, this plant was used as a vessel to house the spirits of those unfortunate enough to find their way down here.\n" +
                "I wish you luck in dealing with the fallout.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 6;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}