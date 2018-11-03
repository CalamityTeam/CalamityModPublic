using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class VerstaltiteBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryonic Bar");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = 100000;
            item.rare = 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryonicOre", 16);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}