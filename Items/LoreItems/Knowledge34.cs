using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.LoreItems
{
	public class Knowledge34 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Red Moon");
			Tooltip.SetDefault("We long ago feared the light of the red moon.\n" +
                "Many went mad, others died, but a scant few became blessed with a wealth of cosmic understanding.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 9;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}
