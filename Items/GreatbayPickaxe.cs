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
    public class GreatbayPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatbay Pickaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 13;
            item.melee = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useTurn = true;
            item.pick = 60;
            item.useStyle = 1;
            item.knockBack = 2;
            item.value = 57000;
            item.rare = 2;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}