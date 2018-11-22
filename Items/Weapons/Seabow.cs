using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons
{
    public class Seabow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seabow");
        }

        public override void SetDefaults()
        {
            item.damage = 14;
            item.ranged = true;
            item.width = 22;
            item.height = 46;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = 45000;
            item.rare = 2;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 16f;
            item.useAmmo = 40;
        }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

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