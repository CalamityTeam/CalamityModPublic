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
	public class Knowledge30 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrageldon Slime");
			Tooltip.SetDefault("A titanic slime infected by a star-borne disease expelled from the belly of an ancient god.\n" +
                "The destruction of this creature will not prevent the spread of the disease.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 7;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}