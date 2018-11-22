using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.GreatSandShark
{
    public class GrandScale : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Scale");
            Tooltip.SetDefault("Large scale of an apex predator");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = 50000;
            item.rare = 7;
        }
    }
}