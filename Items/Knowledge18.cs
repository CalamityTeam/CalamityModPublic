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
	public class Knowledge18 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wall of Flesh");
			Tooltip.SetDefault("I see the deed is done.\n" +
                "The unholy amalgamation of flesh and hatred has been defeated.\n" +
                "Prepare to face the true terrors that lurk in the light and dark parts of this world.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 4;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}