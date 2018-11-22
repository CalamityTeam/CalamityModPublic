using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class OmegaBiomeBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omega Biome Blade");
			Tooltip.SetDefault("Fires different homing projectiles based on what biome you're in\nProjectiles also change based on moon events");
		}

		public override void SetDefaults()
		{
			item.width = 62;
			item.damage = 220;
			item.melee = true;
			item.useAnimation = 18;
			item.useTime = 18;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 8;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 62;
			item.value = 1200000;
			item.rare = 9;
			item.shoot = mod.ProjectileType("OmegaBiomeOrb");
			item.shootSpeed = 15f;
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			for (int projectiles = 0; projectiles <= 2; projectiles++)
			{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("OmegaBiomeOrb"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
			}
	    	return false;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "TrueBiomeBlade");
			recipe.AddIngredient(null, "CoreofCalamity");
			recipe.AddIngredient(null, "BarofLife", 3);
			recipe.AddIngredient(null, "GalacticaSingularity", 3);
			recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 0);
	        }
	    }
	}
}
