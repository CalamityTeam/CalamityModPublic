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
    public class SeashellBoomerangMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashell Boomerang");
        }

        public override void SetDefaults()
        {
            item.width = 18;  //The width of the .png file in pixels divided by 2.
            item.damage = 15;  //Keep this reasonable please.
            item.noMelee = true;  //Dictates whether this is a melee-class weapon.
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 5.5f;  //Ranges from 1 to 9.
            item.UseSound = SoundID.Item1;
            item.melee = true;  //Dictates whether the weapon can be "auto-fired".
            item.height = 34;  //The height of the .png file in pixels divided by 2.
            item.value = 50000;  //Value is calculated in copper coins.
            item.rare = 2;  //Ranges from 1 to 11.
            item.shoot = mod.ProjectileType("SeashellBoomerangProjectileMelee");
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
