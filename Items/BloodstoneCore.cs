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
    public class BloodstoneCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodstone Core");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = 187500;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Bloodstone", 5);
            recipe.AddIngredient(null, "BloodOrb", 2);
            recipe.AddIngredient(null, "Phantoplasm");
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}