using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.SlimeGod
{
    public class GelDart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Dart");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.damage = 25;
            item.thrown = true;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 28;
            item.maxStack = 999;
            item.value = 1000;
            item.rare = 4;
            item.shoot = mod.ProjectileType("GelDartProjectile");
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 150);
            recipe.AddRecipe();
        }
    }
}
