using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Placeables
{
    public class SmoothVoidstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smooth Voidstone");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("SmoothVoidstone");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = 1000;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Voidstone");
            recipe.SetResult(this);
            recipe.AddTile(18);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothVoidstoneWall", 4);
            recipe.SetResult(this);
            recipe.AddTile(18);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothVoidstonePlatform", 2);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}