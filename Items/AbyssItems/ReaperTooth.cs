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
    public class ReaperTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth");
            Tooltip.SetDefault("Sharp enough to cut diamonds");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = 100000;
            item.rare = 8;
        }
    }
}