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
    public class TruePaladinsHammerMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Paladin's Hammer");
        }

        public override void SetDefaults()
        {
            item.width = 14;  //The width of the .png file in pixels divided by 2.
            item.damage = 128;  //Keep this reasonable please.
            item.noMelee = true;  //Dictates whether this is a melee-class weapon.
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 13;
            item.useStyle = 1;
            item.useTime = 13;
            item.knockBack = 20f;  //Ranges from 1 to 9.
            item.UseSound = SoundID.Item1;
            item.melee = true;  //Dictates whether the weapon can be "auto-fired".
            item.height = 28;  //The height of the .png file in pixels divided by 2.
            item.value = 9000000;  //Value is calculated in copper coins.
            item.rare = 9;  //Ranges from 1 to 11.
            item.shoot = mod.ProjectileType("OPHammerMelee");
            item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PaladinsHammer);
            recipe.AddIngredient(null, "CalamityDust", 5);
            recipe.AddIngredient(null, "CoreofChaos", 5);
            recipe.AddIngredient(null, "CruptixBar", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
