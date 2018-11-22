using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Cryogen
{
    public class CryoBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigid Bar");
            Tooltip.SetDefault("Cold to the touch");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = 28750;
            item.rare = 5;
        }
    }
}