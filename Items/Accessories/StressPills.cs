﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class StressPills : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stress Pills");
            Tooltip.SetDefault("Boosts your damage by 8%,\n" +
                               "defense by 8, and max movement speed and acceleration by 5%\n" +
                               "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.stressPills = true;
        }
    }
}