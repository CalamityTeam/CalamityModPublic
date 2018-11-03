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
    public class SeashellBoomerang : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashell Boomerang");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.damage = 15;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.thrown = true;
            item.height = 34;
            item.value = 50000;
            item.rare = 2;
            item.shoot = mod.ProjectileType("SeashellBoomerangProjectile");
            item.shootSpeed = 11.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
