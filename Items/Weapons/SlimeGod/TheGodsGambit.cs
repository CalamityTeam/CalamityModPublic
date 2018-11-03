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
    public class TheGodsGambit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The God's Gambit");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(3291);
            item.damage = 28;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 3.5f;
            item.value = 300000;
            item.rare = 4;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("TheGodsGambitProjectile");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 30);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}