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
	public class Knowledge5 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crabulon");
			Tooltip.SetDefault("A crab and its mushrooms, a love story.\n" +
                "It's interesting how creatures can adapt given certain circumstances.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 2;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}