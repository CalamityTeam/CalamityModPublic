using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class AmidiasSpark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amidias' Spark");
            Tooltip.SetDefault("Taking damage releases a blast of sparks\n" +
                               "Sparks do extra damage in Hardmode");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 1;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.aSpark = true;
        }
    }
}