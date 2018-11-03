using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class TarragonThrowingDart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Throwing Dart");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.damage = 184;
            item.thrown = true;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 34;
            item.maxStack = 999;
            item.value = 25000;
            item.shoot = mod.ProjectileType("TarragonThrowingDartProjectile");
            item.shootSpeed = 24f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
