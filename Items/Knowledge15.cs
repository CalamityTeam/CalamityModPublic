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
	public class Knowledge15 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Slime God");
			Tooltip.SetDefault("It is a travesty, one of the most threatening biological terrors ever born.\n" +
                "If this creature were allowed to combine every slime on the planet then it would be nearly unstoppable.");
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