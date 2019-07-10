using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Bar");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("AstralBar");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 99;
            item.rare = 9;
			item.value = Item.buyPrice(0, 5, 50, 0);
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Stardust", 12);
            recipe.AddIngredient(mod.ItemType("AstralOre"), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}
