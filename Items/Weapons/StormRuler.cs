using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class StormRuler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Ruler");
            Tooltip.SetDefault("Only a storm can fell a greatwood");
        }

        public override void SetDefaults()
        {
            item.width = 74;
            item.damage = 105;
            item.melee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 82;
            item.value = 3000000;
            item.rare = 9;
            item.shoot = mod.ProjectileType("StormRuler");
            item.shootSpeed = 20f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CoreofCinder", 3);
            recipe.AddIngredient(null, "WindBlade");
            recipe.AddIngredient(null, "StormSaber");
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 187, (float)(player.direction * 2), 0f, 150, default(Color), 1.3f);
                Main.dust[num250].velocity *= 0.2f;
            }
        }
    }
}
