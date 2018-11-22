using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class HardenedAstralSandWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hardened Astral Sand Wall");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.WallType("HardenedAstralSandWall");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(18);
            recipe.AddIngredient(mod.ItemType("HardenedAstralSand"));
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
