using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.SunkenSea
{
    public class MolluskHusk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk Husk");
            Tooltip.SetDefault("The remains of a mollusk");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 5;
        }
    }
}