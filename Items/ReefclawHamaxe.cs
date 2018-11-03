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
    public class ReefclawHamaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reefclaw Hamaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.melee = true;
            item.width = 44;
            item.height = 44;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useTurn = true;
            item.axe = 13;
            item.hammer = 50;
            item.useStyle = 1;
            item.knockBack = 4;
            item.value = 15000;
            item.rare = 2;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
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