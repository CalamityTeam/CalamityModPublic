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
    public class WindBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wind Blade");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 29;
            item.melee = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.useTurn = true;
            item.knockBack = 5;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.value = 100000;
            item.rare = 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AerialiteBar", 9);
            recipe.AddIngredient(ItemID.SunplateBlock, 3);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(3) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 59);
            }
        }
    }
}
