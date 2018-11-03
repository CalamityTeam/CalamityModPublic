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
    public class AerialHamaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aerial Hamaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.melee = true;
            item.width = 48;
            item.height = 48;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useTurn = true;
            item.axe = 20;
            item.hammer = 75;
            item.useStyle = 1;
            item.knockBack = 7;
            item.value = 50000;
            item.rare = 3;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);  //Specific hamaxe 1 recipe.
            recipe.AddIngredient(null, "AerialiteBar", 6);
            recipe.AddIngredient(ItemID.SunplateBlock, 5);
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