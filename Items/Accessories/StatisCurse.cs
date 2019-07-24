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
    public class StatisCurse : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Curse");
            Tooltip.SetDefault("Increased max minions by 3 and 10% increased minion damage\n" +
                "Increased minion knockback\n" +
                "Grants shadowflame powers to all minions\n" +
                "Minions make enemies cry on hit");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 10;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.shadowMinions = true;
            modPlayer.tearMinions = true;
            player.minionKB += 2.5f;
            player.minionDamage += 0.1f;
            player.maxMinions += 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FragmentStardust, 10);
            recipe.AddIngredient(null, "StatisBlessing");
            recipe.AddIngredient(null, "TheFirstShadowflame");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}