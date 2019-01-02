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
    public class WulfrumHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Hammer");
        }

        public override void SetDefaults()
        {
            item.damage = 7;
            item.melee = true;
            item.width = 56;
            item.height = 56;
            item.useTime = 8;
            item.useAnimation = 16;
            item.useTurn = true;
            item.hammer = 35;
            item.useStyle = 1;
            item.knockBack = 4f;
            item.value = 25000;
            item.rare = 1;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "WulfrumShard", 16);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}