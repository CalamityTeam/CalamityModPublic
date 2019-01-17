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
    public class Floodtide : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Floodtide");
			Tooltip.SetDefault("Launches sharks, because sharks are awesome!");
		}

        public override void SetDefaults()
        {
            item.damage = 89;
            item.melee = true;
            item.width = 60;
            item.height = 64;
            item.useTime = 23;
            item.useAnimation = 23;
			item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = 408;
            item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 1f, 0.0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 217);
            }
        }
        
        public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "VictideBar", 5);
	        recipe.AddIngredient(ItemID.SharkFin, 2);
	        recipe.AddIngredient(ItemID.AdamantiteBar, 5);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "VictideBar", 5);
	        recipe.AddIngredient(ItemID.SharkFin, 2);
	        recipe.AddIngredient(ItemID.TitaniumBar, 5);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
    }
}