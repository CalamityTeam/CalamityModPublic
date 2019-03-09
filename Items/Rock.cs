using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class Rock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rock");
            Tooltip.SetDefault("The first object Xeroc ever created");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
			item.value = Item.buyPrice(0, 0, 0, 1);
		}
    }
}