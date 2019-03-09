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
    public class UeliaceBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Uelibloom Bar");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 6, 50, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UelibloomOre", 12);
            recipe.AddIngredient(null, "UnholyEssence");
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}