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
    public class DepthCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depths Charm");
            Tooltip.SetDefault("Reduces the damage caused by the pressure of the abyss while out of breath\n" +
                "Removes the bleed effect caused by the abyss");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 50000;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.depthCharm = true;
        }
    }
}