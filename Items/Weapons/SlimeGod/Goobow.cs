using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons.SlimeGod
{
    public class Goobow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goobow");
        }

        public override void SetDefaults()
        {
            item.damage = 42;
            item.ranged = true;
            item.width = 30;
            item.height = 48;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = 105000;
            item.rare = 4;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 12f;
            item.useAmmo = 40;
        }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 18);
            recipe.AddIngredient(ItemID.Gel, 30);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}