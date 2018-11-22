using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class Lucrecia : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lucrecia");
			Tooltip.SetDefault("Finesse\nStriking an enemy with the blade has a 50% chance to make you immune for a short time");
		}

		public override void SetDefaults()
		{
			item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 25;
			item.useTime = 25;
			item.width = 58;
			item.height = 58;
			item.damage = 75;
			item.melee = true;
			item.knockBack = 8.25f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("DNA");
			item.shootSpeed = 32f;
			item.value = 3000000;
			item.rare = 8;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CoreofCalamity");
			recipe.AddIngredient(null, "BarofLife", 5);
			recipe.AddIngredient(ItemID.SoulofLight, 5);
			recipe.AddIngredient(ItemID.SoulofNight, 5);
	        recipe.AddTile(TileID.MythrilAnvil);	
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 234);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	if (Main.rand.Next(2) == 0)
	    	{
				player.immune = true;
				player.immuneTime = 15;
	    	}
		}
	}
}
