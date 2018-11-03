using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Cryogen
{
    public class Icebreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icebreaker");
        }

        public override void SetDefaults()
        {
            item.width = 60;  //The width of the .png file in pixels divided by 2.
            item.damage = 57;  //Keep this reasonable please.
            item.noMelee = true;  //Dictates whether this is a melee-class weapon.
            item.noUseGraphic = true;
            item.useAnimation = 14;
            item.useStyle = 1;
            item.useTime = 14;
            item.knockBack = 6.75f;  //Ranges from 1 to 9.
            item.UseSound = SoundID.Item1;
            item.melee = true;  //Dictates whether the weapon can be "auto-fired".
            item.height = 60;  //The height of the .png file in pixels divided by 2.
            item.value = 400000;  //Value is calculated in copper coins.
            item.rare = 5;  //Ranges from 1 to 11.
            item.shoot = mod.ProjectileType("Icebreaker");
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryoBar", 11);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
