using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.HiveMind
{
	public class LeechingDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Leeching Dagger");
			Tooltip.SetDefault("Enemies release homing leech orbs on death");
		}

		public override void SetDefaults()
		{
			item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 15;
			item.useTime = 15;
			item.width = 26;
			item.height = 26;
			item.damage = 26;
			item.melee = true;
			item.knockBack = 5.25f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.RottenChunk, 2);
	        recipe.AddIngredient(ItemID.DemoniteBar, 5);
	        recipe.AddIngredient(null, "TrueShadowScale", 4);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 14);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	if (target.life <= 0)
	    	{
	    		Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("Leech"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
	    	}
		}
	}
}
