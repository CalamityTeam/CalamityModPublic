using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class Brimlash : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimlash");
		}

		public override void SetDefaults()
		{
			item.width = 50;
			item.damage = 64;
			item.melee = true;
			item.useAnimation = 25;
			item.useTime = 25;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 6f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 50;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
			item.shoot = mod.ProjectileType("Brimlash");
			item.shootSpeed = 15f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UnholyCore", 4);
	        recipe.AddIngredient(null, "EssenceofChaos", 3);
	        recipe.AddIngredient(null, "LivingShard", 5);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 235);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);
		}
	}
}
