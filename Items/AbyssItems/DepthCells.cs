using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.AbyssItems
{
    public class DepthCells : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depth Cells");
            Tooltip.SetDefault("The cells of abyssal creatures");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = 10000;
            item.rare = 3;
        }
    }
}